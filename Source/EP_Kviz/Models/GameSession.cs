using System;
using System.Collections.Generic;

namespace EP_Kviz.Models
{
    public class Cell
    {
        public int Id { get; set; }           // 0..24
        public string Label { get; set; } = "";    // "A", "B", ...
        public int? OwnerPlayerId { get; set; } = null;
        public bool IsAnswered { get; set; } = false;
    }

    public class PendingQuestion
    {
        public int CellId { get; set; }
        public int AskedByPlayerId { get; set; }
        public string QuestionText { get; set; } = "";
        public string Answer { get; set; } = ""; // správná část (po '/')
    }

    public class GameSession
    {
        public int GameId { get; set; }
        public string Mode { get; set; } = "";
        public List<int> Players { get; set; } = new List<int>();
        public Dictionary<int, int> Scores { get; set; } = new Dictionary<int, int>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Nové pro hru
        public List<Cell> Grid { get; set; } = new List<Cell>();
        public int? CurrentTurnPlayerId { get; set; } = null;
        public PendingQuestion? PendingQuestion { get; set; } = null;

        // Přidáme nové vlastnosti pro týmy
        public const int PLAYERS_PROCVICENI = 1;
        public const int PLAYERS_1V1 = 2;
        public const int PLAYERS_2V2 = 4;

        public int RequiredPlayers => Mode switch
        {
            "Procvičení" => PLAYERS_PROCVICENI,
            "1v1" => PLAYERS_1V1,
            "2v2" => PLAYERS_2V2,
            _ => 2 // výchozí hodnota
        };

        // Pro 2v2: rozdělení hráčů do týmů (null = nezařazen)
        public Dictionary<int, string> PlayerTeams { get; set; } = new();

        // Pomocná metoda pro zjištění týmu hráče
        public string? GetPlayerTeam(int playerId) => PlayerTeams.GetValueOrDefault(playerId);

        // Kontrola, zda hra může začít
        public bool CanStart => Mode switch
        {
            "Procvičení" => Players.Count >= PLAYERS_PROCVICENI,
            "1v1" => Players.Count >= PLAYERS_1V1,
            "2v2" => Players.Count >= PLAYERS_2V2,
            _ => false
        };

        // Kontrola, zda je hráč na tahu
        public bool IsPlayersTurn(int playerId)
        {
            if (!CanStart) return false;
            if (Mode == "Procvičení") return true; // v procvičení může hrát vždy
            return CurrentTurnPlayerId == playerId;
        }

        // Získání dalšího hráče na tahu
        public int GetNextPlayer(int currentPlayerId)
        {
            if (Mode == "Procvičení") return currentPlayerId; // stejný hráč pokračuje

            if (Mode == "2v2")
            {
                // Střídání M1 > O1 > M2 > O2
                var blueTeam = Players.Where(p => GetPlayerTeam(p) == "blue").OrderBy(p => Players.IndexOf(p)).ToList();
                var orangeTeam = Players.Where(p => GetPlayerTeam(p) == "orange").OrderBy(p => Players.IndexOf(p)).ToList();
                
                var orderedPlayers = new List<int>();
                for (int i = 0; i < Math.Max(blueTeam.Count, orangeTeam.Count); i++)
                {
                    if (i < blueTeam.Count) orderedPlayers.Add(blueTeam[i]);
                    if (i < orangeTeam.Count) orderedPlayers.Add(orangeTeam[i]);
                }

                int currentIndex = orderedPlayers.IndexOf(currentPlayerId);
                return orderedPlayers[(currentIndex + 1) % orderedPlayers.Count];
            }

            // 1v1 - střídání mezi dvěma hráči
            return Players.First(p => p != currentPlayerId);
        }

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