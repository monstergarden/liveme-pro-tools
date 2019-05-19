using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMPT.Core.Contract.DB;
using LMPT.Core.Contract.DB.Bookmark;
using LMPT.Core.Contract.DB.Cache;
using LMPT.Core.Contract.FromLiveMe;
using LMPT.Core.Services.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using User = LMPT.Core.Contract.DB.Cache.User;

namespace LMPT.Core.Services.Data
{
    public class DataAccess
    {
        private readonly DbContextFactory _dbContextFactory;
        private readonly ILogger<DataAccess> logger;

        public DataAccess(ILogger<DataAccess> logger, DbContextFactory dbContextFactory)
        {
            this.logger = logger;
            _dbContextFactory = dbContextFactory;
        }

        public async Task MigrateAsync()
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                await db.Database.MigrateAsync();
            }
        }

        public LivemeAuthentication FindAuthToken()
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                return db.LivemeAuthentication.FirstOrDefault();
            }
        }

        public List<Replay> FindReplaysFromUid(string uid)
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                return db.Replays.Where(x => x.FromUser.UId == uid).ToList();
            }
        }

        public List<Replay> GetLastReplaysFromUid(string uid, int count)
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                return db.Replays
                    .Where(x => x.FromUser.UId == uid)
                    .OrderByDescending(x => x.StartTimeStamp)
                    .Take(count)
                    .ToList();
            }
        }

        public void StoreAuthToken(LivemeAuthentication authentication)
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                var found = db.LivemeAuthentication.FirstOrDefault();
                if (found != null) db.LivemeAuthentication.Remove(found);

                db.LivemeAuthentication.Add(authentication);
                db.SaveChanges();
            }
        }

        public Replay FindReplayByVid(string videoId)
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                return db.Replays.FirstOrDefault(x => x.VId == videoId);
            }
        }

        public List<Replay> FindReplaysByUid(string uid)
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                return db.Replays.Where(x => x.FromUser.UId == uid).ToList();
            }
        }

        public void SetReplayToWatched(string videoId)
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                var replay = db.Replays.First(x => x.VId == videoId);
                replay.Watched = true;
                db.SaveChanges();
            }
        }

        public void HouseKeeping()
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                var allBookmarks = GetAllBookmarks().ToList();
                var countMinus50 = allBookmarks.Count - 50;
                if (countMinus50 <= 0) return;
                var toDelete = allBookmarks
                    .Take(countMinus50)
                    .Where(x => x.ReplayCount == 0 && x.BookmarkType != BookmarkType.Follower)
                    .ToList();

                foreach (var b in toDelete)
                {
                    b.Deleted = true;
                    b.LastUpdated = DateTime.UtcNow;
                }


                db.SaveChanges();
                logger.LogInformation($"Deleted: {toDelete.Count} bookmarks:");
                toDelete.ForEach(x => logger.LogInformation("Deleted - " + x.Nickname));
            }
        }

        public IEnumerable<Bookmark> GetAllBookmarks()
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                return db.Bookmarks.Where(x => !x.Deleted).ToList();
            }
        }

        public ProfileSeen FindProfileWatched(string uid)
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                return db.ProfileSeen.FirstOrDefault(x => x.Uid == uid);
            }
        }

        public List<User> GetLastProfilesVisited()
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                var lastSeen = db.ProfileSeen.OrderByDescending(x => x.Seen).Take(10).ToList();
                var users = lastSeen.Select(x => db.User.FirstOrDefault(y => y.UId == x.Uid))
                    .Where(x => x != null).ToList();
                return users;
            }
        }

        public void AddToBookmark(LiveMeUser user)
        {
            var info = user.UserInfo;

            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                var found = db.Bookmarks.Find(info.uid);
                if (found != null)
                    found.Deleted = false;
                else
                    db.Bookmarks.Add(new Bookmark
                    {
                        Uid = info.uid,
                        Face = info.face,
                        Gender = info.sex == "male" ? Gender.Male : Gender.Female,
                        Nickname = info.uname,
                        Shortid = Convert.ToInt64(info.short_id),
                        Signature = info.usign,
                        ReplayCount = user.CountInfo.ReplayCount,
                        FollowerCount = user.CountInfo.FollowerCount,
                        FollowingCount = user.CountInfo.FollowingCount
                    });


                db.SaveChanges();
            }
        }

        internal void UpdateBookmark(string uid, LiveMeUser liveMeUser)
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                var bookmark = db.Bookmarks.Find(uid);
                bookmark.ReplayCount = liveMeUser.CountInfo.VideoCount;
                bookmark.FollowerCount = liveMeUser.CountInfo.FollowerCount;
                bookmark.FollowingCount = liveMeUser.CountInfo.FollowingCount;
                bookmark.LastUpdated = DateTime.UtcNow;
                try
                {
                    bookmark.Signature = liveMeUser.UserInfo.usign;
                    bookmark.Face = liveMeUser.UserInfo.face;
                    bookmark.Nickname = liveMeUser.UserInfo.uname;
                    bookmark.Shortid = long.Parse(liveMeUser.UserInfo.short_id);
                }
                catch (Exception)
                {
                    // ignore exception if something went wrong in updting the above UserInfo,
                    // because this is not that important and should not break the scan.
                }

                db.SaveChanges();
            }
        }

        internal IEnumerable<ScanResult> LoadLastScanResults()
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                var lastScan = db.LastScanResult.Include(x => x.Bookmark).ToList();
                return lastScan;
            }
        }

        public void LikeReplay(string vid)
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                var replay = db.Replays.Find(vid);
                replay.Liked = true;
                db.SaveChanges();
            }
        }

        internal void ReplaceLastScanResult(IEnumerable<ScanResult> results)
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                foreach (var row in db.LastScanResult) db.LastScanResult.Remove(row);

                db.SaveChanges();
                db.LastScanResult.AddRange(results);
                db.SaveChanges();
            }
        }

        public void RemoveBookmark(string uid)
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                var found = db.Bookmarks.Find(uid);
                if (found != null)
                {
                    found.Deleted = true;
                    db.SaveChanges();
                }
            }
        }

        public bool IsBookmarked(string uid)
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                var found = db.Bookmarks.Find(uid);
                if (found == null) return false;

                return !found.Deleted;
            }
        }

        public void ChangeBookmarkType(string uid, BookmarkType type)
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                var found = db.Bookmarks.Find(uid);

                if (found != null)
                {
                    found.BookmarkType = type;
                    db.SaveChanges();
                }
            }
        }

        public void SetProfileToSeen(string uid)
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                var find = db.ProfileSeen.Find(uid);
                if (find != null)
                    find.Seen = DateTime.UtcNow.ToUnixTimestamp();
                else
                    db.ProfileSeen.Add(new ProfileSeen
                    {
                        Uid = uid,
                        Seen = DateTime.UtcNow.ToUnixTimestamp()
                    });

                db.SaveChanges();
            }
        }

        public void ToggleBookmark(LiveMeUser user)
        {
            var uid = user.UserInfo.uid;

            if (IsBookmarked(uid))
                RemoveBookmark(uid);
            else
                AddToBookmark(user);
        }
    }
}