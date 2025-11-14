using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileDartsApp.Models
{
    public class ThrowModel
    {
        public string PlayerName { get; set; }
        public int Scored { get; set; }
        public int CurrentScore { get; set; }
        public int ThrownDarts { get; set; }
        public int DoubleTries { get; set; }
        public bool CheckedOut { get; set; }
    }
}
