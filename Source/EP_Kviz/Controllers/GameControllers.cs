using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using EP_Kviz.Models;
using System.Text;
using System.Text.Json;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;

public class GamesController : Controller
{
    private readonly IMemoryCache _cache;
    private static readonly object _questionsLock = new object();
    private static List<(string Q, string A)> _questionsCache = null;

    public GamesController(IMemoryCache cache)
    {
        _cache = cache;
    }

    [Authorize]
    public IActionResult Vyber()
    {
        // Získání přihlášeného uživatele z Identity
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Uzivatele");
        }

        // Pro hru použijeme hashCode userId jako číslo
        int numericUserId = 0;
        using (var sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(userId));
            numericUserId = Math.Abs(BitConverter.ToInt32(hashBytes, 0));
        }
        ViewBag.UserId = numericUserId;
        
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

        // Pro 2v2 inicializujeme první hráč do modrého týmu
        game.PlayerTeams[pid] = "blue";

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

        // Pro procvičení ihned inicializujeme grid a hráč může začít
        EnsureGridInitialized(game);
        
        _cache.Set($"game_{gameId}", game, TimeSpan.FromHours(2));
        return RedirectToAction("Play", new { gameId = gameId, pid = pid });
    }

    public IActionResult FullBlockMode(int pid)
    {
        int gameId = GenerateRandomGameId();

        var game = new GameSession
        {
            GameId = gameId,
            Mode = "FullBlock",
            Players = new List<int> { pid },
            Scores = new Dictionary<int, int> { { pid, 0 } },
            CreatedAt = DateTime.Now
        };

        _cache.Set($"game_{gameId}", game, TimeSpan.FromHours(2));
        return RedirectToAction("Play", new { gameId = gameId, pid = pid });
    }

    public IActionResult Duel(int pid)
    {
        int gameId = GenerateRandomGameId();

        var game = new GameSession
        {
            GameId = gameId,
            Mode = "Duel",
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
            // Kontrola maximálního počtu hráčů
            if (game.Players.Count >= game.RequiredPlayers)
            {
                TempData["Error"] = "Hra je již plná!";
                return RedirectToAction("Vyber");
            }

            if (!game.Players.Contains(pid))
            {
                game.Players.Add(pid);

                // Přiřazení týmu pro 2v2
                if (game.Mode == "2v2")
                {
                    string team = game.Players.Count <= 2 ? "blue" : "orange";
                    game.PlayerTeams[pid] = team;
                }

                // Pokud je teď dost hráčů, inicializuj grid
                if (game.Players.Count == game.RequiredPlayers)
                {
                    game.CheckWinner();
                    if (game.Grid == null || game.Grid.Count == 0)
                    {
                        EnsureGridInitialized(game);
                    }
                }
            }

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


    private void EnsureGridInitialized(GameSession game)
    {
        if (game.Grid?.Count == 25) return;
        if (!game.CanStart) return; // Explicitly disambiguate the property

        game.Grid = new List<Cell>();
        char label = 'A';
        for (int i = 0; i < 25; i++)
        {
            game.Grid.Add(new Cell { Id = i, Label = ((char)(label + i)).ToString() });
        }

        // Nastavení prvního tahu podle módu
        if (game.Mode == "Procvičení")
        {
            game.CurrentTurnPlayerId = game.Players[0];
        }
        else if (game.Mode == "2v2")
        {
            // První modrý hráč začíná
            game.CurrentTurnPlayerId = game.Players.First(p => game.GetPlayerTeam(p) == "blue");
        }
        else if (game.Mode == "FullBlock")
        {
            game.CurrentTurnPlayerId = game.Players[0];
        }
        else // 1v1
        {
            game.CurrentTurnPlayerId = game.Players[0];
        }

        game.PendingQuestion = null;
    }


    private static int _questionIndex = 0;
    private static readonly object _questionIndexLock = new object();

    private List<(string Q, string A)> LoadQuestions()
    {
        if (_questionsCache != null) return _questionsCache;

        lock (_questionsLock)
        {
            if (_questionsCache != null) return _questionsCache;

            string baseDir = AppContext.BaseDirectory;
            string[] candidates =
            {
                Path.Combine(baseDir, "Otázky.txt"),
                Path.Combine(Directory.GetParent(baseDir)?.FullName ?? "", "Otázky.txt"),
                Path.Combine(Directory.GetParent(Directory.GetParent(baseDir)?.FullName ?? "")?.FullName ?? "", "Otázky.txt")
            };

            string path = candidates.FirstOrDefault(p => System.IO.File.Exists(p));
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
    private (string Q, string A) GetNextQuestion()
    {
        var questions = LoadQuestions();
        if (questions == null || questions.Count == 0)
            return ("", "");

        lock (_questionIndexLock)
        {
            var q = questions[_questionIndex];
            _questionIndex++;
            if (_questionIndex >= questions.Count)
                _questionIndex = 0;
            return q;
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
                requiredPlayers = game.RequiredPlayers,
                canStart = game.CanStart,
                currentTurn = game.CurrentTurnPlayerId,
                grid = game.Grid?.Select(c => new { 
                    c.Id, 
                    c.Label, 
                    c.OwnerPlayerId, 
                    c.IsAnswered,
                    team = game.Mode == "2v2" && c.OwnerPlayerId.HasValue ? 
                          game.GetPlayerTeam(c.OwnerPlayerId.Value) : null
                }).Cast<object>().ToList() ?? new List<object>(),
                teams = game.Mode == "2v2" ? game.PlayerTeams : null,
                pending = game.PendingQuestion != null ? new { 
                    game.PendingQuestion.CellId, 
                    game.PendingQuestion.AskedByPlayerId 
                } : null,
                scores = game.Scores ?? new Dictionary<int, int>(),
                isGameOver = game.IsGameOver,
                winnerId = game.WinnerId,
                winnerTeam = game.WinnerTeam
            });
        }
        return Json(new { error = "Game not found" });
    }

    [HttpPost]
    public IActionResult RequestQuestion(int gameId, int pid, int cellId)
    {
        if (!_cache.TryGetValue($"game_{gameId}", out GameSession game))
            return Json(new { error = "Game not found" });

        if (!game.CanStart)
            return Json(new { error = "Čekám na další hráče..." });

        if (!game.IsPlayersTurn(pid))
            return Json(new { error = "Není tvůj tah." });

        var cell = game.Grid?.FirstOrDefault(c => c.Id == cellId);
        if (cell == null)
            return Json(new { error = "Neplatná buňka." });

        if (cell.IsAnswered)
            return Json(new { error = "Buňka už byla zodpovězena." });

        var questions = LoadQuestions();
        if (questions == null || questions.Count == 0)
            return Json(new { error = "Žádné otázky dostupné." });

        var q = GetNextQuestion();

        game.PendingQuestion = new PendingQuestion
        {
            CellId = cellId,
            AskedByPlayerId = pid,
            QuestionText = q.Q,
            Answer = q.A
        };

        _cache.Set($"game_{gameId}", game, TimeSpan.FromHours(2));

        // LOGOVÁNÍ
        System.Diagnostics.Debug.WriteLine($"[RequestQuestion] gameId={gameId}, pid={pid}, cellId={cellId}, Otázka: {q.Q}, Odpověď: {q.A}");

        return Json(new { ok = true, question = q.Q, cellId = cellId });
    }


    private string NormalizeAnswer(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "";

        var normalized = input.Trim().ToLowerInvariant();
        normalized = normalized.Normalize(System.Text.NormalizationForm.FormD);
        var sb = new System.Text.StringBuilder();
        foreach (var c in normalized)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark
                && !char.IsWhiteSpace(c))
                sb.Append(c);
        }
        return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
    }



    [HttpPost]
    public IActionResult SubmitAnswer(int gameId, int pid, int cellId, string answer)
    {
        if (!_cache.TryGetValue($"game_{gameId}", out GameSession game))
            return Json(new { error = "Game not found" });

        if (!game.CanStart)
            return Json(new { error = "Čekám na další hráče..." });

        if (game.PendingQuestion == null)
            return Json(new { error = "Není žádná aktivní otázka." });

        if (game.PendingQuestion.CellId != cellId)
            return Json(new { error = "Otázka neodpovídá této buňce." });

        if (game.PendingQuestion.AskedByPlayerId != pid)
            return Json(new { error = "Nemůžeš odpovědět na otázku, kterou nepožádal tvůj tah." });

        var correct = NormalizeAnswer(game.PendingQuestion.Answer) == NormalizeAnswer(answer);

        // LOGOVÁNÍ
        System.Diagnostics.Debug.WriteLine($"[SubmitAnswer] gameId={gameId}, pid={pid}, cellId={cellId}");
        System.Diagnostics.Debug.WriteLine($"Zadaná odpověď: '{answer}', Správná odpověď: '{game.PendingQuestion.Answer}'");
        System.Diagnostics.Debug.WriteLine($"Porovnání: '{NormalizeAnswer(game.PendingQuestion.Answer)}' == '{NormalizeAnswer(answer)}' => {correct}");

        var cell = game.Grid.First(c => c.Id == cellId);
        if (correct)
        {
            cell.OwnerPlayerId = pid;
            cell.IsAnswered = true;

            if (game.Mode == "FullBlock")
            {
                // Pokud jsou všechna pole zodpovězena, určete vítěze
                if (game.Grid.All(c => c.IsAnswered))
                {
                    // Spočítejte počet polí pro každého hráče
                    var playerCounts = game.Players.ToDictionary(
                        p => p,
                        p => game.Grid.Count(c => c.OwnerPlayerId == p)
                    );

                    // Najděte hráče s nejvyšším počtem polí
                    var max = playerCounts.Values.Max();
                    var winners = playerCounts.Where(kv => kv.Value == max).Select(kv => kv.Key).ToList();

                    if (winners.Count == 1)
                    {
                        game.WinnerId = winners[0];
                    }
                    else
                    {
                        game.WinnerId = null; // Remíza
                    }

                    game.CheckWinner(); // Předpokládáme, že tato metoda nastaví `IsGameOver` na true
                }

            }
            else
            {
                game.CheckWinner();
            }
        }
        else
        {
            // V Duel módu končí hra při špatné odpovědi
            if (game.Mode == "Duel")
            {
                cell.OwnerPlayerId = -1;
                cell.IsAnswered = false;
                game.SetDuelWinner(pid); // Vyhrává druhý hráč
            }
            else
            {
                cell.OwnerPlayerId = -1; // černá
                cell.IsAnswered = false;  // lze znovu odpovídat
            }
        }


        game.PendingQuestion = null;

        if (!game.IsGameOver)
        {
            game.CurrentTurnPlayerId = game.GetNextPlayer(pid);
        }

        _cache.Set($"game_{gameId}", game, TimeSpan.FromHours(2));

        return Json(new
        {
            ok = true,
            correct = correct,
            owner = cell.OwnerPlayerId,
            cellId = cellId,
            nextTurn = game.CurrentTurnPlayerId,
            team = game.Mode == "2v2" ? game.GetPlayerTeam(cell.OwnerPlayerId ?? -1) : null,
            isGameOver = game.IsGameOver,
            winnerId = game.WinnerId,
            winnerTeam = game.WinnerTeam
        });
    }


    private int GenerateRandomGameId()
    {
        return Random.Shared.Next(100000, 999999);
    }
}