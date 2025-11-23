using System;
using System.Collections.Generic;
using System.Linq;

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

        // Výhra
        public int? WinnerId { get; set; } = null;
        public string? WinnerTeam { get; set; } = null;
        public bool IsGameOver => WinnerId.HasValue || !string.IsNullOrEmpty(WinnerTeam);

        // Přidáme nové vlastnosti pro týmy
        public const int PLAYERS_PROCVICENI = 1;
        public const int PLAYERS_1V1 = 2;
        public const int PLAYERS_2V2 = 4;
        public const int PLAYERS_DUEL = 2;

        public int RequiredPlayers => Mode switch
        {
            "Procvičení" => PLAYERS_PROCVICENI,
            "1v1" => PLAYERS_1V1,
            "2v2" => PLAYERS_2V2,
            "Duel" => PLAYERS_DUEL,
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
            "Duel" => Players.Count >= PLAYERS_DUEL,
            _ => false
        };

        // Kontrola, zda je hráč na tahu
        public bool IsPlayersTurn(int playerId)
        {
            if (!CanStart) return false;
            if (Mode == "Procvičení") return true; // v procvičení může hrát vždy
            if (Mode == "Duel") return true; // v duelu mohou oba hráči klikat kdykoliv
            return CurrentTurnPlayerId == playerId;
        }

        // Získání dalšího hráče na tahu
        public int GetNextPlayer(int currentPlayerId)
        {
            if (Mode == "Procvičení") return currentPlayerId; // stejný hráč pokračuje
            if (Mode == "Duel") return currentPlayerId; // v duelu se netřídá, oba mohou klikat

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

        // VÝHERNÍ LOGIKA - Kontrola, zda hráč/tým spojil všechny 4 strany
        public void CheckWinner()
        {
            if (Grid == null || Grid.Count != 25) return;

            // Strany gridu (5x5):
            // Horní:  0, 1, 2, 3, 4
            // Dolní: 20,21,22,23,24
            // Levá:   0, 5,10,15,20
            // Pravá:  4, 9,14,19,24

            var topSide = new[] { 0, 1, 2, 3, 4 };
            var bottomSide = new[] { 20, 21, 22, 23, 24 };
            var leftSide = new[] { 0, 5, 10, 15, 20 };
            var rightSide = new[] { 4, 9, 14, 19, 24 };

            if (Mode == "2v2")
            {
                // Kontrola pro týmy
                CheckTeamWin("blue", topSide, bottomSide, leftSide, rightSide);
                CheckTeamWin("orange", topSide, bottomSide, leftSide, rightSide);
            }
            else if (Mode == "Duel")
            {
                // V Duel módu se kontroluje pouze spojení stran (konec při špatné odpovědi je v controlleru)
                foreach (var playerId in Players)
                {
                    if (CheckPlayerWin(playerId, topSide, bottomSide, leftSide, rightSide))
                    {
                        WinnerId = playerId;
                        IsActive = false;
                        return;
                    }
                }
            }
            else
            {
                // Kontrola pro jednotlivé hráče (1v1, Procvičení)
                foreach (var playerId in Players)
                {
                    if (CheckPlayerWin(playerId, topSide, bottomSide, leftSide, rightSide))
                    {
                        WinnerId = playerId;
                        IsActive = false;
                        return;
                    }
                }
            }
        }

        // Nastavení vítěze v Duel módu při špatné odpovědi
        public void SetDuelWinner(int losingPlayerId)
        {
            if (Mode != "Duel") return;
            
            // Vítězem je druhý hráč (ten, kdo neprohrál)
            WinnerId = Players.First(p => p != losingPlayerId);
            IsActive = false;
        }

        private bool CheckPlayerWin(int playerId, int[] top, int[] bottom, int[] left, int[] right)
        {
            var playerCells = Grid.Where(c => c.OwnerPlayerId == playerId).Select(c => c.Id).ToHashSet();

            // Kontrola, zda má políčko na každé straně
            bool hasTop = top.Any(id => playerCells.Contains(id));
            bool hasBottom = bottom.Any(id => playerCells.Contains(id));
            bool hasLeft = left.Any(id => playerCells.Contains(id));
            bool hasRight = right.Any(id => playerCells.Contains(id));

            if (!hasTop || !hasBottom || !hasLeft || !hasRight)
                return false;

            // Kontrola, zda jsou všechny strany propojené pomocí BFS/DFS
            var topStarts = top.Where(id => playerCells.Contains(id));
            foreach (var start in topStarts)
            {
                if (IsConnectedToAllSides(start, playerCells, top, bottom, left, right))
                    return true;
            }

            return false;
        }

        private void CheckTeamWin(string team, int[] top, int[] bottom, int[] left, int[] right)
        {
            if (!string.IsNullOrEmpty(WinnerTeam)) return; // Už je vítěz

            var teamCells = Grid
                .Where(c => c.OwnerPlayerId.HasValue && GetPlayerTeam(c.OwnerPlayerId.Value) == team)
                .Select(c => c.Id)
                .ToHashSet();

            // Kontrola, zda má políčko na každé straně
            bool hasTop = top.Any(id => teamCells.Contains(id));
            bool hasBottom = bottom.Any(id => teamCells.Contains(id));
            bool hasLeft = left.Any(id => teamCells.Contains(id));
            bool hasRight = right.Any(id => teamCells.Contains(id));

            if (!hasTop || !hasBottom || !hasLeft || !hasRight)
                return;

            // Kontrola propojení
            var topStarts = top.Where(id => teamCells.Contains(id));
            foreach (var start in topStarts)
            {
                if (IsConnectedToAllSides(start, teamCells, top, bottom, left, right))
                {
                    WinnerTeam = team;
                    IsActive = false;
                    return;
                }
            }
        }

        private bool IsConnectedToAllSides(int startCell, HashSet<int> ownedCells, int[] top, int[] bottom, int[] left, int[] right)
        {
            var visited = new HashSet<int>();
            var queue = new Queue<int>();
            queue.Enqueue(startCell);
            visited.Add(startCell);

            bool reachedTop = top.Contains(startCell);
            bool reachedBottom = false;
            bool reachedLeft = left.Contains(startCell);
            bool reachedRight = false;

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();

                // Kontrola, zda jsme dosáhli všech stran
                if (top.Contains(current)) reachedTop = true;
                if (bottom.Contains(current)) reachedBottom = true;
                if (left.Contains(current)) reachedLeft = true;
                if (right.Contains(current)) reachedRight = true;

                if (reachedTop && reachedBottom && reachedLeft && reachedRight)
                    return true;

                // Získání sousedů (horní, dolní, levý, pravý)
                foreach (var neighbor in GetNeighbors(current))
                {
                    if (ownedCells.Contains(neighbor) && !visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return reachedTop && reachedBottom && reachedLeft && reachedRight;
        }

        private List<int> GetNeighbors(int cellId)
        {
            var neighbors = new List<int>();
            int row = cellId / 5;
            int col = cellId % 5;

            // Horní soused
            if (row > 0) neighbors.Add((row - 1) * 5 + col);
            // Dolní soused
            if (row < 4) neighbors.Add((row + 1) * 5 + col);
            // Levý soused
            if (col > 0) neighbors.Add(row * 5 + (col - 1));
            // Pravý soused
            if (col < 4) neighbors.Add(row * 5 + (col + 1));

            return neighbors;
        }
    }
}