using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LMPT.Core.Contract.DB;
using LMPT.Core.Services.Config;
using LMPT.Core.Services.Data;
using LMPT.Core.Services.Helper;
using LMPT.Core.Services.Http;
using Newtonsoft.Json;

namespace LMPT.Core.Services.LivemeApi.Auth
{
    public class LiveMeAuthenticator
    {
        private readonly HttpClient _client;
        private readonly DataAccess _dataAccess;
        private readonly CoreSettings _settings;

        public LiveMeAuthenticator(HttpClient client, CoreSettings settings, DataAccess dataAccess)
        {
            _client = client;
            _client.DefaultRequestHeaders.Add("User-Agent", "FBAndroidSDK.0.0.1");
            _settings = settings;
            _dataAccess = dataAccess;
        }

        public int Thirdchannel { get; set; } = 6;
        public Guid Androidid { get; set; } = Guid.NewGuid();

        public async Task Login()
        {
            var foundInDb = _dataAccess.FindAuthToken();

            if (foundInDb != null)
            {
                double secondsSinceLastLogin = DateTime.UtcNow.ToUnixTimestamp() - foundInDb.LoginTimestamp;
                if (secondsSinceLastLogin < TimeSpan.FromMinutes(30).TotalSeconds) return;
            }

            if (_settings.Email == null || _settings.Password == null)
                throw new ArgumentException("Either email or password was not provided");

            var request = AuthRequest(_settings.Email, _settings.Password);
            var res = await _client.Fetch(request, CancellationToken.None);

            dynamic json = JsonConvert.DeserializeObject(res);
            var authentication = new LivemeAuthentication
            {
                Sid = json.data.sid,
                SsoToken = json.data.sso_token
            };


            var loginRequest = HttpRequestBuilder
                .Post(LiveMeUrls.AppLogin)
                .WithFormData(
                    new
                    {
                        sso_token = authentication.SsoToken
                    });

            var loginResponse = await _client.Fetch(loginRequest, CancellationToken.None);


            dynamic loginData = JsonConvert.DeserializeObject(loginResponse);
            authentication.Tuid = loginData.data.user.user_info.uid;
            authentication.Token = loginData.data.token;
            authentication.LoginTimestamp = DateTime.UtcNow.ToUnixTimestamp();

            _dataAccess.StoreAuthToken(authentication);
        }


        private HttpRequestBuilder AuthRequest(string email, string password)
        {
            return HttpRequestBuilder
                .Post(LiveMeUrls.Login)
                .WithBody(
                    $"--3i2ndDfv2rTHiSisAbouNdArYfORhtTPEefj3q2f\r\nContent-Disposition: form-data; name=\"cmversion\"\r\n\r\n38551987\r\n--3i2ndDfv2rTHiSisAbouNdArYfORhtTPEefj3q2f\r\nContent-Disposition: form-data; name=\"code\"\r\n\r\n\r\n--3i2ndDfv2rTHiSisAbouNdArYfORhtTPEefj3q2f\r\nContent-Disposition: form-data; name=\"name\"\r\n\r\n{email}\r\n--3i2ndDfv2rTHiSisAbouNdArYfORhtTPEefj3q2f\r\nContent-Disposition: form-data; name=\"extra\"\r\n\r\nuserinfo\r\n--3i2ndDfv2rTHiSisAbouNdArYfORhtTPEefj3q2f\r\nContent-Disposition: form-data; name=\"password\"\r\n\r\n{password}\r\n--3i2ndDfv2rTHiSisAbouNdArYfORhtTPEefj3q2f")
                .ReplaceContentHeaders(new Dictionary<string, string>
                {
                    {"sig", "fp1bO-aJwHKoRB0jnsW4hQ6nor8"},
                    {"sid", "9469C0239535A9E579F8D20E5A4D5C3C"},
                    {"appid", "135301"},
                    {"ver", "3.8.55"},
                    {"content-type", "multipart/form-data; boundary=3i2ndDfv2rTHiSisAbouNdArYfORhtTPEefj3q2f"}
                });
        }
    }
}