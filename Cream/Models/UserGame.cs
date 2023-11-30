namespace Cream.Models
{
    public class UserGame
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int? GameId { get; set; }

        public virtual User? User { get; set; }
        public virtual Game? Game { get; set; }


    }
}
