using MobileDartsApp.Entities;
using MobileDartsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileDartsApp.Services
{
    public class StatisticsService
    {
        private LocalGameDbService _gameDbService;
        public GameEntity? CurrentGame { get; set; }   
        public LegEntity? CurrentLeg { get; set; }   
        public PlayerEntity? CurrentPlayer { get; set; }
        public StatisticsService(LocalGameDbService gameDbService)
        {
            _gameDbService = gameDbService;
        }
        public async Task<StatisticsModel> GetStatsForLeg()
        {
            StatisticsModel stats = new StatisticsModel();
            List<ThrowEntity> throws = (from t in await _gameDbService.GetAllThrowsInLeg(CurrentLeg.Id)
                                        where t.PlayerName == CurrentPlayer?.Name
                                        select t).Cast<ThrowEntity>().ToList();
            
            stats.TotalScored += throws.Sum(x => x.Scored);
            stats.ThrownDarts += throws.Sum(x => x.ThrownDarts);
            stats.CheckoutTries += throws.Sum(x => x.DoubleTries);
            stats.CheckoutCount += throws.Count(x => x.CheckedOut);
            stats.HighestThrow = throws.Max(x => x.Scored);
            stats.HighestCheckout = throws.Where(x => x.CheckedOut).Select(x => x.Scored).FirstOrDefault();

            if (throws.Count < 3)
            {
                for (int i = 0; i < throws.Count; i++)
                    stats.TotalFirst9Scored += throws[i].Scored;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                    stats.TotalFirst9Scored += throws[i].Scored;
            }

            stats.Average = Math.Round((double)stats.TotalScored / stats.ThrownDarts * 3, 2);
            stats.First9Average = Math.Round((double)stats.TotalFirst9Scored / 9 * 3, 2);
            if (stats.CheckoutTries <= 0)
                stats.CheckoutPercent = 0;
            else
                stats.CheckoutPercent = Math.Round((double)stats.CheckoutCount / stats.CheckoutTries * 100, 2);
            return stats;
        }

        public async Task<StatisticsModel> GetStatsForGame()
        {
            StatisticsModel stats = new StatisticsModel();
            List<LegEntity> legs = await _gameDbService.GetAllLegsInGame(CurrentGame.Id);
            foreach (LegEntity leg in legs)
            {
                List<ThrowEntity> throws = (from t in await _gameDbService.GetAllThrowsInLeg(leg.Id)
                                            where t.PlayerName == CurrentPlayer.Name
                                            select t).Cast<ThrowEntity>().ToList();

                stats.TotalScored += throws.Sum(x => x.Scored);
                stats.ThrownDarts += throws.Sum(x => x.ThrownDarts);
                stats.CheckoutTries += throws.Sum(x => x.DoubleTries);
                stats.CheckoutCount += throws.Count(x => x.CheckedOut);

                int highestThrowInLeg = throws.Max(x => x.Scored);
                if (stats.HighestThrow < highestThrowInLeg)
                    stats.HighestThrow = highestThrowInLeg;

                int highestCheckoutInLeg = throws.Where(x => x.CheckedOut).Select(x => x.Scored).FirstOrDefault();
                if (stats.HighestCheckout < highestCheckoutInLeg)
                    stats.HighestCheckout = highestCheckoutInLeg;

                if (throws.Count < 3)
                {
                    for (int i = 0; i < throws.Count; i++)
                        stats.TotalFirst9Scored += throws[i].Scored;
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                        stats.TotalFirst9Scored += throws[i].Scored;
                }
            }
            stats.Average = Math.Round((double)stats.TotalScored / stats.ThrownDarts * 3, 2);
            stats.First9Average = Math.Round((double)stats.TotalFirst9Scored / (9 * legs.Count) * 3, 2);
            if (stats.CheckoutTries <= 0)
                stats.CheckoutPercent = 0;
            else
                stats.CheckoutPercent = Math.Round((double)stats.CheckoutCount / stats.CheckoutTries * 100, 2);

            return stats;
        }
    }
}
