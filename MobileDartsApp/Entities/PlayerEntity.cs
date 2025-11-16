using SQLite;

namespace MobileDartsApp.Entities
{
    [Table("Player")]
    public class PlayerEntity
    {
        [PrimaryKey, Column("name"), Unique]
        public string Name { get; set; } = string.Empty;

        [Column("pictureSource")]
        public string PictureSource { get; set; } = "profile.png";

    }
}
