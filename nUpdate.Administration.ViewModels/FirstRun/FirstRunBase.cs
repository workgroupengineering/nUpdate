﻿using System.Collections.Generic;
using System.Threading.Tasks;
using nUpdate.Administration.Models;
using nUpdate.Administration.PluginBase.ViewModels;

namespace nUpdate.Administration.ViewModels.FirstRun
{
    public class FirstRunBase : WizardViewModelBase
    {
        private readonly IFirstRunProvider _firstRunProvider;
        public FirstSetupData FirstSetupData { get; } = new FirstSetupData();

        public FirstRunBase(IFirstRunProvider firstRunProvider)
        {
            _firstRunProvider = firstRunProvider;
            _firstRunProvider.SetFinishAction(out var f);
            FinishingAction = f;

            InitializePages(new List<WizardPageViewModelBase>
            {
                new WelcomePageViewModel(),
                new KeyDatabaseSetupPageViewModel(this),
                new PathSetupPageViewModel(this, _firstRunProvider)
            });
        }

        public override Task<bool> Finish()
        {
            return Task.Run(() =>
            {
                if (!_firstRunProvider.Finish(FirstSetupData))
                    return false;

                FinishingAction?.Invoke();
                return true;
            });
        }
    }
}
