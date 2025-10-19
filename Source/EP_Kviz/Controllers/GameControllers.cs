using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using EP_Kviz.Models;

public class GamesController : Controller
{
    private readonly IMemoryCache _cache;

    // Konstruktor - ASP.NET automaticky předá cache
    public GamesController(IMemoryCache cache)
    {
        _cache = cache;
    }

    // Výběr módu
    public IActionResult Vyber()
    {
        return View();
    }

    // Vytvoření nové hry - 1v1
    public IActionResult OneVOne(int pid)
    {
        int gameId = GenerateRandomGameId();

        var game = new GameSession
        {
            GameId = gameId,
            Mode = "1v1",
            Players = new List<int> { pid },
            CreatedAt = DateTime.Now
        };

        // Uložení do cache (vydrží 2 hodiny)
        _cache.Set($"game_{gameId}", game, TimeSpan.FromHours(2));

        return RedirectToAction("Play", new { gameId = gameId, pid = pid });
    }

    // Vytvoření nové hry - 2v2
    public IActionResult TwoVTwo(int pid)
    {
        int gameId = GenerateRandomGameId();

        var game = new GameSession
        {
            GameId = gameId,
            Mode = "2v2",
            Players = new List<int> { pid },
            CreatedAt = DateTime.Now
        };

        _cache.Set($"game_{gameId}", game, TimeSpan.FromHours(2));

        return RedirectToAction("Play", new { gameId = gameId, pid = pid });
    }

    // Vytvoření nové hry - Procvičení
    public IActionResult Procviceni(int pid)
    {
        int gameId = GenerateRandomGameId();

        var game = new GameSession
        {
            GameId = gameId,
            Mode = "Procvičení",
            Players = new List<int> { pid },
            CreatedAt = DateTime.Now
        };

        _cache.Set($"game_{gameId}", game, TimeSpan.FromHours(2));

        return RedirectToAction("Play", new { gameId = gameId, pid = pid });
    }

    // Připojení k existující hře
    public IActionResult Join(int gameId, int pid)
    {
        if (_cache.TryGetValue($"game_{gameId}", out GameSession game))
        {
            // Hra existuje - přidáme hráče
            if (!game.Players.Contains(pid))
            {
                game.Players.Add(pid);
                _cache.Set($"game_{gameId}", game, TimeSpan.FromHours(2));
            }

            return RedirectToAction("Play", new { gameId = gameId, pid = pid });
        }
        else
        {
            // Hra neexistuje
            TempData["Error"] = "Hra s tímto ID neexistuje nebo již skončila!";
            return RedirectToAction("Vyber");
        }
    }

    // Herní stránka
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

    // Generování náhodného Game ID
    private int GenerateRandomGameId()
    {
        var random = new Random();
        return random.Next(100000, 999999);
    }
}