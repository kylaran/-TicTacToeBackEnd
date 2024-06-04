using System.ComponentModel.DataAnnotations;
using TicTakToe.Entity.Models.Dto;

namespace TicTakToe.Entity.BaseModels
{
    public class User
    {
        public int Id { get; set; }
        public int? IdTg { get; set; }
        public bool? Is_bot { get; set; }
        public string? First_name { get; set; }
        public string? Last_name { get; set; }
        public string? Username { get; set; }
        public string? Language_code { get; set; }
        public bool? Is_premium { get; set; }
        public bool? Added_to_attachment_menu { get; set; }
        public bool? Allows_write_to_pm { get; set; }
        [MaxLength]
        public string? Photo_url { get; set; }
        public double? FreeCoin { get; set; }
        public double? TonCoin { get; set; }
        public bool SeenChanges { get; set; } = false;
        public DateTime? LastGiftReceived { get; set; }

        public User () { }
        public User (UserDto user)
        {
            IdTg = user.Id;
            Is_bot = false;
            First_name = user.first_name;
            Last_name = user.last_name;
            Username = user.username;
            Language_code = user.language_code;
            Is_premium = user.Is_premium;
            Added_to_attachment_menu = user.Added_to_attachment_menu;
            Allows_write_to_pm = user.Allows_write_to_pm;
            Photo_url = user.Photo_url;
            FreeCoin = 50;
            TonCoin = 0;
        }
    }
}
