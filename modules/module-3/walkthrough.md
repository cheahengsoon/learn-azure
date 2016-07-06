# Module 3: Add identity with Azure Active Directory.

### Prerequisites
* Download the [starter code](http://www.google.com) for this module.

### Instructions
#### Azure Active Directory
**Objective**: Add identity with Azure Active Directory. Require authentication to access backend endpoints, and present a login flow to the user when they launch the app.

1. Open up the Azure Portal. Open up the resource group you created in Module 1. Click `Add`, search for `Active Directory`, and click `Create`. You will be redirected to the classic Azure portal.
 
 ![](/modules/module-3/images/new_azure_ad.png)

2. Create a new directory by giving it a name, domain, and country. Logins will become something similar to `test@{domainname}.onmicrosoft.com`. Click the checkbox.

 ![](/modules/module-3/images/add_directory.png)

3. Click on your Azure Active Directory, and navigate to the `Users` tab. Click `Add User` and work through the walkthrough for creating a new user. Be sure to select `New user in your organization`. A temporary password will be generated, along with an email. Save these, as you will need them later for testing.
 
 ![](/modules/module-3/images/add_user_info.png)

4. Now that we have an Active Directory, it's time to register an application with AD. Navigate to the `Applications` tab, and click `Add`.

5. Select `Add an application that my organization is developing`.

 ![](/modules/module-3/images/new_azure_ad_app.png)

6. Enter a name, and be sure to click `Web application and/or Web API`. 

 ![](/modules/module-3/images/create_new_azure_ad_application.png)

7. For `Sign-on Url` and `App Id Uri`, enter your Azure Mobile Apps url (ex: `https://azuretraining.azurewebsites.net`).

8. Click the `Configure` tab and add a `Reply Url` of `https://{YOUR-AZURE-MOBILE-APP}.azurewebsites.net/.auth/login/aad/callback`. Click `Save`.

9. Click `View Endpoints` and copy the `Federation Metadata Document`.
 
 ![](/modules/module-3/images/federation_metadata_document.png)

10. Navigate to that url and copy the `entityId`.

 ![](/modules/module-3/images/copy_entity_id.png)

11. Finally, visit the `Configure` tab of your Azure AD Application and copy the `Client Id`.

 ![](/modules/module-3/images/copy_client_id.png)

Woot! Now that our Azure AD application is configured, it's time update our backend to utilize Azure AD authentication.

#### Backend

1. Navigate the `Settings` for your Azure Mobile App in the Azure Portal, and click on  `Authentication / Authorization`. Change `App Service Authentication` to `ON`. Change `Action to take when request is not authenticated` to `Allow request (no action)`. Finally, turn the `Token Store` setting to `ON` and click the `Azure Active Directory` row.

 ![](/modules/module-3/images/authorization_authentication_blade.png)

2. Change the `Management mode` to `Advanced` and paste in the `Client ID`. For `Issuer Url`, enter the `entityId` value we copied earlier.

 ![](/modules/module-3/images/azure_ad_settings.png)

3. Click `OK` on the `Azure Active Directory Settings` blade and then `Save` on the `Authentication / Authorization` blade.

4. Navigate to `https://{YOUR-AZURE-MOBILE-APP}.azurewebsites.net/.auth/login/aad` in a web browser (running Incognito or Private mode). Log in with the account provisioned earlier. You will be prompted to create a new password, and then you should be redirected to a successful login screen (pending correct configuration).

 ![](/modules/module-3/images/success_login.png)

5. To authorize individual endpoints, all we have to do is add an `AuthorizeAttribute` to the `TableController`s we wish to restrict access to. Repeat for `EmployeesController` and `TimelineItemsController`.

 ```csharp
    [Authorize]
    public class EmployeeController : TableController<Employee>
    {
        ...
    }
 ```
 
6. Republish our backend. Open up Postman and try to access an endpoint. You should receive an HTTP 401 Unauthorized response.

 ![](/modules/module-3/images/401_unauthorized.png)

7. Now that our API is secured, we need an easy way of creating accounts for users who have authenticated with Azure Active Directory. This step isn't always required, but if you wish to create separate accounts that are linked to the social accounts (as many apps often do that use social authentication), it must be done. Right-click `Controllers` and add a new `Web API 2 Controller` and name it `UserInfoController`.

8. We can use ASP.NET Web API's `ClaimsPrincipal` to extract some data from our authenticated user's claims and create a new `Employee` from that data. Once we have saved off the new account to our database, we can return the object to our mobile app to use.

 ```csharp
     [MobileAppController]
    public class UserInfoController : ApiController
    {
        [Authorize]
        public async Task<Employee> Get()
        {
            var user = User as ClaimsPrincipal;
            var isAuthenticated = user.Identity.IsAuthenticated;

            if (!isAuthenticated)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var credentials = await user.GetAppServiceIdentityAsync<AzureActiveDirectoryCredentials>(Request);

            var userId = string.Empty;
            var firstName = string.Empty;
            var lastName = string.Empty;
            var email = string.Empty;
            var photoUrl = string.Empty;

            firstName = credentials.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? string.Empty;
            lastName = credentials.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value ?? string.Empty;
            email = credentials.UserClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            userId = email;
            photoUrl = "https://secure.gravatar.com/avatar/62921d835f6d165597ff0dcd40fd2664?s=260&d=mm&r=g";

            var context = new Models.MobileServiceContext();
            var currentUser = context.Employees.FirstOrDefault(u => u.Id == userId);

            if (currentUser == null)
            {
                currentUser = new Employee
                {
                    Id = userId,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Title = "Software Engineer",
                    PhotoUrl = photoUrl
                };

                context.Employees.Add(currentUser);
            }
            else
            {
                currentUser.FirstName = firstName;
                currentUser.LastName = lastName;
                currentUser.Email = email;
            }

            await context.SaveChangesAsync();

            return currentUser;
        }
    }
 ```

9. Republish the backend! We're now ready to add authentication back to our mobile app.

#### Mobile Apps

1. Authentication is handled on a per-platform basis with the `MobileServiceClient`, which will handle the login flow for us entirely, including presenting a user interface for our user to login with. We can use Xamarin.Forms `DependencyService` to call into these APIs on a per-platform basis. `Yammerly/Service` contains an `IAuthenticationService`, which will serve as our interface. In the  `Helpers` folder for each platform, add the corresponding `Authentication` class, which will log the user in and store some information about the `Employee` from our `UserProfileController`.

 Android:

 ```csharp
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
            var success = false;

            try
            {
                var user = await client.LoginAsync(Xamarin.Forms.Forms.Context, provider);

                if (user != null)
                {
                    Settings.AuthToken = user.MobileServiceAuthenticationToken;
                    
                    var employee = await client.InvokeApiAsync<Employee>("UserInfo", System.Net.Http.HttpMethod.Get, null);
                    Settings.FirstName = employee.FirstName;
                    Settings.LastName = employee.LastName;
                    Settings.PhotoUrl = employee.PhotoUrl;
                    Settings.UserId = employee.Id;

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
 ```

 iOS:

 ```csharp
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
            var success = false;

            try
            {
                var user = await client.LoginAsync(UIApplication.SharedApplication.KeyWindow.RootViewController, provider);

                if (user != null)
                {
                    Settings.AuthToken = user.MobileServiceAuthenticationToken;

                    var employee = await client.InvokeApiAsync<Employee>("UserInfo", System.Net.Http.HttpMethod.Get, null);
                    Settings.FirstName = employee.FirstName;
                    Settings.LastName = employee.LastName;
                    Settings.PhotoUrl = employee.PhotoUrl;
                    Settings.UserId = employee.Id;

                    App.Current.MainPage = new RootPage();

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
 ```

 UWP:

 ```csharp
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
            var success = false;

            try
            {
                var user = await client.LoginAsync(provider);

                if (user != null)
                {
                    Settings.AuthToken = user.MobileServiceAuthenticationToken;
                    
                    var employee = await client.InvokeApiAsync<Employee>("UserInfo", System.Net.Http.HttpMethod.Get, null);
                    Settings.FirstName = employee.FirstName;
                    Settings.LastName = employee.LastName;
                    Settings.PhotoUrl = employee.PhotoUrl;
                    Settings.UserId = employee.Id;

                    Xamarin.Forms.Application.Current.MainPage = new RootPage();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error logging in: {ex.Message}");
            }

            return success;
        }
    }
}
 ```

2. Now that we properly handle login, we need to handle refresh situations. Azure Active Directory tokens expire every hour, so it's important to handle this gracefully. Fortunately, we can use a `DelegatingHandler` to automatically handle this situation for us, and either reauthenticate the user using a refresh token, or prompt the user to log back in. In the `Helpers` directory, add a new class named `AuthenticationRefreshHandler`.

 ```csharp
 using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Yammerly.Services;

using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;

namespace Yammerly.Helpers
{
    public class AuthenticationRefreshHandler : DelegatingHandler
    {
        public MobileServiceClient Client { get; set; }

        SemaphoreSlim semaphore = new SemaphoreSlim(1);
        bool isReauthenticating = false;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Clone the request
            var clonedRequest = await CloneRequest(request);
            var response = await base.SendAsync(clonedRequest, cancellationToken);

            // If the token expired or is invalid, we need to refresh the token
            // or prompt the user to log back in.
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (isReauthenticating)
                    return response;
               
                if (Client == null)
                {
                    throw new InvalidOperationException("Make sure the MobileServiceClient has been set for the DelegatingHandler.");
                }

                var authenticationToken = Client.CurrentUser.MobileServiceAuthenticationToken;

                // In the event of two threads entering this method at the same time, only one should
                // do the refresh, or re-login.
                await semaphore.WaitAsync();

                // Token was already renewed by another concurrent thread
                if (authenticationToken != Client.CurrentUser.MobileServiceAuthenticationToken)
                {
                    semaphore.Release();

                    return await ResendRequest(request, cancellationToken);
                }

                // Refresh our authentication token
                isReauthenticating = true;
                bool didReceiveNewToken = false;

                try
                {
                    didReceiveNewToken = await RefreshToken();

                    // Token refresh failed, we need to re-login.
                    if (!didReceiveNewToken)
                    {
                        didReceiveNewToken = await DependencyService.Get<IAuthenticationService>().LoginAsync(Client, MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error Refreshing Auth: {ex.Message}");
                }
                finally
                {
                    isReauthenticating = false;
                    semaphore.Release();
                }

                if (didReceiveNewToken)
                {
                    if (!request.RequestUri.OriginalString.Contains("/.auth/me"))
                    {
                        return await ResendRequest(request, cancellationToken);
                    }
                }
            }

            return response;
        }

        async Task<HttpResponseMessage> ResendRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Clone the request
            var clonedRequest = await CloneRequest(request);

            // Set the authentication header
            clonedRequest.Headers.Remove("X-ZUMO-AUTH");
            clonedRequest.Headers.Add("X-ZUMO-AUTH", Client.CurrentUser.MobileServiceAuthenticationToken);

            // Resend the request
            return await base.SendAsync(clonedRequest, cancellationToken);
        }

        async Task<bool> RefreshToken()
        {
            try
            {
                var refreshJson = (JObject)await Client.InvokeApiAsync("/.auth/refresh", HttpMethod.Get, null);

                if (refreshJson != null)
                {
                    var newToken = refreshJson["authenticationToken"].Value<string>();
                    Client.CurrentUser.MobileServiceAuthenticationToken = newToken;
                    Settings.AuthToken = newToken;

                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error Refreshing Token: {ex.Message}");
            }

            return false;
        }

        private async Task<HttpRequestMessage> CloneRequest(HttpRequestMessage request)
        {
            var result = new HttpRequestMessage(request.Method, request.RequestUri);
            foreach (var header in request.Headers)
            {
                result.Headers.Add(header.Key, header.Value);
            }

            if (request.Content != null && request.Content.Headers.ContentType != null)
            {
                var requestBody = await request.Content.ReadAsStringAsync();
                var mediaType = request.Content.Headers.ContentType.MediaType;
                result.Content = new StringContent(requestBody, Encoding.UTF8, mediaType);
                foreach (var header in request.Content.Headers)
                {
                    if (!header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    {
                        result.Content.Headers.Add(header.Key, header.Value);
                    }
                }
            }

            return result;
        }
    }
}
 ```

2. Let's update our `AzureService` to ensure that all `HttpRequests` run through this handler. In the event the request needs authorization, Azure will automatically refresh the token (or prompt the user to log back in) and resend the authorized request. Update the `Initialize` method call to include this:

 ```csharp
             var handler = new AuthenticationRefreshHandler();
            MobileService = new MobileServiceClient("https://azure-training.azurewebsites.net", handler)
            {
                // Saves us from having to name things camel-case, or use custom JsonProperty attributes.
                SerializerSettings = new MobileServiceJsonSerializerSettings
                {
                    CamelCasePropertyNames = true
                }
            };
            handler.Client = MobileService;
 ```

3. We need to actually call our login logic. Jump over to `LoginViewModel` and update the `ExecuteLoginCommandAsync` method to use our `DependencyService` to log the user in.

  ```csharp
  var client = DependencyService.Get<IDataService>() as AzureService;
  await DependencyService.Get<IAuthenticationService>().LoginAsync(client.MobileService, MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);
  ```

4. Finally, let's update `App.xaml.cs` to properly take users through our login flow.

   ```csharp
                   DependencyService.Register<IDataService, AzureService>();

                if (!Settings.IsLoggedIn)
                    DependencyService.Get<IDataService>().Initialize();

                if (!Settings.IsLoggedIn)
                {
                    MainPage = new LoginPage();
                }
                else
                {
                    MainPage = new RootPage();
                }
   ```

5. Run the app, and you will be prompted to login. Pending successful login, you will be redirected to the main portion of the application. Your profile name will also be updated.

 ![](/modules/module-3/images/completed_auth.png)