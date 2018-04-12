using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SavvySavings.Views;
using SavvySavings.Models;
using SavvySavings.Services;
using Java.Util;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

[assembly: Xamarin.Forms.Dependency(typeof(SavvySavings.Droid.Services.AccountService))]
namespace SavvySavings.Droid.Services
{
    public class AccountService: ILocalAuth, IWatchItemStore<WatchItem>, ICheckInStore<CheckIn>
    {
        public static List<WatchItem> items;
        public List<WatchItem> Items { get { return items; } }
        public static List<CheckIn> checkIns;
        public List<CheckIn> CheckIns { get { return checkIns; } }

        static Boolean isAuth;
        public Boolean IsAuth { get { return isAuth; } }
        public FirebaseDatabase firData;
        public DatabaseReference dataRef;
        public DatabaseReference watchDir;
        public DatabaseReference checkDir;
        public static Account authAccount;
        public Account AuthAccount { get { return authAccount; } }

        public AccountService()
        {
            //Auth Stuff
            firData = FirebaseDatabase.GetInstance(FirebaseApp.Instance);
            //var firInstance = FirebaseAuth.GetInstance(FirebaseApp.Instance);
            FirebaseAuth.Instance.AuthState += AuthStateChanged;
            /*
            FirebaseAuth.Instance.AuthState += (sender, e) =>
            {
                var user = e?.Auth?.CurrentUser;

                if (user != null)
                {
                    isAuth = true;
                    FirebaseLogService.UserLoginSignup("Email");
                    SetUserData(user);
                }
                else
                {
                    isAuth = false;
                }
            };
            */
            //Auth Data
            items = new List<WatchItem>();
            checkIns = new List<CheckIn>();
        }

        public void AuthStateChanged(object sender, FirebaseAuth.AuthStateEventArgs e)
        {
            var user = e?.Auth?.CurrentUser;

            if (user != null)
            {
                isAuth = true;
                FirebaseLogService.UserLoginSignup("Email");
                SetUserData(user);
            }
            else
            {
                isAuth = false;
            }
        }

        public void DetachListener()
        {
            //FirebaseAuth.Instance.AuthState -= AuthStateChanged;
        }

        public Boolean GetAuthState()
        {
            var user = FirebaseAuth.Instance.CurrentUser;
            if (user != null)
            {
                isAuth = true;
                SetUserData(user);
            }
            else
            {
                isAuth = false;
            }
            return IsAuth;
        }

        public void SetUserData(FirebaseUser user)
        {
            var uid = user.Uid;
            var name = user.DisplayName;
            var email = user.Email;
            var emailVerified = user.IsEmailVerified;
            var photoUrl = user.PhotoUrl;

            authAccount = new Account
            {
                Uid = uid,
                Name = name,
                Email = email,
                EmailVerified = emailVerified
            };

            dataRef = firData.GetReference($"users/{uid}");

            IDictionary<string, Java.Lang.Object> userData = new Dictionary<string, Java.Lang.Object>
            {
                {"name",name },
                {"email", email }
            };
            dataRef.UpdateChildren(userData);

            if (watchDir == null)
            {
                dataRef.AddChildEventListener(new AcctListener());
                watchDir = dataRef.Child("watchItems");
                watchDir.AddChildEventListener(new WatchListener());
                checkDir = dataRef.Child("checkIns");
                checkDir.AddListenerForSingleValueEvent(new CheckinListener());
            }
        }

        public void SetAccountData(String name, String birthday, int radius, bool sendVerification)
        {
            if (authAccount == null)
                authAccount = new Account();
            authAccount.Name = name;
            authAccount.Birthday = Convert.ToDateTime(birthday);
            authAccount.SalesRadius = radius;
            var user = FirebaseAuth.Instance.CurrentUser;
            var profileUpdates = new UserProfileChangeRequest.Builder().SetDisplayName(name).Build();
            user.UpdateProfile(profileUpdates);

            IDictionary<string, Java.Lang.Object> accountData = new Dictionary<string, Java.Lang.Object>
            {
                { "name", name },
                { "birthday", birthday },
                { "radius", radius }
            };
            if (dataRef == null)
                dataRef = firData.GetReference($"users/{FirebaseAuth.Instance.CurrentUser.Uid}");
            dataRef.UpdateChildren(accountData);
            if (sendVerification)
                SendEmailVerification();
        }

        public async Task<Boolean> CheckVerification()
        {
            await FirebaseAuth.Instance.CurrentUser.ReloadAsync();
            return FirebaseAuth.Instance.CurrentUser.IsEmailVerified;
        }

        public void SendEmailVerification()
        {
            var user = FirebaseAuth.Instance.CurrentUser;
            user.SendEmailVerification().AddOnFailureListener(new EmailFailureListener(
                    () => Console.WriteLine("Email Failed to Send"))
                ).AddOnSuccessListener(new EmailSuccessListener(
                    () => Console.WriteLine("Email Sent")));
        }

        public async Task<Boolean> SignIAsync(string email, string password)
        {
            try
            {
                await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
                FirebaseLogService.UserLoginSignup("Email");
                return true;
            }
            catch (Exception ex)
            {
                // Sign-in failed, display a message to the user
                // If sign in succeeds, the AuthState event handler will
                //  be notified and logic to handle the signed in user can happen there
                //TODO: use firebase log for this
                var exx = ex;
                //Toast.MakeText(this, "Sign In failed", ToastLength.Short).Show();
                return false;
            }
        }

        public async Task<Boolean> CreateAccountAsync(string email, string password)
        {
            try
            {
                await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password);
                FirebaseLogService.UserLoginSignup("Email",true);
                return true;
            }
            catch (Exception ex)
            {
                // Sign-up failed, display a message to the user
                // If sign in succeeds, the AuthState event handler will
                var exx = ex;
                //Toast.MakeText(this, "Sign In failed", ToastLength.Short).Show();
                return false;
            }
        }

