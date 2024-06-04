using TicTakToe.Entity.BaseModels;

namespace TicTakToe.Entity.Models.Vm
{
    // Объект ответа на запрос об историях пользователя
    public class GameHistoryVm
    {
        public int TotalCount { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public List<GameHistory>? Data { get; set; }
    }
}
