using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Location;

namespace SavvySavings.Droid.Services
{
    public class FusedLocationProviderCallback : LocationCallback
    {
        readonly MainActivity mainActivity;

        private LocationRequest locationRequest;
        

        public LocationRequest LocationRequest
        {
            get { return locationRequest;  }
        }

        public FusedLocationProviderCallback(MainActivity activity)
        {
            this.mainActivity = activity;
            this.locationRequest = new LocationRequest()
                    .SetPriority(LocationRequest.PriorityBalancedPowerAccuracy)
                    .SetInterval(20 * 60 * 1000)
                    .SetFastestInterval(5 * 60 * 1000);
        }

        public override void OnLocationAvailability(LocationAvailability locationAvailability)
        {

        }
        public override void OnLocationResult(LocationResult result)
        {
            if (result.Locations.Any())
            {
                var location = result.Locations.First();
                Console.WriteLine($"Location: {location}");
            }
        }
    }
}