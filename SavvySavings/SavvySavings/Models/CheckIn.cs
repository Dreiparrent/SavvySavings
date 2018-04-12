using System;
using System.Collections.Generic;
using System.Text;

namespace SavvySavings.Models
{
    public class CheckIn
    {
        public String Name { get; set; }
        public DateTime InDate { get; set; }
        public Double SpValue { get; set; }
        public Double Lng { get; set; }
        public Double Lat { get; set; }
        public String Id { get; set; }
    }
}
