using System;
using Foundation;
using UserNotifications;
using Firebase.CloudMessaging;

namespace SavvySavings.iOS.Services
{
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate, IMessagingDelegate
    {
        #region Constructors
        public UserNotificationCenterDelegate()
        {

        }
        #endregion

        #region Override Methods
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Do something with the notification
            Console.WriteLine("Active Notification: {0}", notification);

            // Tell system to display the notification anyway or use
            // `None` to say we have handled the display locally.
            completionHandler(UNNotificationPresentationOptions.Alert);
        }
        #endregion

        public void ApplicationReceivedRemoteMessage(RemoteMessage message)
        {
            Console.WriteLine("Message {0}", message);
            //typeof(Messaging), typeof(RemoteMessage)
        }

        public void DidRefreshRegistrationToken(Messaging messaging, string fcmToken)
        {
            Console.WriteLine("FCM token = {0}", fcmToken);
        }
        public void DidReceiveMessage(Firebase.CloudMessaging.Messaging messaging, Firebase.CloudMessaging.RemoteMessage remoteMessage)
        {
            Console.WriteLine("message: {0}",remoteMessage.AppData);
        }
    }
}