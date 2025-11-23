using EP_Kviz.Services;
using Microsoft.AspNetCore.Mvc;

namespace EP_Kviz.Controllers
{
    public class ScoreboardController : Controller
    {
        private readonly IScoreService _scoreService;
        private readonly ILogger<ScoreboardController> _logger;

        public ScoreboardController(IScoreService scoreService, ILogger<ScoreboardController> logger)
        {
            _scoreService = scoreService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? gameMode, int page = 1)
        {
            try
            {
                ViewBag.GameMode = gameMode;
                ViewBag.CurrentPage = page;

                var scores = await _scoreService.GetScoresAsync(page, 50, gameMode);
                var totalCount = await _scoreService.GetScoreCountAsync(gameMode);
                
                ViewBag.TotalPages = (int)Math.Ceiling(totalCount / 50.0);
                
                return View(scores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba při načítání scoreboardu");
                ViewBag.Error = "Nepodařilo se načíst žebříček";
                return View(new List<Models.ScoreEntry>());
            }
        }
    }
}
