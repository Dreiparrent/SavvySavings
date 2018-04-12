using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using SavvySavings.Models;
using SavvySavings.Services;
using SavvySavings.Views;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

[assembly: Xamarin.Forms.Dependency(typeof(SavvySavings.iOS.Services.AccountService))]
namespace SavvySavings.iOS.Services
{
    public class AccountService: ILocalAuth, IWatchItemStore<WatchItem>, ICheckInStore<CheckIn>
    {
        NSObject handle;
        static Boolean isAuth;
        public Boolean IsAuth { get { return isAuth; } }
        Database firData;
        DatabaseReference dataRef;
        DatabaseReference checkDir;
        DatabaseReference watchDir;
        public static Account authAccount;
        public Account AuthAccount { get { return authAccount; } }

        List<WatchItem> items;
        List<CheckIn> checkIns;

        public AccountService()
        {
            //Auth init
            handle = Auth.DefaultInstance.AddAuthStateDidChangeListener((Auth auth, User user) => {
                if (user != null)
                {
                    isAuth = true;
                    FirebaseLogService.UserLoginSignup("Email");
                    Console.WriteLine("Auth State CHANGE!!!");
                    SetUserData(user);
                }
            });

            firData = Database.From(Firebase.Core.App.DefaultInstance);

            //Auth Data
            items = new List<WatchItem>();
            checkIns = new List<CheckIn>();            
        }

        public void DetachListener()
        {
            Auth.DefaultInstance.RemoveAuthStateDidChangeListener(handle);
        }

