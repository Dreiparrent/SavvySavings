using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Foundation;
using UIKit;
using SavvySavings.Services;
using Firebase.Auth;
using Firebase.Database;
using SavvySavings.Models;
using Firebase.Analytics;

[assembly: Xamarin.Forms.Dependency(typeof(SavvySavings.iOS.Services.AuthService))]
namespace SavvySavings.iOS.Services
{
    public class AuthService : ILocalAuth
    {
        NSObject handle;
        static Boolean isAuth;
        public Boolean IsAuth { get { return isAuth; } }
        Database firData;
        DatabaseReference dataRef;
        public static Account authAccount;
        public Account AuthAccount { get { return authAccount; } }

        public AuthService()
        {
            handle = Auth.DefaultInstance.AddAuthStateDidChangeListener((Auth auth, User user) => {
                if (user != null)
                {
                    isAuth = true;
                    FirebaseLogService.UserLoginSignup("Email");
                    SetUserData(user);
                }
            });
            firData = Database.From(Firebase.Core.App.DefaultInstance);
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
            var photoUrl = user.PhotoUrl;

            authAccount = new Account
            {
                Uid = uid,
                Name = name,
                Email = email
            };

            object[] keys = { "name", "email" };
            object[] values = { name, email };
            var userData = NSDictionary.FromObjectsAndKeys(values, keys, keys.Length);
            dataRef = firData.GetReferenceFromPath($"users/{uid}");
            dataRef.UpdateChildValues(userData);
        }

        public void SetAccountData(String name, String birthday, int radius)
        {
            if(dataRef == null)
                dataRef = firData.GetReferenceFromPath($"users/{Auth.DefaultInstance.CurrentUser.Uid}");             

            object[] keys = { "name", "birthday", "radius" };
            object[] values = { name, birthday, radius };
            var accountData = NSDictionary.FromObjectsAndKeys(values, keys, keys.Length);
            dataRef.UpdateChildValues(accountData);
            var user = Auth.DefaultInstance.CurrentUser;
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
    }
}