namespace TicTakToe.Entity.BaseModels
{
    //История игр
    public class GameHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
        public DateTime FinishedDate { get; set; }
        public double FinancialMovment { get; set; }
        public bool IsFreeGame { get; set; }
        public bool IsWin {  get; set; }
        public string? OponentName { get; set; }
        public string? OponentPhoto { get; set; }
        public StatusGameEnum StatusGame { get; set; }
    }
}
