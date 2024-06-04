namespace TicTakToe.Entity.Models.Dto
{
    //Запрос с данными пользователя из ТГ
    public class UserDto
    {
        public int Id { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? username { get; set; }
        public string? language_code { get; set; }
        public bool? Is_premium { get; set; }
        public bool? Added_to_attachment_menu { get; set; }
        public bool? Allows_write_to_pm { get; set; }
        public string? Photo_url { get; set; }
    }
}
