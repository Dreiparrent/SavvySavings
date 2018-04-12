using System;

using SavvySavings.Services;
using SavvySavings.Views;
using Xamarin.Forms;
using Xamarin.Auth;
using System.Linq;

#if __ANDROID__
using SavvySavings.Droid.Services;
#endif
#if __IOS__
using SavvySavings.iOS.Services;
#endif

namespace SavvySavings
{
    public partial class App : Application
    {

        public static String AppName = "Savvy Savings";
        public ILocalAuth AuthStore => DependencyService.Get<AccountService>() ?? new AccountService();
        public App()
        {
            InitializeComponent();
            if (AuthStore.GetAuthState())
            {
                if (AuthStore.AuthAccount.Name == null)
                {
                    MainPage = new NavigationPage(new CreateAccount());
                }
                else if (!AuthStore.AuthAccount.EmailVerified)
                {
                    MainPage = new NavigationPage(new VerifyEmailPage(AuthStore));
                }
                else
                {
                    /*
                    var homePage = new HomePage();
                    MainPage = new NavigationPage(homePage);
                    MainPage.Navigation.InsertPageBefore(new MainPage(), homePage);
                    new MainPage();
                    */
                    MainPage = new MainPage();
                }
            } else
                MainPage = new NavigationPage(new LoginPage());

            AuthStore.DetachListener();
        }

        public string UserName
        {
            get
            {
                var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
                return (account != null) ? account.Username : null;
            }
        }

        public string Password
        {
            get
            {
                var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
                return (account != null) ? account.Properties["Password"] : null;
            }
        }

        protected override void OnStart ()
		{
			// Handle when your app starts

		}

		protected override void OnSleep ()
		{
            // Handle when your app sleeps
            AuthStore.DetachListener();
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
