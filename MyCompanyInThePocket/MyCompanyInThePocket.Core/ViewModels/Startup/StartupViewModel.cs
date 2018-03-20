using MyCompanyInThePocket.Core.Services;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace MyCompanyInThePocket.Core.ViewModels
{
    public class StartupViewModel : BaseViewModel
    {
        private readonly IAuthentificationService _authenService;

        public StartupViewModel()
        {
            _authenService = App.Instance.GetInstance<IAuthentificationService>();
            GoToNextPageCommand = new RelayCommand(GoToNextPage);
        }

        public ICommand GoToNextPageCommand
        {
            get; private set;
        }

        private async void GoToNextPage()
        {
            try
            {
                IsBusy = true;
                await _authenService.AuthenticateAsync();

                if (ApplicationSettings.IsBackgroundUpdateEnabled)
                {
                    App.Instance.BackgroundTaskService.Register(ApplicationSettings.DelayBetweenTwoBackgroundUpdate);
                }

<<<<<<< HEAD
                ApplicationSettings.LaunchStartupScreen = false;
                ShowViewModel<MainViewModel>();
            }
            catch (System.Exception)
            {
                //TODO : display error message
            }
            finally
            {
=======
				ApplicationSettings.LaunchStartupScreen = false;
				ShowViewModel<MainViewModel>();
			}
			catch (System.Exception ex)
			{
                await App.Instance.AlertService.ShowExceptionMessageAsync(ex, "Erreur de lors de connexion");
            }
			finally
			{
>>>>>>> e7eccaed4a14a3700fbe48d0bb4d0d5ebf664307
                IsBusy = false;
            }
        }
    }
}
