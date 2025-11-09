using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using EP_Kviz.Models;
using System.Text;
using System.Text.Json;
using System.IO;  // <- Přidej tento řádek

public class GamesController : Controller
{
    private readonly IMemoryCache _cache;
    private static readonly object _questionsLock = new object();
    private static List<(string Q, string A)> _questionsCache = null;

    public GamesController(IMemoryCache cache)
    {
        _cache = cache;
    }

    public IActionResult Vyber()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            // Nepřihlášený uživatel, přesměrování na login
            return RedirectToAction("Login", "Home");
        }

        ViewBag.UserId = userId.Value;
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
            {
                game.Players.Add(pid);
                _cache.Set($"game_{gameId}", game, TimeSpan.FromHours(2));
            }

            return RedirectToAction("Play", new { gameId = gameId, pid = pid });
        }
        else
        {
            TempData["Error"] = "Hra s tímto ID neexistuje nebo již skončila!";
            return RedirectToAction("Vyber");
        }
    }

    // Po vytvoření/joinu při 1v1: inicializujeme grid když dosáhneme 2 hráčů
    private void EnsureGridInitialized(GameSession game)
    {
        if (game.Grid != null && game.Grid.Count == 25) return;

        game.Grid = new List<Cell>();
        char label = 'A';
        for (int i = 0; i < 25; i++)
        {
            game.Grid.Add(new Cell { Id = i, Label = ((char)(label + i)).ToString() });
        }

        // první hráč začne
        game.CurrentTurnPlayerId = game.Players.Count > 0 ? game.Players[0] : (int?)null;
        game.PendingQuestion = null;
    }

    private List<(string Q, string A)> LoadQuestions()
    {
        if (_questionsCache != null) return _questionsCache;

        lock (_questionsLock)
        {
            if (_questionsCache != null) return _questionsCache;

            // Hledáme soubor Otázky.txt relativně k výstupnímu adresáři
            string baseDir = AppContext.BaseDirectory;
            string[] candidates =
            {
                Path.Combine(baseDir, "Otázky.txt"),
                Path.Combine(Directory.GetParent(baseDir)?.FullName ?? "", "Otázky.txt"),
                Path.Combine(Directory.GetParent(Directory.GetParent(baseDir)?.FullName ?? "")?.FullName ?? "", "Otázky.txt")
            };

            string path = candidates.FirstOrDefault(p => System.IO.File.Exists(p));  // <- Zde je změna
            var list = new List<(string Q, string A)>();
            if (path != null)
            {
                foreach (var line in System.IO.File.ReadAllLines(path).Where(l => !string.IsNullOrWhiteSpace(l)))
                {
                    var parts = line.Split('/');
                    var q = parts[0].Trim();
                    var a = parts.Length > 1 ? parts[1].Trim() : "";
                    list.Add((q, a));
                }
            }
            _questionsCache = list;
            return _questionsCache;
        }
    }

    // Úprava Play: pokud hra existuje a má dost hráčů, inicializovat grid
    public IActionResult Play(int gameId, int pid)
    {
        if (_cache.TryGetValue($"game_{gameId}", out GameSession game))
        {
            // pokud je počet hráčů dosažen -> inicializuj grid
            if (game.Players.Count >= 2 && (game.Grid == null || game.Grid.Count == 0))
            {
                EnsureGridInitialized(game);
                _cache.Set($"game_{gameId}", game, TimeSpan.FromHours(2));
            }

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

    // Vrátí kompletní stav (grid/currentTurn/pendingQuestion) pro klienty (polling)
    [HttpGet]
    public IActionResult GetGameState(int gameId)
    {
        if (_cache.TryGetValue($"game_{gameId}", out GameSession game))
        {
            return Json(new
            {
                gameId = game.GameId,
                mode = game.Mode,
                players = game.Players,
                playerCount = game.Players.Count,
                currentTurn = game.CurrentTurnPlayerId,
                grid = game.Grid.Select(c => new { c.Id, c.Label, c.OwnerPlayerId, c.IsAnswered }),
                pending = game.PendingQuestion != null ? new { game.PendingQuestion.CellId, game.PendingQuestion.AskedByPlayerId } : null
            });
        }
        return Json(new { error = "Game not found" });
    }

    // Požadavek na otázku - pouze ten, kdo má tah
    [HttpPost]
    public IActionResult RequestQuestion(int gameId, int pid, int cellId)
    {
        if (!_cache.TryGetValue($"game_{gameId}", out GameSession game))
            return Json(new { error = "Game not found" });

        if (game.CurrentTurnPlayerId != pid)
            return Json(new { error = "Není tvůj tah." });

        var cell = game.Grid.FirstOrDefault(c => c.Id == cellId);
        if (cell == null) return Json(new { error = "Neplatná buňka." });
        if (cell.IsAnswered) return Json(new { error = "Buňka už byla zodpovězena." });

        var questions = LoadQuestions();
        if (questions == null || questions.Count == 0)
            return Json(new { error = "Žádné otázky dostupné." });

        var rnd = new Random();
        var q = questions[rnd.Next(questions.Count)];

        game.PendingQuestion = new PendingQuestion
        {
            CellId = cellId,
            AskedByPlayerId = pid,
            QuestionText = q.Q,
            Answer = q.A
        };

        _cache.Set($"game_{gameId}", game, TimeSpan.FromHours(2));

        // Vrátíme text otázky klientovi (oba klienti ho uvidí skrz polling)
        return Json(new { ok = true, question = q.Q, cellId = cellId });
    }

    // Odeslání odpovědi
    [HttpPost]
    public IActionResult SubmitAnswer(int gameId, int pid, int cellId, string answer)
    {
        if (!_cache.TryGetValue($"game_{gameId}", out GameSession game))
            return Json(new { error = "Game not found" });

        if (game.PendingQuestion == null) return Json(new { error = "Není žádná aktivní otázka." });
        if (game.PendingQuestion.CellId != cellId) return Json(new { error = "Otázka neodpovídá této buňce." });
        if (game.PendingQuestion.AskedByPlayerId != pid) return Json(new { error = "Nemůžeš odpovědět na otázku, kterou nepožádal tvůj tah." });

        var correct = string.Equals((game.PendingQuestion.Answer ?? "").Trim(), (answer ?? "").Trim(), StringComparison.OrdinalIgnoreCase);

        var cell = game.Grid.First(c => c.Id == cellId);
        if (correct)
        {
            cell.OwnerPlayerId = pid;
            cell.IsAnswered = true;
        }
        else
        {
            // Špatná odpověď - buňka zůstane aktivní (IsAnswered = false)
            // a označíme ji černě (OwnerPlayerId = -1)
            cell.OwnerPlayerId = -1;
            cell.IsAnswered = false; // Změna zde - může se znovu odpovídat
        }

        // Clear pending question
        game.PendingQuestion = null;

        // Posun tahu na dalšího hráče (pokud existuje)
        var other = game.Players.FirstOrDefault(p => p != pid);
        game.CurrentTurnPlayerId = other;

        _cache.Set($"game_{gameId}", game, TimeSpan.FromHours(2));

        return Json(new { ok = true, correct = correct, owner = cell.OwnerPlayerId, cellId = cellId, nextTurn = game.CurrentTurnPlayerId });
    }

    private int GenerateRandomGameId()
    {
        var random = new Random();
        return random.Next(100000, 999999);
    }
}