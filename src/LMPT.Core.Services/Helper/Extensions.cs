using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LMPT.Core.Services.Http;

namespace LMPT.Core.Services.Helper
{
    public static class Extensions
    {
        public static long ToUnixTimestamp(this DateTime utcNow)
        {
            return new DateTimeOffset(utcNow).ToUnixTimeSeconds();
        }


        public static DateTime FromUnixTimestamp(this long timestamp)
        {
            var dto = DateTimeOffset.FromUnixTimeSeconds(timestamp);
            return dto.DateTime;
        }

        public static string ToFormattedDateTime(this long timestamp)
        {
            var dto = DateTimeOffset.FromUnixTimeSeconds(timestamp);
            return dto.DateTime.ToString("dd-MM-yyyy H:mm:ss");
        }


        internal static async Task<string> Fetch(this HttpClient client, IRequestBuilder request,
            CancellationToken ctsToken)
        {
            request
                .WithQueryParameter(
                    ("vercode", "38551987"),
                    ("api", "23"),
                    ("ver", "3.8.55")
                );
            var httpRequestMessage = request.Build();
            var res = await client.SendAsync(httpRequestMessage, ctsToken);

            var raw = await res.Content.ReadAsStringAsync();

            var isError = raw.Contains("token error");
            if (isError)
                Console.WriteLine("Invalid token: " + raw);

            return raw;
        }
    }
}