        public Boolean Logout()
        {
            try
            {
                FirebaseAuth.Instance.SignOut();
                return true;
            }
            catch (Exception ex)
            {
                // Sign-up failed, display a message to the user
                // If sign in succeeds, the AuthState event handler will
                var exx = ex;
                //TODO: firebase log error
                //Toast.MakeText(this, "Sign In failed", ToastLength.Short).Show();
                return false;
            }
        }

        public async Task<bool> DeleteItemAsync(WatchItem item)
        {
            await watchDir.Child(item.Id).RemoveValueAsync();
            var _item = items.Where((WatchItem arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(_item);
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<WatchItem>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(Items);
        }

        public async Task<bool> AddItemAsync(WatchItem item)
        {
            if (dataRef == null)
                dataRef = firData.GetReference($"users/{FirebaseAuth.Instance.CurrentUser.Uid}");


            var watchRef = dataRef.Child("watchItems");
            await watchRef.Push().SetValueAsync(item.Name);
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(WatchItem item)
        {
            if(dataRef == null)
                dataRef = firData.GetReference($"users/{FirebaseAuth.Instance.CurrentUser.Uid}");
            var watchRef = dataRef.Child("watchItems");

            await watchRef.Child(item.Id).SetValueAsync(item.Name);
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<CheckIn>> GetCheckInsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(CheckIns);
        }
    }

    public class AcctListener : Java.Lang.Object, IChildEventListener
    {
        public void OnCancelled(DatabaseError error)
        {
            //TODO: maybe log error here?
        }

        public void OnChildAdded(DataSnapshot snapshot, string previousChildName)
        {
            var key = snapshot.Key;
            var snapVal = snapshot.Value.ToString();
            Console.WriteLine($"Account Dir: {snapVal}");
            switch (key)
            {
                case "birthday":
                    AccountService.authAccount.Birthday = Convert.ToDateTime(snapVal);
                    break;
                case "email":
                    AccountService.authAccount.Email = snapVal;
                    break;
                case "name":
                    AccountService.authAccount.Name = snapVal;
                    break;
                case "radius":
                    AccountService.authAccount.SalesRadius = double.Parse(snapVal);
                    break;
            }
                    
        }

        public void OnChildChanged(DataSnapshot snapshot, string previousChildName)
        {
            
        }

        public void OnChildMoved(DataSnapshot snapshot, string previousChildName)
        {
            
        }

        public void OnChildRemoved(DataSnapshot snapshot)
        {
           
        }
    }

    public class CheckinListener : Java.Lang.Object, IValueEventListener
    {

        public void OnCancelled(DatabaseError error) { }

        public void OnDataChange(DataSnapshot snapshot)
        {
            foreach (DataSnapshot child in snapshot.Children.ToEnumerable())
            {
                CheckObj checkIn = (CheckObj)child;
                AccountService.checkIns.Add(checkIn);
                Console.WriteLine($"Acct: {AccountService.checkIns}");
            }
        }
    }

    class CheckObj : CheckIn
    {
        public static explicit operator CheckObj(DataSnapshot snapshot)
        {
            var retCheck = new CheckObj
            {
                InDate = Convert.ToDateTime(snapshot.Child("inDate").Value.ToString()),
                Lat = (double)snapshot.Child("lat").Value,
                Lng = (double)snapshot.Child("lng").Value,
                Name = snapshot.Child("name").Value.ToString(),
                SpValue = (double)snapshot.Child("spValue").Value
            };
            return retCheck;
        }
    }

    public class WatchListener : Java.Lang.Object, IChildEventListener
    {
        public void OnCancelled(DatabaseError error)
        {
            //TODO: maybe log error here?
        }

        public void OnChildAdded(DataSnapshot snapshot, string previousChildName)
        {
            var data = new WatchItem
            {
                Name = snapshot.Value.ToString(),
                Id = snapshot.Key
            };
            AccountService.items.Add(data);
        }

        public void OnChildChanged(DataSnapshot snapshot, string previousChildName)
        {
            var data = new WatchItem
            {
                Name = snapshot.Value.ToString(),
                Id = snapshot.Key
            };
            var _item = AccountService.items.Where((WatchItem arg) => arg.Id == data.Id).FirstOrDefault();
            var changIndex = AccountService.items.IndexOf(_item);
            AccountService.items[changIndex] = data;
        }

        public void OnChildMoved(DataSnapshot snapshot, string previousChildName)
        {
            var data = new WatchItem
            {
                Name = snapshot.Value.ToString(),
                Id = snapshot.Key
            };
            var _item = AccountService.items.Where((WatchItem arg) => arg.Id == previousChildName).FirstOrDefault();
            var changIndex = AccountService.items.IndexOf(_item);
            AccountService.items[changIndex] = data;
        }

        public void OnChildRemoved(DataSnapshot snapshot)
        {
            var data = new WatchItem
            {
                Name = snapshot.Value.ToString(),
                Id = snapshot.Key
            };
            AccountService.items.Remove(data);
        }
    }

    public class EmailFailureListener : Java.Lang.Object, Android.Gms.Tasks.IOnFailureListener
    {
        Action action;
        public EmailFailureListener(Action action) { this.action = action; }
        public void OnFailure(Java.Lang.Exception e)
        {
            //TODO: log to firebase;
            action();
        }
    }

    public class EmailSuccessListener : Java.Lang.Object, Android.Gms.Tasks.IOnSuccessListener
    {
        Action action;
        public EmailSuccessListener(Action action) { this.action = action; }
        public void OnSuccess(Java.Lang.Object result)
        {
            this.action();
        }
    }
}