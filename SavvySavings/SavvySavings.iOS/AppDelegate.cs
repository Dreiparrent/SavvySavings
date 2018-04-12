using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using SavvySavings.iOS.Services;
using UIKit;
using UserNotifications;
using Firebase.Core;
using Firebase.InstanceID;
using Firebase.CloudMessaging;
using Firebase.RemoteConfig;
using SavvySavings.Models;
using CoreLocation;

namespace SavvySavings.iOS
{
    
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IUNUserNotificationCenterDelegate, IMessagingDelegate {
        #region Main App Delegate
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public UserNotificationCenterDelegate notificationCenterDelegate;
        public String senID;
        public NSData DeviceToken;
        public LocationManager Manager;
        public CLLocation curLocation;
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            Firebase.Core.App.Configure();
            senID = "1076235234949";
            #region Check For Notifications
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // iOS 10
                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
                {
                    //Console.WriteLine(granted);
                });

                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = this;
            }
            else
            {
                // iOS 9 <=
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }
            UIApplication.SharedApplication.RegisterForRemoteNotifications();
            #endregion

            //Map and final
            Xamarin.FormsMaps.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
        #endregion

        #region Remote Notifications
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            DeviceToken = deviceToken;
            Messaging.SharedInstance.Delegate = this;
            StartLocationServices();
            StartRemotConfig();
            
#if DEBUG
            Messaging.SharedInstance.SetApnsToken(deviceToken, Firebase.CloudMessaging.ApnsTokenType.Sandbox);
#else
			Messaging.SharedInstance.SetApnsToken(deviceToken, Firebase.CloudMessaging.ApnsTokenType.Production);
