using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileDartsApp.Services.Online.Dtos.Lobby
{
    public class JoinLobbyDto
    {
        public string LobbyGUID { get; set; } = string.Empty;
        public string NewPlayerName { get; set; } = string.Empty;

    }
}
