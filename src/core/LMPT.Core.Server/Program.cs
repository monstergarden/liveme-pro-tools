using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace LMPT.Core.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>()
                        .UseUrls("http://localhost:5788");
                });
        }

        public static string GetVersion()
        {
            return "the verions";
        }
    }
}