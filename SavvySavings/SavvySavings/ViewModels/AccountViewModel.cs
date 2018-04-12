using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using SavvySavings.Models;

namespace SavvySavings.ViewModels
{
    public class AccountViewModel : AccountBaseViewModel
    {
        public ObservableCollection<WatchItem> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public ObservableCollection<CheckIn> CheckIns { get; set; }
        public Command LoadCheckInsCommand { get; set; }

        public AccountViewModel()
        {
            Title = "Account";

            Items = new ObservableCollection<WatchItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            CheckIns = new ObservableCollection<CheckIn>();
            LoadCheckInsCommand = new Command(async () => await ExecuteLoadCheckInsCommand());

            /*
            MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            {
                var _item = item as Item;
                Items.Add(_item);
                await DataStore.AddItemAsync(_item);
            });
            */
        }

        async Task ExecuteLoadItemsCommand()
        {

            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await WatchItemStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task ExecuteLoadCheckInsCommand()
        {

            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                CheckIns.Clear();
                SPoints = 0;
                var checkIns = await ChDataStore.GetCheckInsAsync(true);
                foreach (var checkIn in checkIns)
                {
                    CheckIns.Add(checkIn);
                    SPoints += checkIn.SpValue;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}