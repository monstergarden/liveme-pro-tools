using System;
using System.Collections.Generic;
using LMPT.Core.Contract.DB.Cache;
using LMPT.Core.Contract.FromLiveMe;
using LMPT.Core.Services.Helper;
using LMPT.DB;
using Microsoft.Extensions.Logging;

namespace LMPT.Core.Services.Data
{
    public class HttpCache
    {
        private readonly DbContextFactory _dbContextFactory;
        private readonly ILogger<HttpCache> _logger;

        public HttpCache(ILogger<HttpCache> logger, DbContextFactory dbContextFactory)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
        }


        private void StoreUserReplays(string uid, ReplayFromLiveme videoInfo, ApplicationDBContext db)
        {
            var replay = db.Replays.Find(videoInfo.Vid);

            if (replay != null) db.Remove(replay);

            var user = db.StoreUser(uid, videoInfo.Uname);
            replay = new Replay
            {
                VId = videoInfo.Vid,
                FromUser = user,
                Title = videoInfo.Title,
                CreatedAt = replay?.CreatedAt ?? DateTime.UtcNow.ToUnixTimestamp(),
                VideoUrl = videoInfo.Hlsvideosource.ToString(),
                StartTimeStamp = videoInfo.Vtime,
                ShareNum = videoInfo.Sharenum,
                Watched = replay?.Watched ?? false,
                Downloaded = replay?.Downloaded ?? false,
                Duration = TimeSpan.FromSeconds(videoInfo.Videolength),
                Liked = replay?.Liked ?? false
            };

            db.Replays.Add(replay);
        }

        public void StoreUserReplays(string uid, IEnumerable<ReplayFromLiveme> replays)
        {
            using (var db = _dbContextFactory.CreateApplicationDbContext())
            {
                foreach (var item in replays)
                    try
                    {
                        StoreUserReplays(uid, item, db);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.ToString());
                    }

                db.SaveChanges();
            }
        }
    }
}