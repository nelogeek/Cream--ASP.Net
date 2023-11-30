namespace Cream.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int? GameId { get; set; }
        public DateTime Date { get; set; }

        public virtual User? User { get; set; }
        public virtual Game? Game { get; set; }

        public string Number
        {
            get
            {
                return $"№{Id.ToString("0000")}";
            }
        }
    }
}