#endif
            Messaging.SharedInstance.ShouldEstablishDirectChannel = true;
            Console.WriteLine("Token: {0}",Messaging.SharedInstance.FcmToken);
        }
        #endregion
        #region Notification Token Helpers
        void GetToken()
        {
            Messaging.SharedInstance.RetrieveFcmToken(senID, (token, error) => 
            Console.WriteLine($"Retrieved: {token} : {error}"));
        }

        public void DidRefreshRegistrationToken(Messaging messaging, string fcmToken)
        {
            Console.WriteLine($"fcmToken {fcmToken}");
        }
        #endregion
        #region Recieve Notification
        // iOS 9 <=, fire when recieve notification foreground
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            Messaging.SharedInstance.AppDidReceiveMessage(userInfo);

            // Generate custom event
            NSString[] keys = { new NSString("Event_type") };
            NSObject[] values = { new NSString("Recieve_Notification") };
            var parameters = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(keys, values, keys.Length);

            // Send custom event
            //Firebase.Analytics.Analytics.LogEvent("CustomEvent", parameters);

            if (application.ApplicationState == UIApplicationState.Active)
            {
                var aps_d = userInfo["aps"] as NSDictionary;
                var alert_d = aps_d["alert"] as NSDictionary;
                try
                {
                    NSNumberFormatter formatter = new NSNumberFormatter
                    {
                        NumberStyle = NSNumberFormatterStyle.Decimal
                    };
                    var mLat = userInfo["lat"] as NSString;
                    var mLng = userInfo["lng"] as NSString;
                    //NSNumber lat = (formatter.NumberFromString(mLat));
                    //NSNumber lng = (formatter.NumberFromString(mLng));
                    double lat = Convert.ToDouble(mLat);
                    double lng = Convert.ToDouble(mLng);
                    CLLocation notifLoc = new CLLocation(lat, lng);
                    var distance = notifLoc.DistanceFrom(curLocation);
                    Console.WriteLine($"Distance: {distance}");
                    //Console.WriteLine($"lat: {lat}, lng:{lng}");
                } catch (NullReferenceException e)
                {
                    //TODO: log this maybe?
                    Console.WriteLine(e);
                }

                try
                {
                    var body = alert_d["body"] as NSString;
                    var title = alert_d["title"] as NSString;
                    DebugAlert(title, body);
                } catch (NullReferenceException)
                {
                    //TODO: here?
                }
            }
            completionHandler(UIBackgroundFetchResult.NewData);
        }

        // iOS 10, fire when recieve notification foreground
        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            var userInfo = notification.Request.Content.UserInfo;
            Console.WriteLine("New Notif: {0}", userInfo);
            var title = notification.Request.Content.Title;
            var body = notification.Request.Content.Body;
            DebugAlert(title, body);
        }

        private void DebugAlert(string title, string message)
        {
            //var alert = new UIAlertView(title ?? "Title", message ?? "Message", null, "Cancel", "OK");
            var alert = new UIAlertView()
            {
                Title = title ?? "Title",
                Message = message ?? "Message"
            };
            alert.AddButton("Ok");
            alert.Show();
        }
        #endregion

        #region Location Services
        void StartLocationServices()
        {
            Manager = new LocationManager(200);
            Manager.StartLocationUpdates();
            Manager.LocationUpdated += HandleLocationChanged;
        }

        public void HandleLocationChanged(object sender, LocationUpdatedEventArgs e)
        {
            CLLocation location = e.Location;
            curLocation = e.Location;
            string lng = location.Coordinate.Longitude.ToString();
            string lat = location.Coordinate.Latitude.ToString();
            //Console.WriteLine($"Location Updated: {lng},{lat}");

        }
        #endregion

        #region Remote Config
        void StartRemotConfig()
        {
            RemoteConfigSettings sets = new RemoteConfigSettings(true);
            RemoteConfig.SharedInstance.ConfigSettings = new RemoteConfigSettings(true);
            //RemConfigDefaults defaults = new RemConfigDefaults();
            RemoteConfig.SharedInstance.SetDefaults("remote_config_defaults");
            SetLevels();
#if DEBUG
            double expirationDuration = 10;
#else
			double expirationDuration = 86400;
#endif
            RemoteConfig.SharedInstance.Fetch(expirationDuration, (status, error) =>
            {
                if (error != null)
                {
                    Console.WriteLine(error);
                }
                switch (status)
                {
                    case RemoteConfigFetchStatus.Success:
                        RemoteConfig.SharedInstance.ActivateFetched();
                        SetLevels();
                        break;
                    case RemoteConfigFetchStatus.Throttled:
                    case RemoteConfigFetchStatus.NoFetchYet:
                    case RemoteConfigFetchStatus.Failure:
                        Console.WriteLine("Config not fetched...");
                        break;
                }
            });
        }

        public void SetLevels()
        {
            List<string> spLevels = new List<string>()
            {
                RemoteConfig.SharedInstance.GetConfigValue("SPL1").StringValue,
                RemoteConfig.SharedInstance.GetConfigValue("SPL2").StringValue,
                RemoteConfig.SharedInstance.GetConfigValue("SPL3").StringValue,
                RemoteConfig.SharedInstance.GetConfigValue("SPL4").StringValue,
                RemoteConfig.SharedInstance.GetConfigValue("SPL5").StringValue,
                RemoteConfig.SharedInstance.GetConfigValue("SPL6").StringValue,
                RemoteConfig.SharedInstance.GetConfigValue("SPL7").StringValue,
                RemoteConfig.SharedInstance.GetConfigValue("SPL8").StringValue,
                RemoteConfig.SharedInstance.GetConfigValue("SPL9").StringValue,
                RemoteConfig.SharedInstance.GetConfigValue("SPL10").StringValue
            };
            List<string> spText = new List<string>()
            {
                RemoteConfig.SharedInstance.GetConfigValue("SPT1").StringValue,
                RemoteConfig.SharedInstance.GetConfigValue("SPT2").StringValue,
                RemoteConfig.SharedInstance.GetConfigValue("SPT3").StringValue,
                RemoteConfig.SharedInstance.GetConfigValue("SPT4").StringValue,
                RemoteConfig.SharedInstance.GetConfigValue("SPT5").StringValue,
                RemoteConfig.SharedInstance.GetConfigValue("SPT6").StringValue,
                RemoteConfig.SharedInstance.GetConfigValue("SPT7").StringValue,
                RemoteConfig.SharedInstance.GetConfigValue("SPT8").StringValue,
                RemoteConfig.SharedInstance.GetConfigValue("SPT9").StringValue,
                RemoteConfig.SharedInstance.GetConfigValue("SPT10").StringValue
            };
            PointsModel.SPLevels = spLevels;
            PointsModel.SPText = spText;
        }
        #endregion
        #region foreground background
        public override void WillEnterForeground(UIApplication uiApplication)
        {
            if (DeviceToken != null)
            {
                Messaging.SharedInstance.ShouldEstablishDirectChannel = true;
            }
            Manager.acuracy = 200;
        }
        public override void DidEnterBackground(UIApplication uiApplication)
        {
            Manager.acuracy = 600;
            Messaging.SharedInstance.ShouldEstablishDirectChannel = false;
        }
        #endregion
    }
}
