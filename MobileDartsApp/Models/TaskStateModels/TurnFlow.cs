namespace MobileDartsApp.Models.TaskStateModels
{
    public class TurnFlow
    {
        public bool IsValid { get; set; }
        public bool RequiresDoubleTries { get; set; }
        public bool RequiresCheckoutInfo { get; set; }
    }
}
