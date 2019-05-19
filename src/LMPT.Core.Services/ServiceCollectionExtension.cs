using System;
using System.IO;
using LMPT.Core.Services.Config;
using LMPT.Core.Services.Data;
using LMPT.Core.Services.Http;
using LMPT.Core.Services.LivemeApi;
using LMPT.Core.Services.LivemeApi.Auth;
using LMPT.Core.Services.Model;
using LMPT.DB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LMPT.Core.Services
{
    public static class ServiceCollectionExtension
    {
        public static void LogStatus<T>(this ILogger<T> logger, string message)
        {
            logger.LogInformation(LogEventIds.ForFooter, message);
        }


        public static IHttpClientBuilder AddBackgroundHttpClient<TClient>(
            this IServiceCollection services)
            where TClient : class
        {
            return services.AddHttpClient<TClient>()
                .AddHttpMessageHandler(s =>
                {
                    var foregroundThrottler = s.GetService<ThrottlerLocator>().Background;
                    var logger = s.GetService<ILogger<HttpRequestHandler>>();
                    return new HttpRequestHandler(logger, foregroundThrottler);
                });
        }

        public static IHttpClientBuilder AddForegroundHttpClient<TClient>(
            this IServiceCollection services)
            where TClient : class
        {
            return services.AddHttpClient<TClient>()
                .AddHttpMessageHandler(s =>
                {
                    var backgroundThrottler = s.GetService<ThrottlerLocator>().Foreground;
                    var logger = s.GetService<ILogger<HttpRequestHandler>>();
                    return new HttpRequestHandler(logger, backgroundThrottler);
                });
        }

        public static IServiceCollection AddLmptCoreService(this IServiceCollection serviceCollection,
            DirectoryInfo appDataFolder = null)
        {
            try
            {
                serviceCollection
                    .AddSingleton<ConfigurationProvider>()
                    .AddSingleton(s =>
                    {
                        if (appDataFolder != null) return CoreSettings.GetCoreSettings(appDataFolder);

                        var root = s.GetService<ConfigurationProvider>().RootDirectory;
                        return CoreSettings.GetCoreSettings(root);
                    })
                    .AddSingleton<HttpCache>()

                    .AddScoped<Importer>()
                    .AddTransient<ThrottlerLocator>()
                    .AddTransient<DataAccess>()
                    .AddTransient<BookmarkScanner>()
                    .AddSingleton<DbContextFactory>();
               


                serviceCollection.AddForegroundHttpClient<LiveMeAuthenticator>();
                serviceCollection.AddForegroundHttpClient<LivemeApiProvider>();
                serviceCollection.AddBackgroundHttpClient<LivemeApiProviderBackground>();
                return serviceCollection;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to set up dependencies " + e);
                throw;
            }
        }
    }
}