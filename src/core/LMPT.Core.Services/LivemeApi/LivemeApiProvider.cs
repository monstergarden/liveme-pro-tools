using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LMPT.Core.Contract.FromLiveMe;
using LMPT.Core.Services.Data;
using LMPT.Core.Services.Helper;
using LMPT.Core.Services.Http;
using LMPT.Core.Services.LivemeApi.Auth;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LMPT.Core.Services.LivemeApi
{
    public class LivemeApiProvider
    {
        private static readonly Guid NewGuid = Guid.NewGuid();
        private readonly LiveMeAuthenticator _auth;

        private readonly HttpCache _cache;
        private readonly HttpClient _client;
        private readonly DataAccess _dataAccess;
        private readonly ILogger<LivemeApiProvider> _logger;


        public LivemeApiProvider(ILogger<LivemeApiProvider> logger, HttpClient client,
            HttpCache cache, DataAccess dataAccess, LiveMeAuthenticator auth)
        {
            _logger = logger;
            _client = client;
            _client.DefaultRequestHeaders.Add("User-Agent", "FBAndroidSDK.0.0.1");
            _client.DefaultRequestHeaders.Add("d", "1");
            _cache = cache;
            _dataAccess = dataAccess;
            _auth = auth;
        }


        public async Task<LiveMeUser> GetUserInfo(string uid, CancellationToken ctsToken)
        {
            _logger.LogInformation($"Get UserInfo: {uid}");
            var request = HttpRequestBuilder
                .Post(LiveMeUrls.UserInfo)
                .WithFormData(new
                {
                    userid = uid
                });


            var raw = await _client.Fetch(request, ctsToken);
            dynamic json = JsonConvert.DeserializeObject(raw);
            var userInfo = json.data.user;
            return JsonConvert.DeserializeObject<LiveMeUser>(userInfo.ToString());
        }

        public async Task<ReplayFromLiveme> GetVideoInfo(string vid, CancellationToken token)
        {
            await _auth.Login();

            var authentications = _dataAccess.FindAuthToken();

            var request = HttpRequestBuilder
                .Post(LiveMeUrls.VideoInfo)
                .WithFormData(new
                {
                    videoid = vid,
                    userid = 0,
                    tuid = authentications.Tuid,
                    token = authentications.Token
                });


            var raw = await _client.Fetch(request, token);
            dynamic json = JsonConvert.DeserializeObject(raw);
            var videoInfo = json.data.video_info;
            return JsonConvert.DeserializeObject<ReplayFromLiveme>(videoInfo.ToString());
        }

        public async Task<List<UserInfo>> GetFollowing(string uid, CancellationToken token, int pageIndex = 1,
            int pageSize = 10)
        {
            _logger.LogInformation($"Get GetFollowing: {uid}, {pageIndex}, {pageSize}");
            var request = HttpRequestBuilder
                .Post(LiveMeUrls.Following)
                .WithFormData(new
                {
                    access_token = uid, // strange but that's the way it is.
                    page_index = pageIndex,
                    page_size = pageSize
                });

            var raw = await _client.Fetch(request, token);
            dynamic json = JsonConvert.DeserializeObject(raw);
            var users = json.data;
            var res = JsonConvert.DeserializeObject<List<UserInfo>>(users.ToString());
            return res;
        }


        public async Task<List<UserInfo>> GetFans(string uid, CancellationToken token, int pageIndex = 1,
            int pageSize = 10)
        {
            _logger.LogInformation($"Get Fans: {uid}, {pageIndex}, {pageSize}");
            var request = HttpRequestBuilder
                .Post(LiveMeUrls.Fans)
                .WithFormData(new
                {
                    access_token = uid, // strange but that's the way it is.
                    page_index = pageIndex,
                    page_size = pageSize
                });

            var raw = await _client.Fetch(request, token);
            dynamic json = JsonConvert.DeserializeObject(raw);
            var users = json.data;
            return JsonConvert.DeserializeObject<List<UserInfo>>(users.ToString());
        }

        public async Task<List<ReplayFromLiveme>> GetUsersReplays(string uid, CancellationToken token,
            int pageIndex = 1, int pageSize = 10)
        {
            _logger.LogStatus(
                $"Fetching User Replays for {uid}. Page {pageIndex} (page size: {pageIndex + pageSize}).");

            await _auth.Login();

            var authentications = _dataAccess.FindAuthToken();

            var request = HttpRequestBuilder
                .Post(LiveMeUrls.ReplayVideos)
                .WithFormData(new
                {
                    userid = uid,
                    page_index = pageIndex,
                    page_size = pageSize,
                    androidid = NewGuid,
                    tuid = authentications.Tuid,
                    token = authentications.Token,
                    sso_token = authentications.SsoToken
                });


            var res = await _client.Fetch(request, token);
            try
            {
                dynamic json = JsonConvert.DeserializeObject(res);
                var vidinfo = json.data.video_info;
                var replays = JsonConvert.DeserializeObject<List<ReplayFromLiveme>>(vidinfo.ToString());
                _logger.LogStatus($"Found {replays.Count} Replays for {uid}.");

                _cache.StoreUserReplays(uid, replays);

                return replays;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get user replays. Response was: {res}. Exception: {ex.Message}");
                return new List<ReplayFromLiveme>();
            }
        }
    }
}