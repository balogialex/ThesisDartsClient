using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileDartsApp.Models
{
    public class StatisticsModel
    {
        public double Average { get; set; }
        public double First9Average { get; set; }
        public double CheckoutPercent { get; set; }
        public int ThrownDarts { get; set; }
        public int TotalScored { get; set; }
        public int TotalFirst9Scored { get; set; }
        public int CheckoutCount { get; set; }
        public int CheckoutTries { get; set; }
        public int HighestCheckout { get; set; }
        public int HighestThrow { get; set; }
    }
}
