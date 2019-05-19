using System.Net.Http;
using LMPT.Core.Services.Data;
using LMPT.Core.Services.LivemeApi.Auth;
using Microsoft.Extensions.Logging;

namespace LMPT.Core.Services.LivemeApi
{
    public class LivemeApiProviderBackground : LivemeApiProvider
    {
        public LivemeApiProviderBackground(ILogger<LivemeApiProviderBackground> logger, HttpClient client,
            HttpCache cache, DataAccess applicationDbContext, LiveMeAuthenticator auth) : base(logger, client, cache,
            applicationDbContext, auth)
        {
        }
    }
}