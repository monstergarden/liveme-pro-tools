using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LMPT.Core.Contract.DB.Cache;
using LMPT.Core.Contract.FromLiveMe;
using LMPT.Core.Server.Shared;
using LMPT.Core.Services.Data;
using LMPT.Core.Services.Helper;
using LMPT.Core.Services.LivemeApi;
using Microsoft.Extensions.Logging;

namespace LMPT.Core.Server.ViewModels
{
    public class ListWindowViewModel : BaseViewModel, ISideBarViewModel
    {

        public string SvgIconPath => Icons.Files;
        public string Title => "List Window";

        public List<FansOrFollowingsViewModel> ViewModels = new List<FansOrFollowingsViewModel>();
        public FansOrFollowingsViewModel? Current;
        private readonly MainViewModel mainViewModel;
        private readonly SidebarViewModelFactory _factory;

        public ListWindowViewModel(
            SidebarViewModelFactory factory,
            MainViewModel mvm)
        {
            mainViewModel = mvm;
            _factory = factory;
        }

        public string PageInfo()
        {
            if(Current == null) return string.Empty;
            var idx = ViewModels.IndexOf(Current);
            return $"{idx + 1} / {ViewModels.Count}";
        }

        public void Previous()
        {
            if(Current == null) return;

            var idx = ViewModels.IndexOf(Current);
            if (idx > 0)
            {
                idx--;
                Current = ViewModels[idx];
            }

        }
        public void Next()
        {
            if(Current == null) return;
            var idx = ViewModels.IndexOf(Current);
            if (idx < ViewModels.Count - 1)
            {
                idx++;
                Current = ViewModels[idx];
            }
        }

        public void Open()
        {
            mainViewModel.OpenListWindow();
        }

        public void CloseCurrent()
        {
            if(Current == null) return;
            var currentIdx = ViewModels.IndexOf(Current);

            // Set Current to one that makes sense
            if (ViewModels.Count == 1)
            {
                Current = null;
            }
            else
            {
                if (currentIdx == 0)
                {
                    Next();
                }
                else
                {
                    Previous();
                }
            }

            ViewModels.RemoveAt(currentIdx);



        }


        internal void ShowFollowingsOf(string uid)
        {
            var vm = _factory.CreateFansOrFollowingsViewModel();
            vm.UserId = uid;
            vm.PageType = ListWindowPageType.Followings;
            Current = vm;
            ViewModels.Add(vm);
            NofifyChanged();

        }

        internal void ShowFansOf(string uid)
        {
            var vm = _factory.CreateFansOrFollowingsViewModel();
            vm.UserId = uid;
            vm.PageType = ListWindowPageType.Fans;
            Current = vm;
            ViewModels.Add(vm);
            NofifyChanged();
        }


    }
}