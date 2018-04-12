using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using SavvySavings.Models;
using SavvySavings.Views;

namespace SavvySavings.ViewModels
{
    public class SalesViewModel : SaleBaseViewModel
    {

        public ObservableCollection<Sale> Sales { get; set; }
        public Command LoadSalesCommand { get; set; }

        public SalesViewModel()
        {
            Title = "Browse";
            Sales = new ObservableCollection<Sale>();
            LoadSalesCommand = new Command(async () => await ExecuteLoadSalesCommand());

            /*
            MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            {
                var _item = item as Item;
                Items.Add(_item);
                await DataStore.AddItemAsync(_item);
            });
            */
        }

        async Task ExecuteLoadSalesCommand()
        {

            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Sales.Clear();
                var sales = await DataStore.GetItemsAsync(true);
                foreach (var sale in sales)
                {
                    Sales.Add(sale);
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
