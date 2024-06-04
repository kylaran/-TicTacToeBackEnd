using TicTakToe.Entity.BaseModels;

namespace TicTakToe.Entity.Models.Vm.SSE
{
    public class OverDataVm
    {
        public int? whoWin { get; set; }
        public StatusGameEnum statusGame { get; set; }
        public int lastMove { get; set; }
    }
}
