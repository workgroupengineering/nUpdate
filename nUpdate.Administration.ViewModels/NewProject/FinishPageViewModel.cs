﻿using System.Threading.Tasks;
using nUpdate.Administration.PluginBase.ViewModels;

namespace nUpdate.Administration.ViewModels.NewProject
{
    public class FinishPageViewModel : WizardPageViewModelBase
    {
        private readonly NewProjectBase _newProjectBase;
        private string _currentStatus;
        private bool _loadingIndicatorVisible = true;

        public string CurrentStatus
        {
            get => _currentStatus;
            set => SetProperty(value, ref _currentStatus);
        }

        public bool LoadingIndicatorVisible
        {
            get => _loadingIndicatorVisible;
            set => SetProperty(value, ref _loadingIndicatorVisible);
        }

        public FinishPageViewModel(NewProjectBase newProjectBase)
        {
            _newProjectBase = newProjectBase;
            NeedsUserInteraction = false;
        }

        public override async void OnNavigated(WizardPageViewModelBase fromPage, WizardViewModelBase window)
        {
            // TODO: Prevent crash on exception and add update provider test
            if (await window.Finish())
            {
                LoadingIndicatorVisible = false;
                CurrentStatus = "Project created.";
                await Task.Delay(1000);
                window.RequestGoForward();
            }
        }
    }
}
