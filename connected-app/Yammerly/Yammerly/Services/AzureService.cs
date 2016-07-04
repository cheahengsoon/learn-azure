using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yammerly.Helpers;
using Yammerly.Models;

using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Xamarin.Forms;

namespace Yammerly.Services
{
    public class AzureService : IDataService
    {
        public MobileServiceClient MobileService { get; set; }

        bool isInitialized;

        public async Task Initialize()
        {
            if (isInitialized)
                return;

            var authenticationHandler = new AuthenticationRefreshHandler();
            MobileService = new MobileServiceClient("https://yammerlyproduction.azurewebsites.net", authenticationHandler)
            {
                // Saves us from having to name things camel-case, or use custom JsonProperty attributes.
                SerializerSettings = new MobileServiceJsonSerializerSettings
                {
                    CamelCasePropertyNames = true
                }
            };
            authenticationHandler.Client = MobileService;

            MobileService.CurrentUser = new MobileServiceUser(Settings.UserId);
            MobileService.CurrentUser.MobileServiceAuthenticationToken = Settings.AuthToken;
            
            var store = new MobileServiceSQLiteStore("app.db");
            store.DefineTable<Employee>();
            store.DefineTable<TimelineItem>();

            await MobileService.SyncContext.InitializeAsync(store);
            
            isInitialized = true;
        }

        public async Task<bool> LoginAsync()
        {
            var result = await DependencyService.Get<IAuthenticationService>().LoginAsync(MobileService, MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);

            // Fetch current employee profile.
            var user = await MobileService.InvokeApiAsync<Employee>("UserInfo", System.Net.Http.HttpMethod.Get, null);
            Settings.FirstName = user.FirstName;
            Settings.LastName = user.LastName;
            Settings.PhotoUrl = user.PhotoUrl;

            return result;
        }
        
        #region Data Access
        public async Task<IEnumerable<T>> GetItems<T>() where T : EntityData
        {
            await Initialize();

            await SyncItems<T>();
            return await MobileService.GetSyncTable<T>().ToEnumerableAsync();
        }

        public async Task AddItem<T>(T item) where T : EntityData
        {
            await Initialize();

            await MobileService.GetSyncTable<T>().InsertAsync(item);
            await SyncItems<T>();
        }

        public async Task UpdateItem<T>(T item) where T : EntityData
        {
            await Initialize();

            await MobileService.GetSyncTable<T>().UpdateAsync(item);
            await SyncItems<T>();
        }

        public async Task RemoveItem<T>(T item) where T : EntityData
        {
            await Initialize();

            await MobileService.GetSyncTable<T>().DeleteAsync(item);
            await SyncItems<T>();
        }

        public async Task SyncItems<T>() where T : EntityData
        {
            await Initialize();

            try
            {
                await MobileService.SyncContext.PushAsync();
                await MobileService.GetSyncTable<T>().PullAsync($"all{typeof(T).Name}", MobileService.GetSyncTable<T>().CreateQuery());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during Sync occurred: {ex.Message}");
            }
        }
#endregion
    }
}
