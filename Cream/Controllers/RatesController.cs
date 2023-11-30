using Cream.Data;
using Cream.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cream.Controllers
{
    public class RatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GameRates(int GameId)
        {
            var rates = await _context.GameRates
                .Where(gr => gr.GameId == GameId)
                .Include(gr => gr.Rate)
                .ThenInclude(r => r.Author)
                .Include(gr => gr.Game)
                .ToListAsync();

            return View(rates);
        }

        public async Task<IActionResult> DeveloperRates(int DevId)
        {
            var rates = await _context.DevelopersRates
                .Where(gr => gr.DeveloperId == DevId)
                .Include(gr => gr.Rate)
                .ThenInclude(r => r.Author)
                .Include(gr => gr.Developer)
                .ToListAsync();

            return View(rates);
        }

        public async Task<IActionResult> RateGame(int GameId)
        {
            ViewData["GameId"] = GameId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RateGame(int GameId, int Rating, string Text)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Rate rate = new Rate()
            {
                Rating = Rating,
                Text = Text,
                Date = DateTime.Now,
                AuthorId = userId 
            };

            GameRate gameRate = new GameRate()
            {
                GameId = GameId,
                Rate = rate
            };

            _context.Add(gameRate);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Games");
        }

        public async Task<IActionResult> RateDev(int DevId)
        {
            ViewData["DevId"] = DevId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RateDev(int DevId, int Rating, string Text)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Rate rate = new Rate()
            {
                Rating = Rating,
                Text = Text,
                Date = DateTime.Now,
                AuthorId = userId
            };

            DeveloperRate devRate = new DeveloperRate()
            {
                DeveloperId = DevId,
                Rate = rate
            };

            _context.Add(devRate);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Developers");
        }

        public async Task<IActionResult> DeleteGameRate(int id)
        {
            if (_context.GameRates == null)
            {
                return Problem("Entity set 'ApplicationDbContext.GameRates'  is null.");
            }
            var rate = await _context.GameRates.FindAsync(id);
            if (rate != null)
            {
                _context.GameRates.Remove(rate);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GameRates), rate.GameId);
        }

        public async Task<IActionResult> DeleteDevRate(int id)
        {
            if (_context.DevelopersRates == null)
            {
                return Problem("Entity set 'ApplicationDbContext.DevelopersRates'  is null.");
            }
            var rate = await _context.DevelopersRates.FindAsync(id);
            if (rate != null)
            {
                _context.DevelopersRates.Remove(rate);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DeveloperRates), rate.DeveloperId);
        }


    }
}
