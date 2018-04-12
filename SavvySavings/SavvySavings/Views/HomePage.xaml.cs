using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SavvySavings.Models;
using SavvySavings.ViewModels;
using Acr.UserDialogs;

namespace SavvySavings.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : ContentPage
	{
        public static AccountViewModel viewModel;
        public static String userName;
        private Boolean androidFirstLaunch = true;
        public Boolean AndroidFirstLaunch
        {
            get { return androidFirstLaunch; }
            set { androidFirstLaunch = value; }
        }

        public HomePage(Boolean isAndroidFirst = false)
		{
			InitializeComponent();

            BindingContext = viewModel = new AccountViewModel();

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    headerLayout.Padding = new Thickness(0, 20, 0, 0);
                    break;
                case Device.Android:
                    headerLayout.Padding = new Thickness(0, 0, 0, 0);
                    break;
                case Device.WinPhone:
                    headerLayout.Padding = new Thickness(0, 10, 0, 0);
                    break;
            }
            AndroidFirstLaunch = isAndroidFirst;

            //AddHomeShortcuts();
        }
        
        async void Watch_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new WatchPage());
        }

        async void Map_Clicked(object sender, EventArgs e)
        {
            SalesPage.gotoMap = true;
            await Navigation.PopModalAsync();
        }

        async void Sales_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        async void PushAccount()
        {
            int modalNavCount = Navigation.ModalStack.Count;
            int navCount = Navigation.ModalStack.Count;

            Console.WriteLine($"Nav Numbers: {modalNavCount} | {navCount}");
            Console.WriteLine("1");
            var nav = (INavigation)this.Navigation;
            Console.WriteLine("2");
            await nav.PushAsync((Page)(new AccountPage()));
            Console.WriteLine("3");
            /*
            if(AndroidFirstLaunch)
            {
                await Navigation.PushAsync(new NavigationPage(new AccountPage()));
            } else
            {
                await Navigation.PushAsync(new AccountPage());
            }
            */
        }

        async void Points_Clicked(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem;
            if (item == null)
                return;
            await Navigation.PushAsync(new PointsPage());
            CheckInsListView.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            //base.OnAppearing();
            Console.WriteLine($"Appearing {viewModel.CheckIns.Count.ToString()}");
            if (viewModel.CheckIns.Count == 0)
                viewModel.LoadCheckInsCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            viewModel.AuthStore.DetachListener();
        }
    }
}