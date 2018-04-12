using System;
using System.Collections.Generic;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Java.Lang;
using Java;
using Firebase;
using Android.Gms.Common;
using Firebase.Analytics;
using Firebase.Iid;
using Android.Util;

using SavvySavings.Models;
using SavvySavings.Services;
using SavvySavings.Droid.Services;
using Acr.UserDialogs;
using Android.Gms.Location;
using Android.Content;
using Android.Locations;

namespace SavvySavings.Droid
{
    [Activity(Label = "SavvySavings", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        FirebaseAnalytics firebaseAnalytics;
        FusedLocationProviderClient fusedLocationProviderClient;

        protected override void OnCreate(Bundle bundle)
        {             
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            UserDialogs.Init(this);

            FirebaseApp.InitializeApp(this);
            firebaseAnalytics = FirebaseAnalytics.GetInstance(this);

            var playAvailable = IsPlayServicesAvailable();
            if (playAvailable)
            {
                new GetRemoteConfig();
                Intent locIntent = new Intent(this, typeof(SaleLocationService));
                StartService(locIntent);
                fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
                FusedLocationProviderCallback callback = new FusedLocationProviderCallback(this);
                fusedLocationProviderClient.RequestLocationUpdatesAsync(callback.LocationRequest, callback);
            }
            
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            global::Xamarin.FormsMaps.Init(this, bundle);
            LoadApplication(new App());
#if DEBUG
            MocksLocation();
#endif
        }

        public void MocksLocation()
        {
            Location location = new Location(LocationManager.GpsProvider);

            location.Latitude = 39.716;// Add700ToCoordinates();
            location.Longitude = -104.955;// GlobalLongitude;
            location.Accuracy = 0;
            location.Time = DateTime.Now.Ticks;
            location.ElapsedRealtimeNanos = 100;
            location.Speed = 0.0f;
            location.Altitude = 1.0;
            location.Bearing = 0.0f;

            LocationManager locationManager = GetSystemService(Context.LocationService) as LocationManager;

            locationManager.AddTestProvider(LocationManager.GpsProvider, false, false, false, false, false, false, false, Power.Low, Android.Hardware.SensorStatus.AccuracyHigh);
            locationManager.SetTestProviderLocation(LocationManager.GpsProvider, location);
            locationManager.SetTestProviderEnabled(LocationManager.GpsProvider, true);

            Log.Debug("ANY?", "Mocklocation has been called by service!");

            //return location.Latitude.ToString();
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    //TODO: Log errors somehow
                    //var bundle = new Bundle();
                    //bundle.put

                } else
                {
                    Console.WriteLine("Device is not supported");
                    Finish();
                }
                return false;
            }
            else
            {
                //Google Play Services is available."
                return true;
            }
        }
    }
}

