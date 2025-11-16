using System.Collections.Generic;

namespace EP_Kviz.Models.Dtos
{
    public class GameInfoDto
    {
        public int GameId { get; set; }
        public string Mode { get; set; } = string.Empty;
        public int PlayerCount { get; set; }
        public List<int> Players { get; set; } = new();
        public string State { get; set; } = string.Empty;
    }
}