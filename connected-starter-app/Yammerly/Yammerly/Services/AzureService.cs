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

            throw new NotImplementedException();
            
            isInitialized = true;
        }

        #region Data Access
        public async Task<IEnumerable<T>> GetItems<T>() where T : EntityData
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetItem<T>(string id) where T : EntityData
        {
            throw new NotImplementedException();
        }

        public async Task AddItem<T>(T item) where T : EntityData
        {
            throw new NotImplementedException();
        }

        public async Task UpdateItem<T>(T item) where T : EntityData
        {
            throw new NotImplementedException();
        }

        public async Task RemoveItem<T>(T item) where T : EntityData
        {
            throw new NotImplementedException();
        }

        public async Task SyncItems<T>() where T : EntityData
        {
            throw new NotImplementedException();
        }
#endregion
    }
}
