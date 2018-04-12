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

using Firebase.Analytics;
using SavvySavings.Services;

[assembly: Xamarin.Forms.Dependency(typeof(SavvySavings.Droid.Services.FirebaseLogService))]
namespace SavvySavings.Droid.Services
{
    public class FirebaseLogService : ILocalFirebaseLog
    {
        public static FirebaseAnalytics analytics;
        public FirebaseLogService(FirebaseAnalytics firebaseAnalytics)
        {
            analytics = firebaseAnalytics;
        }

        public void LogToFirebase()
        {
            var bundle = new Bundle();
            //bundle.PutString(FirebaseAnalytics.Param.);
        }

        public static void UserLoginSignup(String signupMethod, Boolean isSignup = false)
        {
            var bundle = new Bundle();
            bundle.PutString(FirebaseAnalytics.Param.SignUpMethod,signupMethod);
            if (analytics == null)
                analytics = FirebaseAnalytics.GetInstance(Application.Context);
            if (isSignup)
                analytics.LogEvent(FirebaseAnalytics.Event.SignUp, bundle);
            else
                analytics.LogEvent(FirebaseAnalytics.Event.Login, bundle);
        }
    }
}