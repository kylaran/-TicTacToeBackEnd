namespace TicTakToe.Entity.BaseModels
{
    public class Table
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public Game? Game { get; set; }
        public int WallSize { get; set; }
        public int WinSize { get; set; }
        public CellEnum[]? Cells { get; set; }
    }
    public enum CellEnum
    {
        None = 0, //Пусто
        Tic = 1, //X
        Tac = 2 //O
    }
}
