using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using RunProcessAsTask;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LMPT.Core.Services.Helper
{
    public class Utils
    {
        public static async Task<string> ExtractFirstFrameAsync(ILogger logger, string source)
        {
            try
            {
                logger.LogInformation(source);
                await Task.Delay(1).ConfigureAwait(false);

                var tempoutJpg = Guid.NewGuid() + "thumb.jpg";


                var cmd = $" -i {source} -vframes 1 {tempoutJpg}";
                var res = await ProcessEx.RunAsync("ffmpeg", cmd);


                var bytes = await File.ReadAllBytesAsync(tempoutJpg);
                var base64 = Convert.ToBase64String(bytes);
                File.Delete(tempoutJpg);

                return base64;
            }
            catch (System.Exception)
            {
                return "";
            }

        }
        public static void ExtractFramesPerMinute(int n, string tsSource)
        {
            var exe = "ffmpeg";
            var outputJpg = Guid.NewGuid() + "thumb.jpg";
            var freq = (1 / 60) * n;


            var cmd = $"{exe} -i {tsSource}  -vf fps={freq} thumb%04d.jpg -hide_banner";
        }
        public static void OpenBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}"));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
        }
    }
}