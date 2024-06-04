using TicTakToe.Entity.BaseModels;

namespace TicTakToe.Entity.Models.Dto
{
    //запрос на ход
    public class MoveGameDto
    {
        public int GameId { get; set; }
        public int UserId { get; set; }
        public int NumberCell { get; set; }
        public CellEnum Cell { get; set; }
        public bool IsMoveWin { get; set; } = false;
        public bool isDrawGame { get; set; } = false;
        public bool? isBotWin { get; set; } = false;
    }
}
