using System;

using SavvySavings.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;


namespace SavvySavings.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : TabbedPage
	{

        public Boolean isInitialized = false;
        public MainPage()
		{
            InitializeComponent();

            if(Device.RuntimePlatform != Device.Android)
                OpenHome();
        }
        
        async void OpenHome()
        {
            await Navigation.PushModalAsync(new NavigationPage(new HomePage()));
            //homePage.Animate("", s => Layout(new Rectangle(((-1 + s) * Width), Y, Width, Height)), 16, 250, Easing.Linear, null, null);
        }
    }
}