using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Yammerly.Services;

using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using System.Threading.Tasks;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(Yammerly.iOS.Helpers.Authentication))]
namespace Yammerly.iOS.Helpers
{
    public class Authentication : IAuthenticationService
    {
        public async Task<bool> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            var success = false;

            try
            {
                await client.LoginAsync(UIApplication.SharedApplication.KeyWindow.RootViewController, provider);

                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging in: {ex.Message}");
            }

            return success;
        }
    }
}