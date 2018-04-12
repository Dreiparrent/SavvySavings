using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.RemoteConfig;
using Java.Lang;
using SavvySavings.Models;

namespace SavvySavings.Droid.Services
{
    public class GetRemoteConfig
    {
        public GetRemoteConfig()
        {
            var fbRemoteConfig = FirebaseRemoteConfig.Instance;

            //FirebaseRemoteConfig.Instance.SetDefaults(Resource.Xml.remote_config_defaults);
            IDictionary<string, Java.Lang.Object> dictionary = new Dictionary<string, Java.Lang.Object>()
            {
                {"SP1", "Beginner" }, 
                {"SP2", "Shopper" }, 
                {"SP3", "Saver" }, 
                {"SP4", "Not Set" }, 
                {"SP5", "Smart Saver" }, 
                {"SP6", "Sharp Saver" }, 
                {"SP7", "Savvy Starter" }, 
                {"SP8", "Savvy Shopper" }, 
                {"SP9", "Savvy Saver" }, 
                {"SP10", "Savvy Saver" },
                {"SPT1", "This is the description of what this level means." },
                {"SPT2", "Redeem for a 5% off cupon at the store of your choice." },
                {"SPT3", "This is the description of what this level means." },
                {"SPT4", "This is the description of what this level means." },
                {"SPT5", "This is the description of what this level means." },
                {"SPT6", "This is the description of what this level means." },
                {"SPT7", "This is the description of what this level means." },
                {"SPT8", "This is the description of what this level means." },
                {"SPT9", "This is the description of what this level means." },
                {"SPT10", "This is the description of what this level means." },
            };
            FirebaseRemoteConfig.Instance.SetDefaults(dictionary);
            SetLevels();
#if DEBUG
            long expirationDuration = 10;
#else
			long expirationDuration = 86400;
#endif
            FirebaseRemoteConfig.Instance.Fetch(expirationDuration)
                .AddOnSuccessListener(new RemoteSuccessListener())
                .AddOnFailureListener(new RemoteFailListener());
        }

        public static void SetLevels()
        {
            List<string> spLevels = new List<string>()
            {
                FirebaseRemoteConfig.Instance.GetString("SPL1"),
                FirebaseRemoteConfig.Instance.GetString("SPL2"),
                FirebaseRemoteConfig.Instance.GetString("SPL3"),
                FirebaseRemoteConfig.Instance.GetString("SPL4"),
                FirebaseRemoteConfig.Instance.GetString("SPL5"),
                FirebaseRemoteConfig.Instance.GetString("SPL6"),
                FirebaseRemoteConfig.Instance.GetString("SPL7"),
                FirebaseRemoteConfig.Instance.GetString("SPL8"),
                FirebaseRemoteConfig.Instance.GetString("SPL9"),
                FirebaseRemoteConfig.Instance.GetString("SPL10")
            };
            List<string> spText = new List<string>()
            {
                FirebaseRemoteConfig.Instance.GetString("SPT1"),
                FirebaseRemoteConfig.Instance.GetString("SPT2"),
                FirebaseRemoteConfig.Instance.GetString("SPT3"),
                FirebaseRemoteConfig.Instance.GetString("SPT4"),
                FirebaseRemoteConfig.Instance.GetString("SPT5"),
                FirebaseRemoteConfig.Instance.GetString("SPT6"),
                FirebaseRemoteConfig.Instance.GetString("SPT7"),
                FirebaseRemoteConfig.Instance.GetString("SPT8"),
                FirebaseRemoteConfig.Instance.GetString("SPT9"),
                FirebaseRemoteConfig.Instance.GetString("SPT10")
            };
            PointsModel.SPLevels = spLevels;
            PointsModel.SPText = spText;
        }
    }

    public class RemoteSuccessListener : Java.Lang.Object, Android.Gms.Tasks.IOnSuccessListener
    {

        public void OnSuccess(Java.Lang.Object result)
        {
            FirebaseRemoteConfig.Instance.ActivateFetched();
            GetRemoteConfig.SetLevels();
        }
    }

    public class RemoteFailListener : Java.Lang.Object, Android.Gms.Tasks.IOnFailureListener
    {
        public void OnFailure(Java.Lang.Exception e)
        {
            Console.WriteLine($"Connection Failed: {e}");
        }
    }
}