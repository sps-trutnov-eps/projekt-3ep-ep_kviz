using EP_Kviz.Models;

namespace EP_Kviz.Services
{
    public interface IScoreService
    {
        /// <summary>
        /// Přidá nový záznam skóre do databáze
        /// </summary>
        Task<ScoreEntry> AddScoreAsync(string? playerId, string? username, long elapsedMs, string? gameMode);

        /// <summary>
        /// Získá top N záznamů seřazených podle nejlepšího času
        /// </summary>
        Task<List<ScoreEntry>> GetTopScoresAsync(int top = 50, string? gameMode = null);

        /// <summary>
        /// Získá všechna skóre se stránkováním
        /// </summary>
        Task<List<ScoreEntry>> GetScoresAsync(int page = 1, int pageSize = 50, string? gameMode = null);

        /// <summary>
        /// Získá celkový počet záznamů
        /// </summary>
        Task<int> GetScoreCountAsync(string? gameMode = null);
    }
}
