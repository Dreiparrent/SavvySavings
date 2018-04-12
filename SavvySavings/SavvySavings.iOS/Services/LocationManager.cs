using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreLocation;

namespace SavvySavings.iOS.Services
{
    public class LocationManager
    {
        protected CLLocationManager locMgr;
        // event for the location changing
        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };
        public event EventHandler<DidEnterRegionEventArgs> RegionEntered = delegate { };

        public int acuracy;

        public LocationManager(int desiredAcuracy)
        {
            this.acuracy = desiredAcuracy;
            this.locMgr = new CLLocationManager
            {
                PausesLocationUpdatesAutomatically = false
            };

            // iOS 8 has additional permissions requirements
            // works in background
            locMgr.RequestAlwaysAuthorization();
            // iOS 9 requires aditional permisions;
            locMgr.AllowsBackgroundLocationUpdates = true;

            var nord = new CLLocationCoordinate2D(39.716864, -104.955342);
            MonitorRegionAtLocation(nord, "nord");
        }

        public CLLocationManager LocMgr
        {
            get { return this.locMgr; }
        }

        public void StartLocationUpdates()
        {
            if (CLLocationManager.LocationServicesEnabled)
            {
                //set the desired accuracy, in meters
                LocMgr.DesiredAccuracy = acuracy;
                LocMgr.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
                {
                    // fire our custom Location Updated event
                    LocationUpdated(this, new LocationUpdatedEventArgs(e.Locations[e.Locations.Length - 1]));
                };
                LocMgr.StartUpdatingLocation();
            }
        }
        public void MonitorRegionAtLocation(CLLocationCoordinate2D center, string identifier)
        {
            if (CLLocationManager.Status == CLAuthorizationStatus.AuthorizedAlways)
            {
                var maxDistance = LocMgr.MaximumRegionMonitoringDistance;
                var region = new CLCircularRegion(center, maxDistance, identifier);
                region.NotifyOnEntry = true;
                region.NotifyOnExit = false;
                LocMgr.RegionEntered += (object sender, CLRegionEventArgs e) =>
                {
                    RegionEntered(this, new DidEnterRegionEventArgs(e.Region));
                };
                LocMgr.StartMonitoring(region);
            }
        }
    }
}