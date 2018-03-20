﻿using System.Collections.Generic;
using MyCompanyInThePocket.Core.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Threading;

namespace MyCompanyInThePocket.Core.ViewModels.Settings
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IMeetingService _meetingService;
        private readonly IAuthentificationService _authenService;
        private readonly IBackgroundTaskService _backgroundTaskService;

        public SuspendableObservableCollection<GroupedSettingsViewModel> SettingsList { get; private set; }

        public SettingsViewModel()
        {

            _meetingService = App.Instance.GetInstance<IMeetingService>();
            _authenService = App.Instance.GetInstance<IAuthentificationService>();
            _backgroundTaskService = App.Instance.BackgroundTaskService;

            //TODO : localisation
            SettingsList = new SuspendableObservableCollection<GroupedSettingsViewModel>();
            SettingsList.Add(new GroupedSettingsViewModel("ACRA", new List<SettingViewModel>
            {
                new ToggleSettingViewModel("Intégration dans le calendrier", new RelayCommand<bool>(ToggleIntegrationCalendar), ApplicationSettings.IsIntegrationToCalendarEnabled),
                new ToggleSettingViewModel("Intégration dans les rappels", new RelayCommand<bool>(ToggleIsIntegrationReminder), ApplicationSettings.IsIntegrationToReminderEnabled),
                new ToggleSettingViewModel("Mise à jour en arrière-plan", new RelayCommand<bool>(ToggleBackgroundUpdate), ApplicationSettings.IsBackgroundUpdateEnabled),
            }));
            //TODO : localisation
            SettingsList.Add(new GroupedSettingsViewModel(OnlineSettings.Identity, new List<SettingViewModel>
            {
                new ButtonSettingViewModel("Déconnexion", new RelayCommand(LogOutAction))
                {
                    ColorStyle = ColorStyle.Danger,
                    HorizontalAlignment = SettingsHorizontalAlignment.Center
                }
            }));
        }

        public bool IsIntegrationToCalendarEnabled
        {
            get
            {
                return ApplicationSettings.IsIntegrationToCalendarEnabled;
            }
            set
            {
                if (value != ApplicationSettings.IsIntegrationToCalendarEnabled)
                {
                    ApplicationSettings.IsIntegrationToCalendarEnabled = value;
                    _meetingService.GetMeetingsAsync(forceRefresh: true, cancellationToken: CancellationToken.None);
                    RaisePropertyChanged(nameof(IsIntegrationToCalendarEnabled));
                }
            }
        }

        public bool IsIntegrationToReminderEnabled
        {
            get
            {
                return ApplicationSettings.IsIntegrationToReminderEnabled;
            }
            set
            {
                if (value != ApplicationSettings.IsIntegrationToCalendarEnabled)
                {
                    ApplicationSettings.IsIntegrationToCalendarEnabled = value;
                    _meetingService.GetMeetingsAsync(forceRefresh: true, cancellationToken: CancellationToken.None);
                    RaisePropertyChanged(nameof(IsIntegrationToReminderEnabled));
                }
            }
        }

        public int DelayBetweenTwoBackgroundUpdate
        {
            get
            {
                return ApplicationSettings.DelayBetweenTwoBackgroundUpdate;
            }
            set
            {
                if (value != ApplicationSettings.DelayBetweenTwoBackgroundUpdate)
                {
                    ApplicationSettings.DelayBetweenTwoBackgroundUpdate = value;

                    if (IsBackgroundUpdateEnabled)
                    {
                        _backgroundTaskService.Register(ApplicationSettings.DelayBetweenTwoBackgroundUpdate);
                    }

                    RaisePropertyChanged(nameof(DelayBetweenTwoBackgroundUpdate));
                }
            }
        }

        public bool IsBackgroundUpdateEnabled
        {
            get
            {
                return ApplicationSettings.IsBackgroundUpdateEnabled;
            }
            set
            {
                if (value != ApplicationSettings.IsBackgroundUpdateEnabled)
                {
                    ApplicationSettings.IsBackgroundUpdateEnabled = value;

                    if (value)
                    {
                        _backgroundTaskService.Register(ApplicationSettings.DelayBetweenTwoBackgroundUpdate);
                    }
                    else
                    {
                        _backgroundTaskService.UnRegister();
                    }
                    RaisePropertyChanged(nameof(IsBackgroundUpdateEnabled));
                }
            }
        }

        private async void LogOutAction()
        {
            //TODO : localisation
            var result = await App.Instance.AlertService.ShowDestructiveAlertAsync(null, "Souhaitez-vous vraiment vous déconnecter ?", "Déconnexion", "Annuler");

            if (result)
            {
                _backgroundTaskService.UnRegister();
                _authenService.Disconnect();
                ApplicationSettings.Clear();
                this.ShowViewModel<StartupViewModel>();
            }
        }

        private void ToggleBackgroundUpdate(bool isOn)
        {
            IsBackgroundUpdateEnabled = isOn;
        }

        private void ToggleIntegrationCalendar(bool isOn)
        {
            IsIntegrationToCalendarEnabled = isOn;
        }

        private void ToggleIsIntegrationReminder(bool isOn)
        {
            IsIntegrationToReminderEnabled = isOn;
        }
    }
}
