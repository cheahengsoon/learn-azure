using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Yammerly.Helpers;
using Yammerly.Services;

using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using System.Threading.Tasks;
using UIKit;
using Yammerly.Models;
using Yammerly.Views;

[assembly: Xamarin.Forms.Dependency(typeof(Yammerly.iOS.Helpers.Authentication))]
namespace Yammerly.iOS.Helpers
{
    public class Authentication : IAuthenticationService
    {
        public async Task<bool> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            return true;
        }
    }
}