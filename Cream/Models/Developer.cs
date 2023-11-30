namespace Cream.Models
{
    public class Developer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int? GenreId { get; set; }

        public virtual Genre? Genre { get; set; } 
        public virtual List<Game>? Games { get; set; }
    }
}
