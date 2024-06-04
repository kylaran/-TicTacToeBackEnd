namespace TicTakToe.Entity.Models.Dto
{
    //Добавить бесплатных монет пользователю запрос
    public class AddBalanceDto
    {
        public int UserId { get; set; }
        public double FreeCoin { get; set; }
    }
}
