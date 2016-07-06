using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Yammerly.Helpers;
using Yammerly.Models;

using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.Storage.Blob;

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

            // MobileServiceClient handles communication with our backend, auth, and more for us.
            var handler = new AuthenticationRefreshHandler();
            MobileService = new MobileServiceClient("https://azuretrainingrunthrough.azurewebsites.net", handler)
            {
                // Saves us from having to name things camel-case, or use custom JsonProperty attributes.
                SerializerSettings = new MobileServiceJsonSerializerSettings
                {
                    CamelCasePropertyNames = true
                }
            };
            handler.Client = MobileService;

            // Configure online/offline sync.
            var store = new MobileServiceSQLiteStore("app.db");
            store.DefineTable<Employee>();
            store.DefineTable<TimelineItem>();
            await MobileService.SyncContext.InitializeAsync(store);

            isInitialized = true;
        }

        #region Data Access
        public async Task<IEnumerable<T>> GetItems<T>() where T : EntityData
        {
            await Initialize();

            await SyncItems<T>();
            return await MobileService.GetSyncTable<T>().ToEnumerableAsync();
        }

        public async Task<T> GetItem<T>(string id) where T : EntityData
        {
            await Initialize();

            await SyncItems<T>();

            return await MobileService.GetSyncTable<T>().LookupAsync(id);
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
