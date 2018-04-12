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
    [Service]
    public class GeofenceTransitionsIntentService : IntentService
    {

        protected override void OnHandleIntent(Intent intent)
        {
            GeofencingEvent geofencingEvent = GeofencingEvent.FromIntent(intent);
            if (geofencingEvent.HasError)
            {
                int err = geofencingEvent.ErrorCode;
                //TODO: get this error code
                Console.WriteLine($"Error: {err}");
                return;
            }

            //Get Transition Type
            int geofenceTransition = geofencingEvent.GeofenceTransition;

            //Test the transition

            if (geofenceTransition == Geofence.GeofenceTransitionEnter ||
                geofenceTransition == Geofence.GeofenceTransitionExit)
            {
                IList<IGeofence> triggeringGeofences = geofencingEvent.TriggeringGeofences;

                foreach (var transition in triggeringGeofences)
                {
                    Console.WriteLine(transition.RequestId, geofenceTransition);
                }

                Console.WriteLine("ENTERED ENTERED ENTERED ENTERED ENTERED ENTERED!!!!");

                //TODO: send notification here?
            }
            else
            {
                Console.WriteLine(geofenceTransition.GetTypeCode());
            }
        }
    }
}