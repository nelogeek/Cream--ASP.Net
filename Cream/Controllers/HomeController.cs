using Cream.Data;
using Cream.DTO;
using Cream.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Cream.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment, ApplicationDbContext context)
        {
            _logger = logger;
            _environment = environment;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var gameReviews = await _context.GameRates
            .Include(gr => gr.Game)
            .Include(gr => gr.Rate)
            .GroupBy(gr => gr.GameId)
            .Select(g => new GameTopDTO
            {
                Game = g.First().Game.Name,
                Rate = (double)g.Sum(x => x.Rate.Rating) / g.Count()
            })
            .OrderByDescending(g => g.Rate)
            .Take(10)
            .ToListAsync();

            var gameCopies = await _context.UserGames
                .Include(ug => ug.Game)
                .GroupBy(ug => ug.GameId)
                .Select(g => new GameTopDTO
                {
                    Game = g.First().Game.Name,
                    Rate = g.Count()
                })
                .Take(10)
                .ToListAsync();

            ViewData["gameReviews"] = gameReviews;
            ViewData["gameCopies"] = gameCopies;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}