namespace TicTakToe.Entity.BaseModels
{
    public class Game
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int ParticipantCreater { get; set; }
        public int? ParticipantJoined { get; set; }
        public bool IsFreeGame { get; set; } = true;
        public int TableId { get; set; }
        public Table Table { get; set; }
        public StatusGameEnum Status { get; set; }
        public int LastMoveFrom { get; set; }
        public bool LastMoveFromSee { get; set; } = false;
        public DateTime LastMoveDate { get; set; }
        public int? LastMoveIndex { get; set; }
        public int MoveTime { get; set; } = 11; //время ход в секундах
        public bool IsPrivateGame { get; set; } = false;
        public double PriceGame { get; set; }
        public bool WithBot { get; set; } = false;
        public FirstStepEnum FirstStep { get; set; } = FirstStepEnum.Random;
    }
    public enum StatusGameEnum
    {
        WaitingOpponent = 0, //Ожидание опонента
        GameOn = 1, // Идёт игра
        GameOver = 2, // Игра завершилась
        GameOverGaveUp = 3, // Игра завершилась по причине сдачи
        GameFailed = 4, // игра завершилась из-за законченного времени
        GameOverDraw = 5, // игра завершилась в Ничью
        GameClosed = 6 // игра закрыта, не дождался опонента
    }
    public enum FirstStepEnum
    {
        Random = 0,
        Creater = 1,
        Joiner = 2
    }
}
