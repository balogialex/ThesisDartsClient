namespace MobileDartsApp.Models
{
    public class GameModel
    {
        public GameSettingsModel GameSettings { get; private set; }
        public List<LegModel> Legs { get; private set; }
        public GameModel(GameSettingsModel settings)
        {
            GameSettings = settings ?? throw new ArgumentNullException(nameof(settings));
            Legs = new List<LegModel> { new LegModel() };
        }
        public ThrowModel? getAndPopLastThrow()
        {
            if (Legs.Count() == 0 || (Legs.Count == 1 && Legs.Last().Throws.Count == 0))
            {
                return null;
            }
            else if(Legs.Count > 1 && Legs.Last().Throws.Count == 0)
            {
                Legs.RemoveAt(Legs.Count-1);
                ThrowModel tm = Legs.Last().Throws.Last();
                Legs.Last().Throws.Remove(tm);
                return tm;
            }
            else
            {
                ThrowModel tm = Legs.Last().Throws.Last();
                Legs.Last().Throws.Remove(tm);
                return tm;
            }
        }
        public void StartNewLeg()
        {
            Legs.Add(new LegModel());
        }
        public ThrowModel getlastThrowInLegOfPlayer(string playerName)
        {
            var lastLeg = Legs.Last();
            var playerThrows = lastLeg.Throws.Where(t => t.PlayerName == playerName);

            if (!playerThrows.Any())
                return new ThrowModel { CurrentScore = GameSettings.StartScore, PlayerName = playerName, Scored = 0 };

            return playerThrows.Last();
        }
        public void addThrow(ThrowModel toAdd)
        {
            if (Legs.Last().Throws.Count != 0 && Legs.Last().Throws.Last().CheckedOut)
            {
                Legs.Add(new LegModel());
                Legs.Last().Throws.Add(toAdd);
            }
            else
            {
                Legs.Last().Throws.Add(toAdd);
            }
        }
        public string calculateWhoThrowsNext()
        {
            if (Legs.Last().Throws.Count == 0 && Legs.Count() == 1)
                return GameSettings.OrderOfPlayers.First();
            else if (Legs.Last().Throws.Count == 0)
            {
                return GameSettings.OrderOfPlayers[calculateWhoStartsNextLeg()];
            }
            ThrowModel lastAddedThrow = Legs.Last().Throws.Last();

            if (lastAddedThrow.PlayerName == GameSettings.OrderOfPlayers.Last())
                return GameSettings.OrderOfPlayers.First();
            else if(GameSettings.OrderOfPlayers.Count > 0)
            {
                int index = GameSettings.OrderOfPlayers.IndexOf(lastAddedThrow.PlayerName);
                return GameSettings.OrderOfPlayers[index+1];
            }
            return "";
        }
        private int calculateWhoStartsNextLeg()
        {
            int indexOfStarter = 0;
            foreach (LegModel round in Legs.Where(x=>x.Throws.Count !=0))
            {
                if (indexOfStarter==GameSettings.OrderOfPlayers.Count-1)
                    indexOfStarter = 0;
                else
                    indexOfStarter++;
            }
            return indexOfStarter;

        }
        public double getCheckoutPercentForPlayer(string name)
        {
            int sumOfDoubleTries = 0;
            int numberOfLegsWon = 0;
            foreach (LegModel legModel in Legs)
            {
                numberOfLegsWon += legModel.Throws.Where(x => x.PlayerName == name && x.CheckedOut).Count();
                if(numberOfLegsWon > 0)
                    foreach (ThrowModel throwModel in legModel.Throws.Where(x => x.PlayerName == name))
                        sumOfDoubleTries += throwModel.DoubleTries;
                
            }
            if (numberOfLegsWon > 0)
                return Math.Round(numberOfLegsWon / (double)sumOfDoubleTries * 100, 2);
            else
                return 0;
        }
        public double getAverageForPlayer(string name)
        {
            int sumOfThrownScore = Legs.Select(x=> x.Throws.Where(x=>x.PlayerName == name).Select(x=>x.Scored).Sum()).Sum();
            int SumOfThrownDarts = Legs.Select(x => x.Throws.Where(x => x.PlayerName == name).Select(x => x.ThrownDarts).Sum()).Sum();
            if (sumOfThrownScore > 0)
                return Math.Round(sumOfThrownScore / (double)SumOfThrownDarts * 3, 2);
            else
                return 0;
        }
        public bool matchEnded()
        {
            bool won = false;
            int maxWonLegs = 0;
            var legsWonGroupedByPlayers = from l in Legs
                                          group l by l.LegWinnerName into g
                                          select g;
            foreach (var groupitem in legsWonGroupedByPlayers)
                if (groupitem.Count() > maxWonLegs)
                    maxWonLegs = groupitem.Count();

            switch (GameSettings.LegFormat)
            {
                case LegFormat.FirstTo:
                    if (maxWonLegs == GameSettings.LegCount)
                        won = true;
                    break;
                case LegFormat.BestOf:
                    if (maxWonLegs == Math.Round(GameSettings.LegCount / 2.0) || Legs.Count() == GameSettings.LegCount)
                        won = true;
                    break;
            }
            return won;
        }
    }
}
