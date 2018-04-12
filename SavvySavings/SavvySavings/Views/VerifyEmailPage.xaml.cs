using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SavvySavings.ViewModels;
using SavvySavings.Services;

namespace SavvySavings.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VerifyEmailPage : ContentPage
	{
        ILocalAuth authStore;
        public VerifyEmailPage (ILocalAuth authStore)
		{
			InitializeComponent ();
            this.authStore = authStore;
            RecheckVerification();
        }

        public async void RecheckVerification()
        {
            Boolean isVerified = await authStore.CheckVerification();
            if(isVerified)
                await Navigation.PushModalAsync(new MainPage());
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            authStore.SendEmailVerification();
            Navigation.PushModalAsync(new MainPage());
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            authStore.DetachListener();
        }
    }
}