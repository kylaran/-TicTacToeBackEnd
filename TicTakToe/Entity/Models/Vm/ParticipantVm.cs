using TicTakToe.Entity.BaseModels;

namespace TicTakToe.Entity.Models.Vm
{
    //Объект участника игры
    public class ParticipantVm
    {
        public int UserId { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Photo_url { get; set; }
        public double FreeCoin { get; set; }
        public double TonCoin { get; set; }
        public ParticipantVm() { }
        public ParticipantVm(User user)
        {
            UserId = user.IdTg.Value;
            First_name = user.First_name;
            Last_name = user.Last_name;
            Photo_url = user.Photo_url;
            FreeCoin = user.FreeCoin.Value;
            TonCoin = user.TonCoin.Value;
        }

    }
}
