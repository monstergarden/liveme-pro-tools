using System;
using System.Linq; 
using System.Collections.Generic;
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
            var port = GetPort(args.ToList());
            
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>()
                        .UseUrls($"http://localhost:{port}");
                });
        }

        private static int GetPort(List<string> args)
        {
            try
            {
                var idx = args.IndexOf("--port");
                var portString = args[idx+1];
                return int.Parse(portString);

            }
            catch (System.Exception)
            {
                
                return 5788;
            }
        }

        public static string GetVersion()
        {
            return "the verions";
        }
    }
}