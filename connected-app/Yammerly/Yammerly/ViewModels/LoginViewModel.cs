using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yammerly.Helpers;
using Yammerly.Views;

using Xamarin.Forms;
using Yammerly.Services;

namespace Yammerly.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        Command loginCommand;
        public Command LoginCommand
        {
            get { return loginCommand ?? (loginCommand = new Command(async () => await ExecuteLoginCommandAsync())); }
        }

        async Task ExecuteLoginCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                bool authenticated = false;

                var client = DependencyService.Get<IDataService>() as AzureService;
                
                authenticated = await DependencyService.Get<IAuthenticationService>().LoginAsync(client.MobileService, Microsoft.WindowsAzure.MobileServices.MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);

                if (authenticated)
                {
                    if (Device.OS != TargetPlatform.Android)
                        App.Current.MainPage = new RootPage();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
