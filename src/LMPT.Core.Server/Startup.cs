using System;
using System.IO;
using System.Reflection;
using LMPT.Core.Server.Utils;
using LMPT.Core.Server.ViewModels;
using LMPT.Core.Services;
using LMPT.Core.Server.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LMPT.Core.Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                builder.AddProvider<LmptLoggingProvider>();
            });


            services.AddLmptCoreService();
            services.AddScoped<JsInteropHelper>()
                .AddScoped<SidebarViewModelFactory>()
                .AddScoped<FooterViewModel>()
                .AddScoped<BookmarksPageViewModel>()
                .AddScoped<DownloadsViewModel>()
                .AddScoped<BookmarkFeedViewModel>()
                .AddScoped<ListWindowViewModel>()
                .AddScoped<MainViewModel>()
                .AddScoped<ProfileViewModel>()
                .AddScoped<SideBarHelper>()
                .AddSingleton<ViewModelMediator>()
                .AddSingleton<Analytics>()
                .AddTransient<FansOrFollowingsViewModel>();

            
            
            services.AddBlazorContextMenu(options =>
            {
                //Configures the default template
                options.ConfigureTemplate(template =>
                {
                    template.MenuCssClass = "dark-menu";
                    template.MenuItemCssClass = "dark-menu-item";
                    template.MenuItemDisabledCssClass = "dark-menu-item--disabled";
                    template.MenuItemWithSubMenuCssClass = "dark-menu-item--with-submenu";
                });
            });


            services.AddRazorPages();
            services.AddServerSideBlazor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Use component registrations and static files from the app project.
            var binFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Console.WriteLine(binFolder);

            //app.ApplicationServices.GetService<CoreApp>().StartUp().Wait();
            // app.UseStaticFiles(new StaticFileOptions()
            // {
            //     FileProvider = new PhysicalFileProvider(Path.Combine(binFolder, @"../../../../../electron/app")),
            //     RequestPath = new PathString("")
            // });
            app.UseStaticFiles();
            app.UseDeveloperExceptionPage();

            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub().AddComponent<App>("app");
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}