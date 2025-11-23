using EP_Kviz.Services;
using Microsoft.AspNetCore.Mvc;

namespace EP_Kviz.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoresController : ControllerBase
    {
        private readonly IScoreService _scoreService;
        private readonly ILogger<ScoresController> _logger;

        public ScoresController(IScoreService scoreService, ILogger<ScoresController> logger)
        {
            _scoreService = scoreService;
            _logger = logger;
        }

        /// <summary>
        /// Přidá nový záznam skóre
        /// POST /api/scores
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddScore([FromBody] AddScoreRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { error = "Request body je prázdný" });
                }

                if (request.ElapsedMs <= 0)
                {
                    return BadRequest(new { error = "ElapsedMs musí být větší než 0" });
                }

                if (string.IsNullOrWhiteSpace(request.PlayerId) && string.IsNullOrWhiteSpace(request.Username))
                {
                    return BadRequest(new { error = "Musí být zadáno buď playerId nebo username" });
                }

                var scoreEntry = await _scoreService.AddScoreAsync(
                    request.PlayerId,
                    request.Username,
                    request.ElapsedMs,
                    request.GameMode
                );

                return Ok(new
                {
                    success = true,
                    scoreId = scoreEntry.Id,
                    message = "Skóre úspěšně uloženo"
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Neplatný požadavek na přidání skóre: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při přidávání skóre");
                return StatusCode(500, new { error = "Interní chyba serveru" });
            }
        }

        /// <summary>
        /// Získá žebříček nejlepších skóre
        /// GET /api/scores?top=50&gameMode=Procvičení
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetScores([FromQuery] int? top, [FromQuery] int? page, [FromQuery] string? gameMode)
        {
            try
            {
                List<Models.ScoreEntry> scores;
                int startRank = 1;

                if (page.HasValue)
                {
                    // Stránkování
                    int pageSize = top ?? 50;
                    scores = await _scoreService.GetScoresAsync(page.Value, pageSize, gameMode);
                    startRank = (page.Value - 1) * pageSize + 1;
                }
                else
                {
                    // Top N výsledků
                    scores = await _scoreService.GetTopScoresAsync(top ?? 50, gameMode);
                }

                var result = scores.Select((s, index) => new
                {
                    rank = startRank + index,
                    id = s.Id,
                    playerId = s.PlayerId,
                    username = s.Username ?? $"Hráč {s.PlayerId ?? "Anonym"}",
                    elapsedMs = s.ElapsedMs,
                    score = s.Score,
                    gameMode = s.GameMode,
                    createdAt = s.CreatedAt,
                    timeFormatted = FormatTime(s.ElapsedMs)
                });

                return Ok(new
                {
                    success = true,
                    count = scores.Count,
                    scores = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při načítání skóre");
                return StatusCode(500, new { error = "Interní chyba serveru" });
            }
        }

        private static string FormatTime(long elapsedMs)
        {
            var timeSpan = TimeSpan.FromMilliseconds(elapsedMs);
            return $"{(int)timeSpan.TotalMinutes:00}:{timeSpan.Seconds:00}.{timeSpan.Milliseconds:000}";
        }
    }

    public class AddScoreRequest
    {
        public string? PlayerId { get; set; }
        public string? Username { get; set; }
        public long ElapsedMs { get; set; }
        public string? GameMode { get; set; }
    }
}
