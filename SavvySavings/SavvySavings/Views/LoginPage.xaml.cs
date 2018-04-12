using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Auth;

using SavvySavings.Views;
using SavvySavings.ViewModels;

namespace SavvySavings.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
        public static LogRegViewModel viewModel;
        String tmpEmail;
        String tmpPassword;
        public LoginPage ()
		{
			InitializeComponent ();
            viewModel = new LogRegViewModel();
            ErrorLabel.IsVisible = false;
            loginButton.IsEnabled = false;
            tmpEmail = tmpPassword = "";
        }

        async void Login_Clicked(object sender, EventArgs e)
        {
            String eml = email.Text;
            String pswd = password.Text;
            Boolean isSignin = await viewModel.AuthStore.SignIAsync(eml, pswd);
            if (isSignin)
                await Navigation.PushModalAsync(new MainPage());
            else
                ErrorLabel.IsVisible = true;
            //SaveCredentials(usr, pswd);
        }

        async void Register_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }

        private void Email_Changed(object sender, TextChangedEventArgs e)
        {
            ErrorLabel.IsVisible = false;
            tmpEmail = e.NewTextValue;
            loginButton.IsEnabled = viewModel.IsValid(emailAddress: e.NewTextValue);
        }

        private void Password_Changed(object sender, TextChangedEventArgs e)
        {
            ErrorLabel.IsVisible = false;
            tmpPassword = e.NewTextValue;
            loginButton.IsEnabled = viewModel.IsValid(password: e.NewTextValue);
        }


        public void SaveCredentials(string userName, string password)
        {
            if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
            {
                Account account = new Account
                {
                    Username = userName
                };
                account.Properties.Add("Password", password);
                AccountStore.Create().Save(account, App.AppName);
                HomePage.userName = userName;
                GoToHome();
                //Navigation.RemovePage(this);
            }
        }

        async void GoToHome()
        {
            
            //await Navigation.PopModalAsync();
            await Navigation.PushModalAsync(new CreateAccount());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //Navigation.PushModalAsync(new NavigationPage(new HomePage(true)));
        }
    }
}