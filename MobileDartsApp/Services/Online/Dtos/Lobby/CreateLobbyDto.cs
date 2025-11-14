using MobileDartsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileDartsApp.Services.Online.Dtos.Lobby
{
    public class CreateLobbyDto
    {
        public string LobbyTitle { get; set; } = string.Empty;
        public string LobbyCreator { get; set; } = string.Empty;
        public GameSettingsModel Settings { get; set; }

    }
}
