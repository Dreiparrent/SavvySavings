using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SavvySavings.Services;
using SavvySavings.Models;
#if __ANDROID__
using SavvySavings.Droid.Services;
#endif
#if __IOS__
using SavvySavings.iOS.Services;
#endif
using Acr.UserDialogs;

namespace SavvySavings.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreateAccount : ContentPage
	{
        public ILocalAuth AuthStore => DependencyService.Get<AccountService>() ?? new AccountService();
        public CreateAccount (Account account = null)
		{
			InitializeComponent ();
            submitButton.IsEnabled = false;
            var dateNow = DateTime.Now.AddYears(-10);
            bdEntry.MaximumDate = dateNow;
            bdEntry.MinimumDate = new DateTime(1900, 1, 1);

            if(account != null)
            {
                submitButton.IsVisible = HeaderLabel.IsVisible = false;
                nameEntry.Text = account.Name;
                bdEntry.Date = account.Birthday;
                radiusPicker.SelectedIndex = (int)account.SalesRadius;
            } else
            {
                saveButton.IsVisible = false;
                bdEntry.Date = dateNow.AddDays(-1);
            }
        }

        private void NameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length < 1)
            {
                submitButton.IsEnabled = false;
            } else
            {
                submitButton.IsEnabled = true;
            }
        }

        private async void SubmitButton_Clicked(object sender, EventArgs e)
        {
            await AuthStore.CheckVerification();
            AuthStore.SetAccountData(nameEntry.Text, bdEntry.Date.ToString("MM/dd/yyyy"), radiusPicker.SelectedIndex, true);
            await Navigation.PushModalAsync(new MainPage());
            UserDialogs.Instance.Alert("Please make sure to verify your email before your next login", "Verify Email");
            //TODO: add dialog after await
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            var name = nameEntry.Text;
            var birthday = bdEntry.Date;
            var radius = radiusPicker.SelectedIndex;
            AuthStore.SetAccountData(name, birthday.ToString("MM/dd/yyyy"), radius, false);
            AccountPage.viewModel.AuthStore.AuthAccount.Name = name;
            AccountPage.viewModel.AuthStore.AuthAccount.Birthday = birthday;
            AccountPage.viewModel.AuthStore.AuthAccount.SalesRadius = radius;
            Navigation.PopAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            AuthStore.DetachListener();
        }
    }
}