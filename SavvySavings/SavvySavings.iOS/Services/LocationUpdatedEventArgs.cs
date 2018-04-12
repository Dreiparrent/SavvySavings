using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreLocation;

namespace SavvySavings.iOS.Services
{
    public class LocationUpdatedEventArgs: EventArgs
    {
        CLLocation location;
        public CLLocation Location { get { return location; } }

        public LocationUpdatedEventArgs(CLLocation location)
        {
            this.location = location;
        }
    }
}