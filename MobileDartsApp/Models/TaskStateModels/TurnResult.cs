using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileDartsApp.Models.TaskStateModels
{
    public class TurnResult
    {
        public bool IsValid { get; set; }
        public bool Checkout { get; set; }
        public bool MatchEnded { get; set; }
        public bool LegEnded { get; set; }
        public ThrowModel Throw { get; set; }
    }

}
