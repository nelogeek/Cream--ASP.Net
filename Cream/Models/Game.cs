namespace Cream.Models
{
    public class Game
    {
        public int Id { get; set; } 
        public string Name { get; set; }    
        public int? GenreId { get; set; }
        public int? DeveloperId { get; set; }
        public DateTime ReleaseDate { get; set; }
        public double Price { get; set; }


        public virtual Genre? Genre { get; set; }   
        public virtual Developer? Developer { get;set; }
    }
}
