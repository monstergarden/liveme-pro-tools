using System;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LMPT.Core.Server.ViewModels
{
    public interface ISideBarViewModel
    {
        string SvgIconPath { get; }
        string Title { get;  }
        void Open();
        
    }

    public class SidebarViewModelFactory
    {
        private readonly ILogger<SidebarViewModelFactory> logger;
        private readonly IServiceProvider provider;

        public SidebarViewModelFactory(ILogger<SidebarViewModelFactory> logger, IServiceProvider provider)
        {
            this.logger = logger;
            this.provider = provider;
            
        }

        public ISideBarViewModel CreateBookmarksViewModel()
        {
            var vm = provider.GetService<BookmarksPageViewModel>();
            return vm;
        }

        public FansOrFollowingsViewModel CreateFansOrFollowingsViewModel()
        {
            var vm = provider.GetService<FansOrFollowingsViewModel>();
            return vm;
        }

        public ListWindowViewModel CreateListWindowViewModel()
        {
            var vm = provider.GetService<ListWindowViewModel>();
            return vm;
        }

        public ISideBarViewModel CreateBookmarksFeedViewModel()
        {
            try
            {
                var vm = provider.GetService<BookmarkFeedViewModel>();
                return vm;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                logger.LogError(ex, "failed to create viemodel");
                throw;
            }
     
        }
    }
}