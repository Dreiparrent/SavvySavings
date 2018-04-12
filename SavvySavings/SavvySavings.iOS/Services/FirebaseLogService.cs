using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using SavvySavings.Services;
using Firebase.Analytics;

[assembly: Xamarin.Forms.Dependency(typeof(SavvySavings.iOS.Services.FirebaseLogService))]
namespace SavvySavings.iOS.Services
{
    class FirebaseLogService : ILocalFirebaseLog
    {
        public void LogToFirebase()
        {

        }

        public static void UserLoginSignup(String signupMethod, Boolean isSignup = false)
        {
            NSString[] keys = { ParameterNamesConstants.SignUpMethod };
            NSObject[] values = { new NSString(signupMethod) };
            var parameters = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(keys, values, keys.Length);
            if (isSignup)
            {
                Console.WriteLine($"Signup!!: {EventNamesConstants.SignUp}");
                Analytics.LogEvent(EventNamesConstants.SignUp, parameters);
            }
                
            else
                Analytics.LogEvent(EventNamesConstants.Login, parameters);
        }
    }
}