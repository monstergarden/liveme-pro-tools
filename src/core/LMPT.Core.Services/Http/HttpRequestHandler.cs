using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LMPT.Core.Services.Http
{
    public class HttpRequestHandler : DelegatingHandler
    {
        private readonly ILogger<HttpRequestHandler> _logger;
        private readonly Throttler _throttler;


        public HttpRequestHandler(ILogger<HttpRequestHandler> logger, Throttler throttler)
        {
            _logger = logger;
            _throttler = throttler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var rawurl = request.RequestUri.OriginalString;

            var res = await _throttler.WithLimitedCalls(
                async () =>
                {
                    var response = await base.SendAsync(request, cancellationToken);

                    var raw = await response.Content.ReadAsStringAsync();
                    // var json = JsonConvert.DeserializeObject(raw);
                    // var content = JsonConvert.SerializeObject(json, Formatting.Indented);


                    _logger.LogDebug(
                        $"Got Http Response for {request.RequestUri}. Status: {response.StatusCode}. Content: {raw}");
                    return response;
                }).ConfigureAwait(true);
            return res;
        }
    }
}