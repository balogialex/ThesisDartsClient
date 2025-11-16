using MobileDartsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileDartsApp.Models
{
    public class LegModel
    {
        public List<ThrowModel> Throws { get; set; }
        public string LegWinnerName { get; set; }
        public LegModel() 
        {
            Throws = new List<ThrowModel>();
        }
    }
}
