using TicTakToe.Entity.BaseModels;

namespace TicTakToe.Entity.Models.Dto
{
    //Запрос создания игры
    public class CreateGameDto
    {
        public int UserId { get; set; }
        public bool IsFreeGame { get; set; }
        public bool IsPrivateGame { get; set; }
        public bool WithBot { get; set; }
        public int WallSize { get; set; }
        public int WinSize { get; set; }
        public int MoveTime { get; set; }
        public double Price { get; set; }
        public FirstStepEnum FirstStep { get; set; }
    }
}
