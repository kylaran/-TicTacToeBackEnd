using TicTakToe.Entity.Models.Dto;

namespace TicTakToe.Entity.Models.Vm
{
    //Вывод списка игр с фильтром ответным
    public class ListGamesVm
    {
        public int TotalCount { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public List<GameInList> Data { get; set; }
        public SearchGameDto filter { get; set; }
    }
}
