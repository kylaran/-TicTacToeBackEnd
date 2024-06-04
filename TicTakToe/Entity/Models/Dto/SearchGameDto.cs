namespace TicTakToe.Entity.Models.Dto
{
    //Запрос для списка игр
    public class SearchGameDto
    {
        public int Take { get; set; } = 10;
        public int Skip { get; set; } = 0;
        public double PriceFrom { get; set; } = 0;
        public double PriceTo { get; set; } = 10000000;
        public int WinSizeFrom { get; set; } = 3;
        public int WinSizeTo { get; set; } = 20;
        public int WallSizeFrom { get; set; } = 3;
        public int WallSizeTo { get; set; } = 20;
    }
}
