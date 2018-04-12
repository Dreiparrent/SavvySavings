using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using SavvySavings.Models;

namespace SavvySavings.Services
{
    public class LocalFirebase
    {
        public static ILocalFirebase<Sale> Current;
    }
}
