using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SavvySavings.Models;

namespace SavvySavings.Services
{
    public interface ILocalAuth
    {
        Boolean IsAuth { get; }
        Boolean GetAuthState();
        void DetachListener();
        Account AuthAccount { get; }
        Task<Boolean> CheckVerification();
        void SetAccountData(String name, String birthday, int radius, bool sendVerification);
        Task<Boolean> SignIAsync(string email, string password);
        Task<Boolean> CreateAccountAsync(string email, string password);
        void SendEmailVerification();
        Boolean Logout();
    }
}
