using System;
using System.Collections.Concurrent;
using System.Linq;
using EP_Kviz.Models;
using EP_Kviz.Models.Dtos;

namespace EP_Kviz.Services
{
    public class GameManager
    {
        private readonly ConcurrentDictionary<int, Game> _games = new();
        private readonly Random _rng = new();

        public int CreateGame(string mode, int ownerPlayerId)
        {
            int id;
            do
            {
                id = _rng.Next(100000, 999999);
            } while (!_games.TryAdd(id, CreateEmptyGame(id, mode, ownerPlayerId)));

            return id;
        }

        private Game CreateEmptyGame(int id, string mode, int ownerPlayerId)
        {
            return new Game
            {
                Id = id,
                Mode = mode,
                Players = new System.Collections.Generic.List<Player>
                {
                    new Player { Id = ownerPlayerId }
                },
                State = GameState.Lobby,
                CreatedAt = DateTime.UtcNow
            };
        }

        public bool JoinGame(int gameId, int playerId)
        {
            if (!_games.TryGetValue(gameId, out var game)) return false;

            lock (game.SyncRoot)
            {
                if (game.Players.Any(p => p.Id == playerId)) return true;
                if (game.State != GameState.Lobby) return false;

                game.Players.Add(new Player { Id = playerId });
                return true;
            }
        }

        public GameInfoDto? GetGameInfo(int gameId)
        {
            if (!_games.TryGetValue(gameId, out var game)) return null;

            lock (game.SyncRoot)
            {
                return new GameInfoDto
                {
                    GameId = game.Id,
                    Mode = game.Mode,
                    PlayerCount = game.Players.Count,
                    Players = game.Players.Select(p => p.Id).ToList(),
                    State = game.State.ToString()
                };
            }
        }

        // stubs to implement game rules
        public bool StartGame(int gameId) { /* set State = InProgress */ return _games.TryGetValue(gameId, out var g) && (g.State = GameState.InProgress) == GameState.InProgress; }
        public bool MakeMove(int gameId, int playerId, object moveData) { /* apply rules */ return true; }
        public bool EndGame(int gameId) { /* finalize */ return true; }
    }
}