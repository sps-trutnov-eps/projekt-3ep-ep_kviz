using EP_Kviz.Models;
using Microsoft.EntityFrameworkCore;

namespace EP_Kviz.Services
{
    public class ScoreService : IScoreService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ScoreService> _logger;

        public ScoreService(AppDbContext context, ILogger<ScoreService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ScoreEntry> AddScoreAsync(string? playerId, string? username, long elapsedMs, string? gameMode)
        {
            // Validace
            if (elapsedMs <= 0)
            {
                throw new ArgumentException("ElapsedMs musí být větší než 0", nameof(elapsedMs));
            }

            if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(playerId))
            {
                throw new ArgumentException("Musí být zadáno buď playerId nebo username");
            }

            if (!string.IsNullOrWhiteSpace(username) && username.Length > 100)
            {
                throw new ArgumentException("Username nesmí být delší než 100 znaků", nameof(username));
            }

            // Vytvoření nového záznamu
            var scoreEntry = new ScoreEntry
            {
                PlayerId = playerId,
                Username = username,
                ElapsedMs = elapsedMs,
                Score = (int)elapsedMs, // Score = ElapsedMs pro jednoduché třídění
                GameMode = gameMode,
                CreatedAt = DateTime.UtcNow
            };

            _context.Scores.Add(scoreEntry);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Nový score záznam přidán: Id={Id}, PlayerId={PlayerId}, Username={Username}, ElapsedMs={ElapsedMs}, GameMode={GameMode}", 
                scoreEntry.Id, scoreEntry.PlayerId, scoreEntry.Username, scoreEntry.ElapsedMs, scoreEntry.GameMode);

            return scoreEntry;
        }

        public async Task<List<ScoreEntry>> GetTopScoresAsync(int top = 50, string? gameMode = null)
        {
            var query = _context.Scores.AsQueryable();

            if (!string.IsNullOrWhiteSpace(gameMode))
            {
                query = query.Where(s => s.GameMode == gameMode);
            }

            return await query
                .OrderBy(s => s.ElapsedMs) // Nižší čas = lepší
                .Take(top)
                .ToListAsync();
        }

        public async Task<List<ScoreEntry>> GetScoresAsync(int page = 1, int pageSize = 50, string? gameMode = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;
            if (pageSize > 100) pageSize = 100;

            var query = _context.Scores.AsQueryable();

            if (!string.IsNullOrWhiteSpace(gameMode))
            {
                query = query.Where(s => s.GameMode == gameMode);
            }

            return await query
                .OrderBy(s => s.ElapsedMs)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetScoreCountAsync(string? gameMode = null)
        {
            var query = _context.Scores.AsQueryable();

            if (!string.IsNullOrWhiteSpace(gameMode))
            {
                query = query.Where(s => s.GameMode == gameMode);
            }

            return await query.CountAsync();
        }
    }
}
