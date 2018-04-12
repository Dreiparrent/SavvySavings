using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreLocation;

namespace SavvySavings.iOS.Services
{
    public class DidEnterRegionEventArgs
    {
        CLRegion region;
        public CLRegion Region  { get { return region; } }

        public DidEnterRegionEventArgs(CLRegion region)
        {
            this.region = region;
        }
    }
}