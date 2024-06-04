using TicTakToe.Entity.BaseModels;

namespace TicTakToe.Entity.Models.Vm
{
    public class UserBalanceVm
    {
        public double? FreeCoin { get; set; }
        public double? TonCoin { get; set; }
        public UserBalanceVm() { }
        public UserBalanceVm(User user)
        {
            FreeCoin = user.FreeCoin;
            TonCoin = user.TonCoin;
        }
    }
}
