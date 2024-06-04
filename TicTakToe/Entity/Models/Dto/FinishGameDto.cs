namespace TicTakToe.Entity.Models.Dto
{
    // Запрос завершить игру по причине времени
    public class FinishGameDto
    {
        public int GameId { get; set; }
        public int UserId { get; set; }
    }
}
