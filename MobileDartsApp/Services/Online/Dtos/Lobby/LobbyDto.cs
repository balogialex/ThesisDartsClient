using MobileDartsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileDartsApp.Services.Online.Dtos.Lobby
{
    public class LobbyDto
    {
        public string LobbyGUID { get; set; } = string.Empty;
        public string LobbyCreator { get; set; } = string.Empty;
        public string LobbyTitle { get; set; } = string.Empty;
        public GameSettingsModel Settings { get; set; }
        public List<string> Players { get; set; } = new List<string>();
        public Dictionary<string, bool> PlayerReadiness { get; set; } = new Dictionary<string, bool>();


    }
}
