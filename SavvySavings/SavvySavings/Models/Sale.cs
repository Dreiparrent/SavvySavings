using System;
using System.Collections.Generic;
using System.Text;

namespace SavvySavings.Models
{
    public class Sale
    {
        public string Begin { get; set; }
        public string End { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        //public string[] Tags { get; set; }
    }
}
