using System.ComponentModel.DataAnnotations;

namespace EP_Kviz.Models
{
    public class ScoreEntry
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// FK na UzivateleModel (nullable - pro anonymní hráče)
        /// </summary>
        public string? PlayerId { get; set; }

        /// <summary>
        /// Uživatelské jméno (nullable - pro duplicitní nebo anonymní záznamy)
        /// </summary>
        [MaxLength(100)]
        public string? Username { get; set; }

        /// <summary>
        /// Skóre pro jednoduché třídění (uloženo v ms, stejné jako ElapsedMs)
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Přesný čas v milisekundách
        /// </summary>
        public long ElapsedMs { get; set; }

        /// <summary>
        /// Datum a čas vytvoření záznamu
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Herní mód (Procvičení, 1v1, 2v2)
        /// </summary>
        [MaxLength(50)]
        public string? GameMode { get; set; }
    }
}
