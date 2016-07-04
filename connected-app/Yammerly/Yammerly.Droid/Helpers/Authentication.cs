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
using Yammerly.Helpers;
using Plugin.CurrentActivity;
using Yammerly.Models;
using Yammerly.Views;

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
                var user = await client.LoginAsync(Xamarin.Forms.Forms.Context, provider);

                if (user != null)
                {
                    Settings.AuthToken = user.MobileServiceAuthenticationToken;
                    Settings.UserId = user.UserId;
                    
                    var employee = await client.InvokeApiAsync<Employee>("UserInfo", System.Net.Http.HttpMethod.Get, null);
                    Settings.FirstName = employee.FirstName;
                    Settings.LastName = employee.LastName;
                    Settings.PhotoUrl = employee.PhotoUrl;

                    Xamarin.Forms.Application.Current.MainPage = new RootPage();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging in: {ex.Message}");
            }

            return success;
        }
    }
}