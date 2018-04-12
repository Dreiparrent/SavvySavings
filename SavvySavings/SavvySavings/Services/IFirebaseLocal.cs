using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

namespace SavvySavings.Services
{
    public interface ILocalFirebase<T>
    {
        void Init();
        //Task<bool> UpdateItemAsync(T item);
        //Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }
}
