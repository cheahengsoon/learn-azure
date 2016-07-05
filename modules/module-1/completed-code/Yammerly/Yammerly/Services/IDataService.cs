using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yammerly.Models;

namespace Yammerly.Services
{
    public interface IDataService
    {
        Task Initialize();
        Task<IEnumerable<T>> GetItems<T>() where T : EntityData;
        Task AddItem<T>(T item) where T : EntityData;
        Task UpdateItem<T>(T item) where T : EntityData;
        Task RemoveItem<T>(T item) where T : EntityData;
        Task SyncItems<T>() where T : EntityData;
    }
}
