using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace SavvySavings.ViewModels
{
    public class MapViewModel
    {
        public static String Title;

        public MapViewModel()
        {
            Title = "Map";

            //OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
        }

        public ICommand OpenWebCommand { get; }
    }
}
