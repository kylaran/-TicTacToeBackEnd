using TicTakToe.Entity.BaseModels;

namespace TicTakToe.Entity.Models.Vm
{
    // Вывод таблицы игры
    public class TableVm
    {
        public int Id { get; set; }
        public int WallSize { get; set; }
        public int WinSize { get; set; }
        public CellEnum[]? Cells { get; set; }
        public TableVm() { }
        public TableVm(Table table)
        {
            Id = table.Id;
            WallSize = table.WallSize;
            WinSize = table.WinSize;
            Cells = table.Cells;
        }
    }
}
