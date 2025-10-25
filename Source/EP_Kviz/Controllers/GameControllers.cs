using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using EP_Kviz.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public class GamesController : Controller
{
    private readonly IMemoryCache _cache;

    public GamesController(IMemoryCache cache)
    {
        _cache = cache;
    }

    public IActionResult Vyber()
    {
        return View();
    }

    public IActionResult OneVOne(int pid)
    {
        int gameId = GenerateRandomGameId();

        var game = new GameSession
        {
            GameId = gameId,
            Mode = "1v1",
            Players = new List<int> { pid },
            Scores = new Dictionary<int, int> { { pid, 0 } },
            CreatedAt = DateTime.Now
        };

        _cache.Set($"game_{gameId}", game, TimeSpan.FromHours(2));

        return RedirectToAction("Play", new { gameId = gameId, pid = pid });
    }

    public IActionResult TwoVTwo(int pid)
    {
        int gameId = GenerateRandomGameId();

        var game = new GameSession
        {
            GameId = gameId,
            Mode = "2v2",
            Players = new List<int> { pid },
            Scores = new Dictionary<int, int> { { pid, 0 } },
            CreatedAt = DateTime.Now
        };

        _cache.Set($"game_{gameId}", game, TimeSpan.FromHours(2));

        return RedirectToAction("Play", new { gameId = gameId, pid = pid });
    }

    public IActionResult Procviceni(int pid)
    {
        int gameId = GenerateRandomGameId();

        var game = new GameSession
        {
            GameId = gameId,
            Mode = "Procvičení",
            Players = new List<int> { pid },
            Scores = new Dictionary<int, int> { { pid, 0 } },
            CreatedAt = DateTime.Now
        };

        _cache.Set($"game_{gameId}", game, TimeSpan.FromHours(2));

        return RedirectToAction("Play", new { gameId = gameId, pid = pid });
    }

    public IActionResult Join(int gameId, int pid)
    {
        if (_cache.TryGetValue($"game_{gameId}", out GameSession game))
        {
            if (!game.Players.Contains(pid))
                game.Players.Add(pid);

            if (game.Scores == null)
                game.Scores = new Dictionary<int, int>();

            if (!game.Scores.ContainsKey(pid))
                game.Scores[pid] = 0;

            _cache.Set($"game_{gameId}", game, TimeSpan.FromHours(2));

            return RedirectToAction("Play", new { gameId = gameId, pid = pid });
        }
        else
        {
            TempData["Error"] = "Hra s tímto ID neexistuje nebo již skončila!";
            return RedirectToAction("Vyber");
        }
    }

    public IActionResult Play(int gameId, int pid)
    {
        if (_cache.TryGetValue($"game_{gameId}", out GameSession game))
        {
            ViewBag.GameId = game.GameId;
            ViewBag.Mode = game.Mode;
            ViewBag.PlayerId = pid;
            ViewBag.Players = game.Players;
            ViewBag.PlayerCount = game.Players.Count;

            return View("~/Views/Games/Modes/Play.cshtml");
        }
        else
        {
            TempData["Error"] = "Hra neexistuje!";
            return RedirectToAction("Vyber");
        }
    }

    [HttpGet]
    public IActionResult GetGameInfo(int gameId)
    {
        if (_cache.TryGetValue($"game_{gameId}", out GameSession game))
        {
            return Json(new
            {
                playerCount = game.Players.Count,
                players = game.Players,
                mode = game.Mode,
                scores = game.Scores ?? new Dictionary<int, int>()
            });
        }

        return Json(new { error = "Game not found" });
    }

    // Simple test endpoint — increases pid's score by 1
    [HttpGet]
    public IActionResult AddPoint(int gameId, int pid)
    {
        if (_cache.TryGetValue($"game_{gameId}", out GameSession game))
        {
            lock (game)
            {
                if (game.Scores == null)
                    game.Scores = new Dictionary<int, int>();

                if (!game.Scores.ContainsKey(pid))
                    game.Scores[pid] = 0;

                game.Scores[pid]++;

                _cache.Set($"game_{gameId}", game, TimeSpan.FromHours(2));

                return Json(new
                {
                    success = true,
                    players = game.Players,
                    scores = game.Scores
                });
            }
        }

        return Json(new { error = "Game not found" });
    }

    private int GenerateRandomGameId()
    {
        var random = new Random();
        return random.Next(100000, 999999);
    }
}