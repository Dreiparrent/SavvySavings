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
using Android.Util;

namespace SavvySavings.Droid.Services
{
    public class SaleLocationService : Service
    {

        IList<IGeofence> mGeofenceList;
        PendingIntent mGeofencePendingIntent;
        public override void OnCreate()
        {
            mGeofenceList = new List<IGeofence>();
            Dictionary<string, double> nord = new Dictionary<string, double>()
            {
                { "lat", 39.716864 },
                { "lng", -104.955342 }
            };

            Dictionary<string, double> test = new Dictionary<string, double>()
            {
                { "lat", 39.716 },
                { "lng", -104.955 }
            };

            GeofencingClient geofencingClient = LocationServices.GetGeofencingClient(this);

            mGeofenceList.Add(new GeofenceBuilder()
                .SetRequestId("nord")
                .SetCircularRegion(nord["lat"], nord["lng"], 3f)
                .SetExpirationDuration(Geofence.NeverExpire)
                .SetTransitionTypes(Geofence.GeofenceTransitionEnter | Geofence.GeofenceTransitionExit)
                .Build());

            geofencingClient.AddGeofences(GetGeofencingRequest(), GetGeofencePendingIntent());
            Console.WriteLine("WTF WTF WTF WTF");
            Log.Debug("THIS", "WTF Created!");
        }

        private GeofencingRequest GetGeofencingRequest()
        {
            GeofencingRequest.Builder builder = new GeofencingRequest.Builder();
            builder.SetInitialTrigger(GeofencingRequest.InitialTriggerEnter);
            builder.AddGeofences(mGeofenceList);
            return builder.Build();
        }

        private PendingIntent GetGeofencePendingIntent()
        {
            if (mGeofencePendingIntent != null)
                return mGeofencePendingIntent;
            Android.Content.Intent intent = new Android.Content.Intent(this, typeof(GeofenceTransitionsIntentService));
            mGeofencePendingIntent = PendingIntent.GetService(this, 0, intent, PendingIntentFlags.UpdateCurrent);
            return mGeofencePendingIntent;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("THIS", "WTF Started!");
            return StartCommandResult.Sticky;
        }
    }
}