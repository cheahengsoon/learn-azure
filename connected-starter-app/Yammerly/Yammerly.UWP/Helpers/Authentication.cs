using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yammerly.Helpers;
using Yammerly.Models;
using Yammerly.Services;
using Yammerly.Views;

[assembly: Xamarin.Forms.Dependency(typeof(Yammerly.UWP.Helpers.Authentication))]
namespace Yammerly.UWP.Helpers
{
    public class Authentication : IAuthenticationService
    {
        public async Task<bool> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            return true;
        }
    }
}