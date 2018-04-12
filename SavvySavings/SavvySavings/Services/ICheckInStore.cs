using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SavvySavings.Services
{
    public interface ICheckInStore<T>
    {
        Task<IEnumerable<T>> GetCheckInsAsync(bool forceRefresh = false);
    }
}
