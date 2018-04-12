using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SavvySavings.ViewModels;

namespace SavvySavings.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterPage : ContentPage
	{
        LogRegViewModel viewModel;
        public RegisterPage ()
		{
			InitializeComponent ();
            BindingContext = viewModel = LoginPage.viewModel;

            FailLabel.IsVisible = false;
            RegisterButton.IsEnabled = false;
            Email.Text = viewModel.EmailAddress;
            Password.Text = viewModel.Password;
		}

        async void Register_Clicked(object sender, EventArgs e)
        {
            String eml = Email.Text;
            String pswd = Password.Text;
            String pswdChk = PasswordChk.Text;
            if (pswd == pswdChk)
            {
                Boolean isSignin = await viewModel.AuthStore.CreateAccountAsync(eml, pswd);
                if (isSignin)
                    await Navigation.PushModalAsync(new CreateAccount());
                else
                    FailLabel.IsVisible = true;
            }
            else
            {
                await PasswordChk.TranslateTo(-10f, 0, 50);
                await PasswordChk.TranslateTo(10f, 0, 50);
                await PasswordChk.TranslateTo(-10f, 0, 50);
                await PasswordChk.TranslateTo(10f, 0, 50);
                await PasswordChk.TranslateTo(0, 0, 50);
            }
        }

        private void Email_Changed(object sender, TextChangedEventArgs e)
        {
            FailLabel.IsVisible = false;
            RegisterButton.IsEnabled = viewModel.IsValid(emailAddress: e.NewTextValue);
        }

        private void Password_Changed(object sender, TextChangedEventArgs e)
        {
            FailLabel.IsVisible = false;
            RegisterButton.IsEnabled = viewModel.IsValid(password: e.NewTextValue);
        }
        private void PasswordChk_Changed(object sender, TextChangedEventArgs e)
        {
            FailLabel.IsVisible = false;
            RegisterButton.IsEnabled = viewModel.IsValid(pass2: e.NewTextValue);
        }
    }
}