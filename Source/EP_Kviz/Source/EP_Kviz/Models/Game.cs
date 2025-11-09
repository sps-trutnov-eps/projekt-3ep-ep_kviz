using System;
using System.Collections.Generic;

namespace EP_Kviz.Models
{
    public enum GameState { Lobby, InProgress, Finished }

    public class Game
    {
        public int Id { get; set; }
        public string Mode { get; set; } = string.Empty;
        public GameState State { get; set; } = GameState.Lobby;
        public List<Player> Players { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // per-game lock for thread-safety
        public object SyncRoot { get; } = new();
    }
}