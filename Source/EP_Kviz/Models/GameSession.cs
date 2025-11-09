namespace EP_Kviz.Models
{
    public class Cell
    {
        public int Id { get; set; }           // 0..24
        public string Label { get; set; }    // "A", "B", ...
        public int? OwnerPlayerId { get; set; } = null;
        public bool IsAnswered { get; set; } = false;
    }

    public class PendingQuestion
    {
        public int CellId { get; set; }
        public int AskedByPlayerId { get; set; }
        public string QuestionText { get; set; }
        public string Answer { get; set; } // správná část (po '/')
    }

    public class GameSession
    {
        public int GameId { get; set; }
        public string Mode { get; set; }
        public List<int> Players { get; set; } = new List<int>();
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Nové pro hru
        public List<Cell> Grid { get; set; } = new List<Cell>();
        public int? CurrentTurnPlayerId { get; set; } = null;
        public PendingQuestion PendingQuestion { get; set; } = null;
    }
}