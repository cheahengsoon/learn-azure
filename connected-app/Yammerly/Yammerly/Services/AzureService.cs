using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yammerly.Models;

using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;

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

            MobileService = new MobileServiceClient("https://yammerlyproduction.azurewebsites.net", null)
            {
                // Saves us from having to name things camel-case, or use custom JsonProperty attributes.
                SerializerSettings = new MobileServiceJsonSerializerSettings
                {
                    CamelCasePropertyNames = true
                }
            };

            var store = new MobileServiceSQLiteStore("app.db");
            store.DefineTable<Employee>();
            store.DefineTable<TimelineItem>();

            await MobileService.SyncContext.InitializeAsync(store);
            
            isInitialized = true;
        }

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
    }
}
