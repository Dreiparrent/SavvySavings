using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Auth;
using System.Collections.ObjectModel;

using SavvySavings.Views;
using SavvySavings.Models;
using SavvySavings.ViewModels;

using Acr.UserDialogs;

namespace SavvySavings.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AccountPage : ContentPage
	{

        public static AccountViewModel viewModel;

        public AccountPage ()
		{
			InitializeComponent ();
            BindingContext = viewModel = HomePage.viewModel;
            string realRadius;
            switch (viewModel.AuthStore.AuthAccount.SalesRadius)
            {
                case 0:
                    realRadius = "Search Radius: 1 mile";
                    break;
                case 1:
                    realRadius = "Search Radius: 3 miles";
                    break;
                case 2:
                    realRadius = "Search Radius: 5 miles";
                    break;
                case 3:
                    realRadius = "Search Radius: 10 miles";
                    break;
                case 4:
                    realRadius = "Search Radius: 20 miles";
                    break;
                default:
                    realRadius = "Search Radius: 3 miles";
                    break;
            }
            RadiusLabel.Text = realRadius;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }

        private void EditAcct_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CreateAccount(viewModel.AuthStore.AuthAccount));
        }

        private void EditWatch_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new WatchPage());
        }

        async void Logout_Clicked(object sender, EventArgs e)
        {
            var loggedOut = viewModel.AuthStore.Logout();
            if (loggedOut)
            {
                await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
                Navigation.RemovePage(this);
            }
        }

        async void GoToLogin()
        {
            Navigation.InsertPageBefore(new NavigationPage(new LoginPage()),this);
            await Navigation.PopModalAsync();
        }
    }    
}