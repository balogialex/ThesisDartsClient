using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileDartsApp.Models.TaskStateModels
{
    public class UndoResult
    {
        public bool HadInsertedScore { get; set; }
        public string InsertedScore { get; set; }
        public string PlayerName { get; set; }
        public int CurrentScore { get; set; }
    }
}
