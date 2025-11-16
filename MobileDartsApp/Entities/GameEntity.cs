using SQLite;

namespace MobileDartsApp.Entities
{
    public class GameEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;


    }
}
