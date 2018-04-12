using System;
using System.Collections.Generic;
using System.Text;

namespace SavvySavings.Models
{
    public class RemConfigDefaults
    {
        public string SPL1 = "Beginner";
        public string SPL2 = "Browser";
        public string SPL3 = "Shopper";
        public string SPL4 = "Saver";
        public string SPL5 = "Smart Saver";
        public string SPL6 = "Sharp Saver";
        public string SPL7 = "Savvy Starter";
        public string SPL8 = "Savvy Shopper";
        public string SPL9 = "Savvy Saver";
        public string SPL10 = "Savvy Saver";
        public List<string> spLevels;

        public string SPT1 = "Cash 25SP in return for 50SP for each new friend invited.";
        public string SPT2 = "Redeem for a chance to win any 10% off coupon";
        public string SPT3 = "Redeem for a 5% off cupon at the store of your choice.";
        public string SPT4 = "This is the description of what this level means.";
        public string SPT5 = "This is the description of what this level means.";
        public string SPT6 = "This is the description of what this level means.";
        public string SPT7 = "Gain acess to Savvy Savings exclusive cupons.";
        public string SPT8 = "This is the description of what this level means.";
        public string SPT9 = "This is the description of what this level means.";
        public string SPT10 = "This is the description of what this level means.";
        public List<string> spText;

        public RemConfigDefaults()
        {
            spLevels = new List<string> { SPL1,SPL2,SPL3,SPL4,SPL5,SPL6,SPL7,SPL8,SPL9,SPL10 };
            spText = new List<string> { SPT1, SPT2, SPT3, SPT4, SPT5, SPT6, SPT7, SPT8, SPT9, SPT10 };
        }
    }
}
