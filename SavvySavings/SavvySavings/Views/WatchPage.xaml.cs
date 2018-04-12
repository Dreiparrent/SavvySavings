using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SavvySavings.ViewModels;
using SavvySavings.Models;
using Acr.UserDialogs;

namespace SavvySavings.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WatchPage : ContentPage
	{
        public static AccountViewModel viewModel;
        public WatchPage ()
		{
			InitializeComponent ();

            BindingContext = viewModel = new AccountViewModel();
		}

        async void Delete_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var item = (WatchItem)mi.CommandParameter;
            await viewModel.WatchItemStore.DeleteItemAsync(item);
            WatchItemListView.BeginRefresh();
        }

        public async void OnSaleSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as WatchItem;
            if (item == null)
                return;
            PromptConfig config = new PromptConfig
            {
                Title = "Update",
                Message = "Update Watch Item",
                Placeholder = "Watch Item",
                OkText = "Save",
                Text = item.Name
            };
            var dialog = await UserDialogs.Instance.PromptAsync(config);
            var itemName = dialog.Value;
            WatchItem updateItem = new WatchItem
            {
                Id = item.Id,
                Name = itemName
            };
            await viewModel.WatchItemStore.UpdateItemAsync(updateItem);
            WatchItemListView.SelectedItem = null;
            //TODO: add INotify
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }

        private async void Add_Clicked(object sender, EventArgs e)
        {
            PromptConfig config = new PromptConfig
            {
                Title = "Add",
                Message = "Add your watch item",
                Placeholder = "Watch Item",
                OkText = "Save"
            };
            var dialog = await UserDialogs.Instance.PromptAsync(config);
            var itemName = dialog.Value;
            WatchItem addItem = new WatchItem
            {
                Id = new Random().ToString(),
                Name = itemName
            };
            await viewModel.WatchItemStore.AddItemAsync(addItem);
            //TODO: add INotify
        }

    }
}