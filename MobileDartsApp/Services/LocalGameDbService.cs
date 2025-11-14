using MobileDartsApp.Entities;
using MobileDartsApp.Models;
using SQLite;
using System.Diagnostics;

namespace MobileDartsApp.Services
{
    public class LocalGameDbService
    {
        private const string DB_NAME = "DartsApp_local_db.db3";
        private readonly SQLiteAsyncConnection _connection;
        public LocalGameDbService()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
        }
        public async Task InitializeTablesAsync()
        {
            await _connection.CreateTableAsync<GameEntity>();
            await _connection.CreateTableAsync<LegEntity>();
            await _connection.CreateTableAsync<ThrowEntity>();
            await _connection.CreateTableAsync<PlayerEntity>();
        }
        public async Task<List<GameEntity>> GetAllGameEntities()
        {
            return await _connection.Table<GameEntity>().ToListAsync();

        }
        public async Task<List<LegEntity>> GetAllLegsInGame(int gameID)
        {
            try
            {
                return await _connection.Table<LegEntity>()
                    .Where(l => l.GameId == gameID).ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving legs for game ID {gameID}: {ex.Message}");
                throw;
            }
        }
        public async Task<List<GameEntity>> GetAllGameEntitiesByName(PlayerEntity? player)
        {
            if (player == null || string.IsNullOrWhiteSpace(player.Name))
                throw new ArgumentException("Player entity is null or invalid.");

            var games = await _connection.QueryAsync<GameEntity>(
                @"
                SELECT DISTINCT g.*
                FROM GameEntity g
                JOIN LegEntity l ON g.Id = l.GameId
                JOIN ThrowEntity t ON l.Id = t.LegId
                WHERE t.PlayerName = ?
                ",
                player.Name
            );

            return games;
        }
        public async Task<List<ThrowEntity>> GetAllThrowsInLeg(int legID)
        {
            try
            {
                return await _connection.Table<ThrowEntity>().Where(l => l.LegId == legID).ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving legs for game ID {legID}: {ex.Message}");
                throw;
            }
        }
        public async Task<GameEntity> GetById(int id)
        {
            return await _connection.Table<GameEntity>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }
        public async Task Create(GameModel gameModel)
        {
            int legIndex = 1;
                try
                {
                    GameEntity gameEntity = new GameEntity()
                    {
                        Date = DateTime.Now.Date,
                        Title = $"Player(s): {string.Join(" & ", gameModel.GameSettings.OrderOfPlayers)}",
                        Description = "This was a good game!"
                    };
                    await _connection.InsertAsync(gameEntity);

                    foreach (LegModel leg in gameModel.Legs)
                    {
                        var legEntity = new LegEntity()
                        {
                            GameId = gameEntity.Id,
                            LegWinnerName = leg.LegWinnerName,
                            LegIndex = legIndex
                        };

                        await _connection.InsertAsync(legEntity);
                        legIndex++;
                        foreach (ThrowModel throwModel in leg.Throws)
                        {
                            var throwEntity = new ThrowEntity()
                            {
                                PlayerName = throwModel.PlayerName,
                                LegId = legEntity.Id,
                                Scored = throwModel.Scored,
                                Currentscore = throwModel.CurrentScore,
                                ThrownDarts = throwModel.ThrownDarts,
                                DoubleTries = throwModel.DoubleTries,
                                CheckedOut = throwModel.CheckedOut
                            };
                            await _connection.InsertAsync(throwEntity);
                        }
                    }
                }
                catch
                {
                    throw;
                }
        }
    }
}
