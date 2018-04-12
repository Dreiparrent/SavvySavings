using System;

namespace SavvySavings.Models
{
    public class Account
    {
        public string Uid { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public double SalesRadius { get; set; }
    }
}
