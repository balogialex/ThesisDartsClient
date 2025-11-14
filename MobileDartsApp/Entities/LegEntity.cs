using SQLite;

namespace MobileDartsApp.Entities
{
    public class LegEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int GameId { get; set; }

        [Indexed]
        public string LegWinnerName { get; set; }
        public int LegIndex { get; set; }
    }
}
