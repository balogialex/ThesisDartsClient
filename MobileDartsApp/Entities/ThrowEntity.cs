using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileDartsApp.Entities
{
    public class ThrowEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public string PlayerName { get; set; }
        [Indexed]
        public int LegId { get; set; }
        public int Currentscore { get; set; }
        public int Scored { get; set; }
        public int ThrownDarts { get; set; }
        public int DoubleTries { get; set; }
        public bool CheckedOut { get; set; }


    }
}
