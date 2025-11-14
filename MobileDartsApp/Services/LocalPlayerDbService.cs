using MobileDartsApp.Entities;
using SQLite;


namespace MobileDartsApp.Services
{
    public class LocalPlayerDbService
    {
        private const string DB_NAME = "DartsApp_local_db.db3";
        private readonly SQLiteAsyncConnection _connection;
        public LocalPlayerDbService()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
        }
        public async Task<List<PlayerEntity>> GetAllPlayerEntities()
        {
            return await _connection.Table<PlayerEntity>().ToListAsync();
        }
        
        public async Task<PlayerEntity> GetByName(string name)
        {
            return await _connection.Table<PlayerEntity>().Where(x => x.Name == name).FirstOrDefaultAsync();
        }
        public async Task Create(PlayerEntity player)
        {
            await _connection.InsertAsync(player);
        }
        public async Task Update(PlayerEntity player)
        {
            await _connection.UpdateAsync(player);
        }
        public async Task Delete(PlayerEntity player)
        {
            await _connection.DeleteAsync(player);
        }
    }
}
