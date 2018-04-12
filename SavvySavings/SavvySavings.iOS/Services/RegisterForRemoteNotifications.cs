using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Firebase.Core;
using Firebase.CloudMessaging;
using NotificationCenter;
using UserNotifications;
using UserNotificationsUI;
using Foundation;
using UIKit;

namespace SavvySavings.iOS.Services
{
    public class RegisterForRemoteNotifications : IMessagingDelegate, IUNUserNotificationCenterDelegate
    {
        public RegisterForRemoteNotifications()
        {
            // Register your app for remote notifications.
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {

                // iOS 10 or later
                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) => {
                    Console.WriteLine("granted: {0}",granted);
                });

                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = this;

                // For iOS 10 data message (sent via FCM)
                Messaging.SharedInstance.Delegate = this;
            }
            else
            {
                // iOS 9 or before
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }
            UIApplication.SharedApplication.RegisterForRemoteNotifications();
        }

        public IntPtr Handle => throw new NotImplementedException();

        public void DidRefreshRegistrationToken(Messaging messaging, string fcmToken)
        {
            Console.WriteLine("FCM token = {0}", fcmToken);
        }

        // To receive notifications in foreground on iOS 10 devices.
        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Do your magic to handle the notification data
            Console.WriteLine("Notification Data {0}",notification.Request.Content.UserInfo);
        }

        // Receive data message on iOS 10 devices.
        public void ApplicationReceivedRemoteMessage(RemoteMessage remoteMessage)
        {
            Console.WriteLine("Remote Message: {0}",remoteMessage.AppData);
        }

        public void Dispose()
        {
            Console.WriteLine("Dispose");
            //throw new NotImplementedException();
        }
    }
}