        public Boolean GetAuthState()
        {
            var user = Auth.DefaultInstance.CurrentUser;
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

        void SetUserData(User user)
        {
            var uid = user.Uid;
            var name = user.DisplayName;
            var email = user.Email;
            var verifyEmail = user.IsEmailVerified;
            var photoUrl = user.PhotoUrl;

            authAccount = new Account
            {
                Uid = uid,
                Name = name,
                Email = email,
                EmailVerified = verifyEmail
            };
            object[] keys = { "name", "email" };
            object[] values = { name, email };
            var userData = NSDictionary.FromObjectsAndKeys(values, keys, keys.Length);

            dataRef = firData.GetReferenceFromPath($"users/{uid}");
            dataRef.UpdateChildValues(userData);
            if (watchDir == null)
            {
                CreateObserver();
            }
        }

        public async Task<Boolean> CheckVerification()
        {
            await Auth.DefaultInstance.CurrentUser.ReloadAsync();
            return Auth.DefaultInstance.CurrentUser.IsEmailVerified;
        }

        void CreateObserver()
        {
            dataRef.ObserveEvent(DataEventType.ChildAdded, (snapshot) =>
            {
                var key = snapshot.Key;
                var snapVal = snapshot.GetValue().ToString();
                switch (key)
                {
                    case "birthday":
                        authAccount.Birthday = Convert.ToDateTime(snapVal);
                        break;
                    case "email":
                        authAccount.Email = snapVal;
                        break;
                    case "name":
                        authAccount.Name = snapVal;
                        break;
                    case "radius":
                        authAccount.SalesRadius = double.Parse(snapVal);
                        break;
                }
            });


            checkDir = dataRef.GetChild("checkIns");
            checkDir.ObserveSingleEvent(DataEventType.Value, (snapshot) => {
                NSEnumerator children = snapshot.Children;

                var child = children.NextObject() as DataSnapshot;

                while (child != null)
                {
                    var data = (CheckObj)child.GetValue<NSDictionary>();
                    data.Id = child.Key;
                    checkIns.Add(data);

                    child = children.NextObject() as DataSnapshot;
                }
            }, (error) =>
            {
                Console.WriteLine(error);
            });

            foreach (CheckIn check in checkIns)
            {
                object[] keys = { "name", "inDate", "spValue", "lat", "lng" };
                object[] values = {
                    check.Name,
                    check.InDate.ToString(),
                    check.SpValue,
                    check.Lat,
                    check.Lng
                };
                var checkData = NSDictionary.FromObjectsAndKeys(values, keys, keys.Length);
                //checkDir.UpdateChildValues(checkData);
            }

            watchDir = dataRef.GetChild("watchItems");

            nuint handleAdd = watchDir.ObserveEvent(DataEventType.ChildAdded, (snapshot) => {
                var data = new WatchItem
                {
                    Name = snapshot.GetValue().ToString(),
                    Id = snapshot.Key
                };
                items.Add(data);
            }, (error) =>
            {
                Console.WriteLine(error);
            });

            nuint handleChenge = watchDir.ObserveEvent(DataEventType.ChildChanged, (snapshot) =>
            {
                var data = new WatchItem
                {
                    Name = snapshot.GetValue().ToString(),
                    Id = snapshot.Key
                };
                var _item = items.Where((WatchItem arg) => arg.Id == data.Id).FirstOrDefault();
                var itemIndex = items.IndexOf(_item);
                items[itemIndex] = data;

            }, (error) => {


            });

            nuint handleDelete = watchDir.ObserveEvent(DataEventType.ChildRemoved, (snapshot) => {
                var data = new WatchItem
                {
                    Name = snapshot.GetValue().ToString(),
                    Id = snapshot.Key
                };
                items.Remove(data);
            }, (error) =>
            {
                Console.WriteLine(error);
            });
        }

        public void SetAccountData(String name, String birthday, int radius, bool sendVerification)
        {
            if (authAccount == null)
                authAccount = new Account();
            authAccount.Name = name;
            authAccount.Birthday = Convert.ToDateTime(birthday);
            authAccount.SalesRadius = radius;
            object[] keys = { "name", "birthday", "radius" };
            object[] values = { name, birthday, radius };
            var accountData = NSDictionary.FromObjectsAndKeys(values, keys, keys.Length);
            //dataRef.SetValue<NSDictionary>(accountData);
            var user = Auth.DefaultInstance.CurrentUser;
            if (dataRef == null)
                dataRef = firData.GetReferenceFromPath($"users/{user.Uid}");
            dataRef.UpdateChildValues(accountData);
            var changeRequest = user.ProfileChangeRequest();
            changeRequest.DisplayName = name;
            changeRequest.CommitChanges((NSError error) =>
            {
                if (error != null)
                {
                    //TODO: make this firebase log
                    // An error happened.
                    return;
                }
                // Profile updated.
            });
            if (sendVerification)
                SendEmailVerification();
        }

        public void SendEmailVerification()
        {
            var user = Auth.DefaultInstance.CurrentUser;
            user.SendEmailVerification((error) =>
            {
                if (error != null)
                {
                    AuthErrorCode errorCode;
                    if (IntPtr.Size == 8) // 64 bits devices
                        errorCode = (AuthErrorCode)((long)error.Code);
                    else // 32 bits devices
                        errorCode = (AuthErrorCode)((int)error.Code);

                    // Posible error codes that SendEmailVerification method could throw
                    // Visit https://firebase.google.com/docs/auth/ios/errors for more information
                    switch (errorCode)
                    {
                        case AuthErrorCode.UserNotFound:
                        default:
                            //TODO: log to firebase
                            // Print error
                            break;
                    }

                    return;
                }
                else
                {
                    //Email sent
                }
            });
        }

        public async Task<Boolean> SignIAsync(string email, string password)
        {
            try
            {
                User user = await Auth.DefaultInstance.SignInAsync(email, password);
                SetUserData(user);
                return true;
            }
            catch (NSErrorException ex)
            {
                //TODO: log this with firebase
                var exx = ex;
                return false;
            }
        }

        public async Task<Boolean> CreateAccountAsync(string email, string password)
        {
            try
            {
                User user = await Auth.DefaultInstance.CreateUserAsync(email, password);
                FirebaseLogService.UserLoginSignup("Email", true);
                return true;
            }
            catch (NSErrorException ex)
            {
                //TODO: log this with firebase
                var exx = ex;
                return false;
            }
        }

        public Boolean Logout()
        {
            NSError error;
            var signedOut = Auth.DefaultInstance.SignOut(out error);

            if (!signedOut)
            {
                AuthErrorCode errorCode;
                if (IntPtr.Size == 8) // 64 bits devices
                    errorCode = (AuthErrorCode)((long)error.Code);
                else // 32 bits devices
                    errorCode = (AuthErrorCode)((int)error.Code);

                // Posible error codes that SignOut method with credentials could throw
                // Visit https://firebase.google.com/docs/auth/ios/errors for more information
                switch (errorCode)
                {
                    case AuthErrorCode.KeychainError:
                    default:
                        //TODO: firebase log error
                        // Print error
                        break;
                }
                return false;
            }
            else
                return true;

        }

        public async Task<bool> DeleteItemAsync(WatchItem item)
        {
            //Todo: make this error check
            watchDir.GetChild(item.Id).RemoveValue((NSError error, DatabaseReference reference) =>
            {
                if (error != null)
                {
                    //Todo: make this error check
                }
            });
            var _item = items.Where((WatchItem arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(_item);
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<WatchItem>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }

        public async Task<bool> AddItemAsync(WatchItem item)
        {
            if (dataRef == null)
                dataRef = firData.GetReferenceFromPath($"users/{Auth.DefaultInstance.CurrentUser.Uid}");
            var watchRef = dataRef.GetChild("watchItems");

            watchRef.GetChildByAutoId().SetValue<NSString>((NSString)item.Name);
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(WatchItem item)
        {
            if(dataRef == null)
                dataRef = firData.GetReferenceFromPath($"users/{Auth.DefaultInstance.CurrentUser.Uid}");
            var watchRef = dataRef.GetChild("watchItems");

            watchRef.GetChild(item.Id).SetValue<NSString>((NSString)item.Name);
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<CheckIn>> GetCheckInsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(checkIns);
        }
    }

    class CheckObj : CheckIn
    {
        public static explicit operator CheckObj(NSDictionary dictionary)
        {
            var retSale = new CheckObj
            {
                InDate = Convert.ToDateTime(dictionary["inDate"].ToString()),
                Lat = (double)(NSNumber)dictionary["lat"],
                Lng = (double)(NSNumber)dictionary["lng"],
                Name = (NSString)dictionary["name"].ToString(),
                SpValue = (double)(NSNumber)dictionary["spValue"]
            };

            return retSale;
        }
    }
}