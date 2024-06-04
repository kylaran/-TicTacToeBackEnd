using TicTakToe.Entity.BaseModels;

namespace TicTakToe.Entity.Models.Vm
{
    //Вывод игры из списка игр
    public class GameInList
    {
        public int Id { get; set; }
        public int CreaterId { get; set; }
        public string? CreaterPhotoUrl { get; set; }
        public string CreaterName { get; set; }
        public bool IsFree { get; set; }
        public double Price { get; set; }
        public int SizeTable { get; set; }
        public int WinSize { get; set; }
        public FirstStepEnum FirstStep { get; set; }
        public GameInList() { }
        public GameInList(Game game, User user)
        {
            Id = game.Id;
            CreaterId = user.IdTg.Value;
            CreaterPhotoUrl = user.Photo_url;
            CreaterName = user.First_name;
            IsFree = game.IsFreeGame;
            Price = game.PriceGame;
            SizeTable = game.Table.WallSize;
            WinSize = game.Table.WinSize;
            FirstStep = game.FirstStep;
        }

    }
}
