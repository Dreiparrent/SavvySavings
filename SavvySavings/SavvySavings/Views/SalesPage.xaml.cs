using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SavvySavings.Models;
using SavvySavings.Views;
using SavvySavings.ViewModels;

namespace SavvySavings.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SalesPage : ContentPage
    {
        public static SalesViewModel viewModel;
        public static Boolean gotoMap = false;

        public SalesPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new SalesViewModel();
        }

        public void OnSaleSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var sale = args.SelectedItem as Sale;
            if (sale == null)
                return;

            MapPage.moveMap = true;
            MapPage.iLat = sale.Lat;
            MapPage.iLng = sale.Lng;
            MapPage.iRadius = 0.5;
            SwitchTabMap(true);
        }

        public void SwitchTabMap(Boolean itemSelected)
        {
            var tabbedPage = this.Parent.Parent as TabbedPage;
            tabbedPage.CurrentPage = tabbedPage.Children[1];
            if (itemSelected)
            {
                SalesListView.SelectedItem = null;
            }
        }

        async void Home_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new HomePage(false)));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (gotoMap)
            {
                gotoMap = false;
                SwitchTabMap(false);
            }

            if (viewModel.Sales.Count == 0)
                viewModel.LoadSalesCommand.Execute(null);
        }
    }
}