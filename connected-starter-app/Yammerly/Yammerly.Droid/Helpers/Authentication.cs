using System;
using System.Threading.Tasks;

using Yammerly.Helpers;
using Yammerly.Models;
using Yammerly.Services;
using Yammerly.Views;

using Microsoft.WindowsAzure.MobileServices;

[assembly: Xamarin.Forms.Dependency(typeof(Yammerly.Droid.Helpers.Authentication))]
namespace Yammerly.Droid.Helpers
{
    public class Authentication : IAuthenticationService
    {
        public async Task<bool> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            return true;
        }
    }
}