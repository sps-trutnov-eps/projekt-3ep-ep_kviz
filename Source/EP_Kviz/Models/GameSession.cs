using System;
using System.Collections.Generic;

namespace EP_Kviz.Models
{
    public class GameSession
    {
        public int GameId { get; set; }
        public string Mode { get; set; } = string.Empty;
        public List<int> Players { get; set; } = new List<int>();
        public Dictionary<int, int> Scores { get; set; } = new Dictionary<int, int>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Convenience helper used by controllers / services
        public void EnsurePlayer(int playerId)
        {
            if (!Players.Contains(playerId))
                Players.Add(playerId);

            if (!Scores.ContainsKey(playerId))
                Scores[playerId] = 0;
        }
    }
}