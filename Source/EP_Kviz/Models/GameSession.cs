namespace EP_Kviz.Models
{
    public class GameSession
    {
        public int GameId { get; set; }
        public string Mode { get; set; }
        public List<int> Players { get; set; } = new List<int>();
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}