using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using System.Net.Mail;

using SavvySavings.Services;
#if __ANDROID__
using SavvySavings.Droid.Services;
#endif
#if __IOS__
using SavvySavings.iOS.Services;
#endif

namespace SavvySavings.ViewModels
{
    public class LogRegViewModel
    {
        public ILocalAuth AuthStore => DependencyService.Get<AccountService>() ?? new AccountService();

        public Command LoadSigninCommand { get; set; }
        public String EmailAddress;
        public String Password;
        public String Pass2;

        public LogRegViewModel()
        {
            EmailAddress = Password = "";
            Pass2 = " ";
        }

        public bool IsValid(string emailAddress = null, string password = null, string pass2 = null)
        {
            EmailAddress = (emailAddress == null) ? EmailAddress : emailAddress;
            Password = (password == null) ? Password : password;
            Pass2 = (pass2 == null) ? Pass2 : pass2;
            if (EmailAddress.Length > 0 && Password.Length > 0 && Pass2.Length > 0)
            {
                try
                {
                    MailAddress m = new MailAddress(EmailAddress);

                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
            }
            else
            return false;
        }
    }
}
