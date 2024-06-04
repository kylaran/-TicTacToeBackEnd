using TicTakToe.Entity.BaseModels;

namespace TicTakToe.Entity.Models.Vm
{
    //Вывод игры
    public class GameVm
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public ParticipantVm ParticipantCreater { get; set; }
        public ParticipantVm? ParticipantJoined { get; set; }
        public bool IsFreeGame { get; set; }
        public TableVm Table { get; set; }
        public StatusGameEnum Status { get; set; }
        public int LastMoveFrom { get; set; }
        public DateTime LastMoveDate { get; set; }
        public int MoveTime { get; set; }
        public bool IsPrivateGame { get; set; }
        public double PriceGame { get; set; }
        public FirstStepEnum FirstStep { get; set; }
        public bool WithBot { get; set; }
        public GameVm() { }
        public GameVm(Game game, ParticipantVm participantCreater, ParticipantVm? participantJoined)
        {
            Id = game.Id;
            CreatedDate = game.CreatedDate;
            ParticipantCreater = participantCreater;
            ParticipantJoined = participantJoined;
            IsFreeGame = game.IsFreeGame;
            Table = new TableVm(game.Table);
            Status = game.Status;
            LastMoveFrom = game.LastMoveFrom;
            LastMoveDate = game.LastMoveDate;
            MoveTime = game.MoveTime;
            IsPrivateGame = game.IsPrivateGame;
            PriceGame = game.PriceGame;
            FirstStep = game.FirstStep;
            WithBot = game.WithBot;
        }
    }
}
