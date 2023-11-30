namespace Cream.Models
{
    public class GameRate
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int RateId { get; set; }

        public virtual Game? Game { get; set; } 
        public virtual Rate? Rate { get; set; }
    }
}
