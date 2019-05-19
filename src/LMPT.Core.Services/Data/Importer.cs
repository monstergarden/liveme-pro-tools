using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LMPT.Core.Contract.DB.Bookmark;
using LMPT.Core.Contract.DB.Cache;
using LMPT.Core.Contract.LegacyJson;
using LMPT.DB;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LMPT.Core.Services.Data
{
    public class Importer
    {
        private readonly ConfigurationProvider _config;
        private readonly DbContextFactory _dbFactory;
        private readonly ILogger<Importer> _logger;

        public Importer(ILogger<Importer> logger, DbContextFactory dbContextFac, ConfigurationProvider config)
        {
            _logger = logger;
            _dbFactory = dbContextFac;
            _config = config;
        }


        public async Task ImportBookmarksFromFile()
        {
            _logger.LogInformation($"Reading json file: {_config.OldBookmarksJsonFile}");
            var jsonBookmark = File.ReadAllText(_config.OldBookmarksJsonFile);

            _logger.LogInformation("Deserializing...");
            var forDbImport = ConvertBookmarkJsonToDb(jsonBookmark).ToList();

            _logger.LogInformation($"Found {forDbImport.Count} entries in json. Now importing to DB ...");
            var sucessCounter = await Import(forDbImport);

            _logger.LogInformation(
                $"Found Bookmarks in Json: {forDbImport.Count}; Sucessfully imported in DB: {sucessCounter}");
        }

        private async Task<int> Import<T>(IEnumerable<T> forDbImport) where T : class
        {
            var successfullCounter = 0;
            foreach (var b in forDbImport)
                using (var db = _dbFactory.CreateApplicationDbContext())
                {
                    try
                    {
                        var res = db.Add(b);

                        await db.SaveChangesAsync();
                        successfullCounter++;
                    }
                    catch (Exception e)
                    {
                        _logger.LogDebug(e.Message);
                    }
                }

            return successfullCounter;
        }

        public async Task ImportWatchedFromFile()
        {
            _logger.LogInformation($"Reading json file: {_config.OldWatchedJsonFile}");
            var jsonRaw = File.ReadAllText(_config.OldWatchedJsonFile);

            _logger.LogInformation("Deserializing...");
            var watched = JsonConvert.DeserializeObject<List<WatchedJson>>(jsonRaw);
            var newCount = 0;
            _logger.LogInformation($"Found {watched.Count} entries in json. Now importing to DB ...");


            using (var db = _dbFactory.CreateApplicationDbContext())
            {
                foreach (var w in watched)
                {
                    var found = db.Replays.Find(w.videoid);
                    if (found == null)
                    {
                        db.Add(new Replay
                        {
                            Watched = true,
                            VId = w.videoid,
                            CreatedAt = DateTimeOffset.Now.ToUnixTimeSeconds()
                        });
                        newCount++;
                    }
                    else
                    {
                        found.Watched = true;
                    }
                }

                await db.SaveChangesAsync();
            }

            _logger.LogInformation($"Sucessfully imported New Watched Replays: {newCount}");
        }

        public IEnumerable<Bookmark> ConvertBookmarkJsonToDb(string jsonRaw)
        {
            var bookmarks = JsonConvert.DeserializeObject<List<BookmarkJson>>(jsonRaw);
            var forDbImport = bookmarks.Select(x =>
            {
                var gender = Gender.Undefined;
                if (x.Sex != null) gender = x.Sex.Contains("male") ? Gender.Male : Gender.Female;

                return new Bookmark
                {
                    FollowerCount = x.Counts.Followers,
                    FollowingCount = x.Counts.Followings,
                    ReplayCount = x.Counts.Replays,
                    Face = x.Face,
                    Gender = gender,
                    Nickname = x.Nickname,
                    Uid = x.Uid,
                    Shortid = x.Shortid,
                    Signature = x.Signature,
                    BookmarkType = BookmarkType.Follower
                };
            });
            return forDbImport;
        }

        public void ImportProfilesSeenFromFile()
        {
            var jsonRaw = File.ReadAllText(_config.OldProfileJsonFile);
            var watched = JsonConvert.DeserializeObject<List<ProfileJson>>(jsonRaw);
            var newCount = 0;
            using (var db = _dbFactory.CreateApplicationDbContext())
            {
                foreach (var w in watched)
                {
                    var found = db.ProfileSeen.Find(w.userid);
                    if (found == null)
                    {
                        var newProfileSeen = new ProfileSeen
                        {
                            Uid = w.userid,
                            Seen = long.Parse(w.dt)
                        };
                        db.Add(newProfileSeen);
                        newCount++;
                    }
                }

                db.SaveChanges();
            }

            _logger.LogInformation($"Sucessfully imported New Profiles Seen: {newCount}");
        }
    }
}