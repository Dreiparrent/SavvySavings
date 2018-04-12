using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using SavvySavings.ViewModels;
using SavvySavings.Views;
using SavvySavings.Models;

namespace SavvySavings.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MapPage : ContentPage
	{
        public ObservableCollection<Sale> sales;
        public List<Pin> pins;
        public static Boolean moveMap;
        public static Double iLat;
        public static Double iLng;
        public static Double iRadius = 0;

        public MapPage()
        {
            InitializeComponent ();
            sales = SalesPage.viewModel.Sales;
            pins = new List<Pin>();
            moveMap = false;
            iLat = 39.73;
            iLng = -104.945;
            if (iRadius == 0)
            iRadius =  3;
            MainMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(iLat, iLng), Distance.FromMiles(iRadius)));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (moveMap)
            {
                MainMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(iLat, iLng), Distance.FromMiles(iRadius)));
                moveMap = false;
            }            
            MainMap.Pins.Clear();
            SalesPage.viewModel.LoadSalesCommand.Execute(null);
            foreach (Sale sale in sales)
            {
                AddPin(sale.Lat, sale.Lng, sale.Name, sale.Desc);
            }
        }

        public void AddPin(Double sLat, Double sLng, String sName, String sDesc)
        {
            Position pos = new Position(sLat, sLng);
            Pin pin = new Pin
            {
                Position = pos,
                Label = sName,
                Address = sDesc
            };
            pins.Add(pin);
            Console.WriteLine("Pin, Lat: {0}, Long: {1}", pin.Position.Latitude, pin.Position.Longitude);
            MainMap.Pins.Add(pin);
        }


    }
}