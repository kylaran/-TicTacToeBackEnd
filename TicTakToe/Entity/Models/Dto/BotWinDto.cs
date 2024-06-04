using TicTakToe.Entity.BaseModels;

namespace TicTakToe.Entity.Models.Dto
{
    public class BotWinDto
    {
        public int GameId { get; set; }
        public StatusGameEnum Status { get; set; }
    }
}