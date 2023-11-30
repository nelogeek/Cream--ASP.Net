namespace Cream.Models
{
    public class Refund
    {
        public int Id { get; set; } 
        public int? OrderId { get;set; }
        public string? UserId { get; set; }
        public int? GameId { get; set; }
        public DateTime Date { get; set; }
        public string Reason { get; set; } 

        public virtual Order? Order { get; set; }
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
