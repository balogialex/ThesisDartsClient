namespace MobileDartsApp.Services
{
    using MobileDartsApp.Models;
    using MobileDartsApp.Models.TaskStateModels;

    public class GameService
    {
        private readonly SettingsService _settingsService;

        public GameModel CurrentGame { get; private set; }

        private readonly List<int> _notPossibleCheckouts = new() { 180, 177, 174, 171, 169, 168, 166, 165, 163, 162, 159 };
        private readonly List<int> _notPossibleThrownScore = new() { 179, 178, 176, 175, 173, 172, 169, 166, 163 };

        private ThrowModel? _pendingThrow;
        private int _pendingPossibleScore;

        public GameService(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public void CreateNewGame()
        {
            if (_settingsService.CurrentSettings == null)
                throw new InvalidOperationException("Nincsenek betöltve játékbeállítások.");

            CurrentGame = new GameModel(_settingsService.CurrentSettings);
        }

        public ThrowModel GetNextThrowData()
        {
            var nextPlayer = CurrentGame.calculateWhoThrowsNext();
            var tm = CurrentGame.getlastThrowInLegOfPlayer(nextPlayer);
            return tm;
        }

        public string InsertNumber(string insertedScore, string number, int currentScore)
        {
            if (string.IsNullOrEmpty(number)) return insertedScore;

            if (!int.TryParse(insertedScore + number, out int possibleValue))
                return insertedScore;

            if (possibleValue > 180
                || _notPossibleThrownScore.Contains(possibleValue)
                || (insertedScore.Length > 0 && insertedScore[0] == '0')
                || (possibleValue == currentScore && _notPossibleCheckouts.Contains(currentScore)))
            {
                return insertedScore;
            }

            return insertedScore + number;
        }

        public UndoResult UndoOrBackspace(string insertedScore)
        {
            var result = new UndoResult
            {
                InsertedScore = insertedScore,
                HadInsertedScore = !string.IsNullOrEmpty(insertedScore)
            };

            if (result.HadInsertedScore)
            {
                result.InsertedScore = insertedScore.Remove(insertedScore.Length - 1);
            }
            else
            {
                var tsm = CurrentGame.getAndPopLastThrow();
                if (tsm != null)
                {
                    result.InsertedScore = tsm.Scored.ToString();
                    result.PlayerName = tsm.PlayerName;
                    result.CurrentScore = tsm.CurrentScore;
                }
            }

            return result;
        }

        public double GetAverageForPlayer(string name) =>
            CurrentGame.getAverageForPlayer(name);

        public double GetCheckoutPercentForPlayer(string name) =>
            CurrentGame.getCheckoutPercentForPlayer(name);

        public TurnFlow PrepareTurn(string insertedScore, string currentPlayer, int currentScore)
        {
            _pendingThrow = null;
            _pendingPossibleScore = 0;

            if (string.IsNullOrWhiteSpace(insertedScore))
                return new TurnFlow { IsValid = false };

            if (!int.TryParse(insertedScore, out int inputScore))
                return new TurnFlow { IsValid = false };

            int possibleScore = currentScore - inputScore;
            if (possibleScore < 0 || possibleScore == 1)
                return new TurnFlow { IsValid = false };

            if (_notPossibleThrownScore.Contains(inputScore))
                return new TurnFlow { IsValid = false };

            var t = new ThrowModel
            {
                PlayerName = currentPlayer,
                CurrentScore = currentScore,
                Scored = inputScore,
                CheckedOut = (possibleScore == 0),
                ThrownDarts = (possibleScore == 0) ? 0 : 3,
                DoubleTries = 0
            };

            _pendingThrow = t;
            _pendingPossibleScore = possibleScore;

            bool requiresDouble = currentScore <= 170 && !_notPossibleCheckouts.Contains(currentScore) && (possibleScore <= 50);
            bool requiresCheckout = (possibleScore == 0);

            return new TurnFlow
            {
                IsValid = true,
                RequiresDoubleTries = requiresDouble,
                RequiresCheckoutInfo = requiresCheckout
            };
        }

        public void SetPendingDoubleTries(int doubleTries)
        {
            if (_pendingThrow != null)
                _pendingThrow.DoubleTries = doubleTries;
        }

        public void SetPendingCheckoutDarts(int dartsUsed)
        {
            if (_pendingThrow != null)
                _pendingThrow.ThrownDarts = dartsUsed;
        }

        public TurnResult CommitTurn()
        {
            if (_pendingThrow == null)
                return new TurnResult { IsValid = false };

            CurrentGame.addThrow(_pendingThrow);

            bool legEnded = (_pendingPossibleScore == 0);
            bool matchEnded = false;

            if (legEnded)
            {
                CurrentGame.Legs.Last().LegWinnerName = _pendingThrow.PlayerName;

                if (CurrentGame.matchEnded())
                    matchEnded = true;
                else
                    CurrentGame.StartNewLeg();
            }

            var result = new TurnResult
            {
                IsValid = true,
                Checkout = legEnded,
                LegEnded = legEnded,
                MatchEnded = matchEnded,
                Throw = _pendingThrow
            };

            _pendingThrow = null;
            _pendingPossibleScore = 0;

            return result;
        }

    }
}
