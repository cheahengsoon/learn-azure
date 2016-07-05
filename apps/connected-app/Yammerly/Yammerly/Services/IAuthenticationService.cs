using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.MobileServices;

namespace Yammerly.Services
{
    public interface IAuthenticationService
    {
        Task<bool> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider);
    }
}
