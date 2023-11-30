namespace Cream.Models
{
    public class DeveloperRate
    {
        public int Id { get; set; }
        public int DeveloperId { get; set; }
        public int RateId { get; set; }

        public virtual Developer? Developer { get; set; }
        public virtual Rate? Rate { get; set; }
    }
}
