using System.ComponentModel.DataAnnotations;
using TicTakToe.Entity.BaseModels;

namespace TicTakToe.Entity.Models.Vm
{
    // Вывод юзера на запросы о нём
    public class UserVm
    {
        public int? Id { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? username { get; set; }
        public string? language_code { get; set; }
        public bool? Is_premium { get; set; }
        public bool? Added_to_attachment_menu { get; set; }
        public bool? Allows_write_to_pm { get; set; }
        [MaxLength]
        public string? Photo_url { get; set; }
        public double? FreeCoin { get; set; }
        public double? TonCoin { get; set; }
        public DateTime? LastGiftRecevied { get; set; }
        public UserVm() { }
        public UserVm(User user)
        {
            Id = user.IdTg;
            first_name = user.First_name;
            last_name = user.Last_name;
            username = user.Username;
            language_code = user.Language_code;
            Is_premium = user.Is_premium;
            Added_to_attachment_menu = user.Added_to_attachment_menu;
            Allows_write_to_pm = user.Allows_write_to_pm;
            Photo_url = user.Photo_url;
            FreeCoin = user.FreeCoin;
            TonCoin = user.TonCoin;
            LastGiftRecevied = user.LastGiftReceived;
        }
    }
}
