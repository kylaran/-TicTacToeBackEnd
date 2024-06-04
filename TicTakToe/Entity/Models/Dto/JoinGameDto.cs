namespace TicTakToe.Entity.Models.Dto
{
    //Запрос присоединения к игре
    public class JoinGameDto
    {
        public int UserId { get; set; }
        public int GameId { get; set; }
    }
}
