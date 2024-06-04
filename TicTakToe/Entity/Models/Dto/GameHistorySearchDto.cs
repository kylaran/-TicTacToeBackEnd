namespace TicTakToe.Entity.Models.Dto
{
    //Запрос на список истории
    public class GameHistorySearchDto
    {
        public int UserId { get; set; }
        public int Take { get; set; } = 10;
        public int Skip { get; set; } = 0;
    }
}
