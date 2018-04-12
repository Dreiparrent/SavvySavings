using System;
using System.Collections.Generic;
using System.Text;

namespace SavvySavings.Models
{
    public class PointsModel
    {
        private static List<string> spLevels;
        public static List<string> SPLevels
        {
            get { return spLevels; }
            set { spLevels = value; }
        }
        private static List<string> spText;
        public static List<string> SPText
        {
            get { return spText; }
            set { spText = value; }
        }
    }
}
