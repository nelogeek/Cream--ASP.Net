using System.ComponentModel.DataAnnotations;

namespace Cream.Models
{
    public class Rate
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        [Range(0,5)]
        public int Rating { get; set; }
        public string Text { get; set; }
        public string AuthorId { get; set; }

        public virtual User? Author { get; set; }
    }
}
