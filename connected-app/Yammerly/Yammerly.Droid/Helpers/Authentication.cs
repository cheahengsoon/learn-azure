using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.WindowsAzure.MobileServices;
using Yammerly.Services;
using Plugin.CurrentActivity;

[assembly: Xamarin.Forms.Dependency(typeof(Yammerly.Droid.Helpers.Authentication))]
namespace Yammerly.Droid.Helpers
{
    public class Authentication : IAuthenticationService
    {
        public async Task<bool> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            var success = false;

            try
            {
                await client.LoginAsync(CrossCurrentActivity.Current.Activity, provider);

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