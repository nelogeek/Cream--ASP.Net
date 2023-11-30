using Cream.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Cream.DTO;

namespace Cream.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Developer> Developers { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Order> Orders { get; set; } 
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<UserGame> UserGames { get; set; }
        public DbSet<GameRate> GameRates { get; set; }
        public DbSet<DeveloperRate> DevelopersRates { get; set; }

        

    }
}