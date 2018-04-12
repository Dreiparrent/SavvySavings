using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SavvySavings.Models;

[assembly: Xamarin.Forms.Dependency(typeof(SavvySavings.Services.MockAccountDataStore))]
namespace SavvySavings.Services
{
    public class MockAccountDataStore : IWatchItemStore<WatchItem>, ICheckInStore<CheckIn>
    {
        List<WatchItem> items;
        List<CheckIn> checkIns;

        public MockAccountDataStore()
        {
            items = new List<WatchItem>();

            var mockItems = new List<WatchItem>
            {
               new WatchItem{Name = "Bananna Republic", Id = "1"},
               new WatchItem{Name = "Forever 21", Id = "2"},
               new WatchItem{Name = "Shoes", Id = "3"},
               new WatchItem{Name = "Converse", Id = "4"},
               new WatchItem{Name = "Hightops", Id = "5"},
               new WatchItem{Name = "Polo Shirts", Id = "6"},
               new WatchItem{Name = "16th Streen Mall", Id = "7"}
            };

            foreach (var item in mockItems)
            {
                items.Add(item);
            }

            checkIns = new List<CheckIn>();
            /*
            var mockCheckins = new List<CheckIn>
            {
                new CheckIn{Name = "Bananna Republic", InDate=new DateTime(2018,1,24,16,0,0), SpValue = 200, Lat=24,Lng=24,Id=0},
                new CheckIn{Name = "Forever 21", InDate=new DateTime(2018,2,2,17,24,15), SpValue= 200, Lat=24,Lng=24,Id=1},
                new CheckIn{Name = "Nordstrom", InDate=new DateTime(2018,2,4,17,15,26), SpValue = 250, Lat=24,Lng=24,Id=2}
            };
            
            foreach(var checkIn in mockCheckins)
            {
                checkIns.Add(checkIn);
            }
            */
        }

        
        public async Task<bool> AddItemAsync(WatchItem item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(WatchItem item)
        {
            /*
            var _item = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(_item);
            items.Add(item);
            */
            return await Task.FromResult(true);
        }
        
        public async Task<bool> DeleteItemAsync(WatchItem item)
        {
            var _item = items.Where((WatchItem arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(_item);

            return await Task.FromResult(true);
        }
        /*
        public async Task<WatchItem> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }
        */
        public async Task<IEnumerable<WatchItem>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }

        public async Task<IEnumerable<CheckIn>> GetCheckInsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(checkIns);
        }
    }
}
