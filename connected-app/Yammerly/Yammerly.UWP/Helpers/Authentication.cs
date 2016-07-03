using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammerly.Services;

[assembly: Xamarin.Forms.Dependency(typeof(Yammerly.UWP.Helpers.Authentication))]
namespace Yammerly.UWP.Helpers
{
    public class Authentication : IAuthenticationService
    {
        public async Task<bool> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            var success = false;

            try
            {
                await client.LoginAsync(provider);

                success = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error logging in: {ex.Message}");
            }

            return success;
        }
    }
}