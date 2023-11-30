using Cream.Models;

namespace Cream.DTO
{
    public class GameDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Genre { get; set; }
        public string? Developer { get; set; }
        public DateTime ReleaseDate { get; set; }
        public double Price { get; set; }
        public bool IsOwned { get; set; }

        public GameDTO(Game game, bool isOwned)
        {
            this.Id = game.Id;
            this.Name = game.Name;
            this.Genre = game.Genre != null ? game.Genre.Name : game.GenreId.ToString();
            this.Developer = game.Developer != null ? game.Developer.Name : game.DeveloperId.ToString();
            this.ReleaseDate = game.ReleaseDate;
            this.Price = game.Price;
            this.IsOwned = isOwned;
        }

        public GameDTO()
        {

        }
    }
}
