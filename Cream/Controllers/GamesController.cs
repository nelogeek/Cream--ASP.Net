using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cream.Data;
using Cream.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Cream.DTO;
using System.Security.Claims;

namespace Cream.Controllers
{
    [Authorize]
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _environment;

        public GamesController(ApplicationDbContext context, UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: Games
        public async Task<IActionResult> Index(int? forDeveloper)
        {
            var request = _context.Games
                .Include(g => g.Developer)
                .Include(g => g.Genre)
                .AsQueryable();

            if (forDeveloper != null)
                request = request.Where(g => g.DeveloperId == forDeveloper);

            var list = await request.ToListAsync();

            List<GameDTO> games = new List<GameDTO>();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            list.ForEach(g =>
            {
                var own = _context.UserGames
                .Any(ug => ug.GameId == g.Id && ug.UserId == userId);


                games.Add(new GameDTO(g, own));
            });

            return View(games);
        }

        public async Task<IActionResult> Library(string user)
        {
            var list = await _context.UserGames
                .Where(g => g.UserId == user)
                .Include(g => g.Game)
                .Include(g => g.Game.Genre)
                .Include(g => g.Game.Developer)
                .Select(g => g.Game)
                .ToListAsync();

            return View(list);
        }
        
        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Developer)
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            ViewData["DeveloperId"] = new SelectList(_context.Developers, "Id", "Name");
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name");
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,GenreId,DeveloperId,ReleaseDate,Price")] Game game)
        {
            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();

                var devGames = await _context.Games
                    .Where(g => g.DeveloperId == game.DeveloperId)
                    .GroupBy(g => g.GenreId)
                    .Select(gr => new
                    {
                        Genre = gr.Key,
                        Count = gr.Count()
                    })
                    .OrderByDescending(gr => gr.Count)
                    .FirstAsync();

                var dev = await _context.Developers
                    .FirstOrDefaultAsync(d => d.Id == game.DeveloperId);

                dev.GenreId = devGames.Genre;
                _context.Update(dev);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DeveloperId"] = new SelectList(_context.Developers, "Id", "Name", game.DeveloperId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", game.GenreId);
            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            ViewData["DeveloperId"] = new SelectList(_context.Developers, "Id", "Name", game.DeveloperId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", game.GenreId);
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,GenreId,DeveloperId,ReleaseDate,Price")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                    var devGames = _context.Games
                    .Where(g => g.DeveloperId == game.DeveloperId)
                    .GroupBy(g => g.Genre)
                    .Select(gr => new
                    {
                        Genre = gr.Key,
                        Count = gr.Count()
                    })
                    .OrderByDescending(gr => gr.Count)
                    .First();

                    var dev = await _context.Developers
                        .FirstOrDefaultAsync(d => d.Id == game.DeveloperId);

                    dev.Genre = devGames.Genre;
                    _context.Update(dev);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DeveloperId"] = new SelectList(_context.Developers, "Id", "Name", game.DeveloperId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", game.GenreId);
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Developer)
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Games == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Games'  is null.");
            }
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return (_context.Games?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> IncomeReport()
        {
            string path = _environment.WebRootPath;
            string fileName = "income-report-" + DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss") + ".txt";
            string fullPath = Path.Combine(path, "reports", fileName);

            var games = await _context.UserGames
                .Include(ug => ug.Game)
                .GroupBy(ug => ug.GameId)
                .Select(g => new
                {
                    GameId = g.Key,
                    Game = g.First().Game,
                    Total = g.First().Game.Price * g.Count()
                })
                .ToListAsync();


            await using (StreamWriter sw = new(fullPath, true))
            {
                sw.WriteLine("Games income: ");
                games.ForEach(game =>
                {   
                    sw.WriteLine("{0,15} | {1,15}$", game.Game.Name, game.Total);
                });
            }
            var file = System.IO.File.ReadAllBytes(fullPath);
            return File(file, "text/plain", fileName);
        }


        public async Task<IActionResult> ReportLastRates()
        {
            string path = _environment.WebRootPath;
            string fileName = "last-reviews-report-" + DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss") + ".txt";
            string fullPath = Path.Combine(path, "reports", fileName);

            var reviews = await _context.GameRates
                .Include(o => o.Game)
                .Include(o => o.Rate)
                .ThenInclude(r => r.Author)
                .Select(o => new
                {
                    game = o.Game,
                    rate = o.Rate
                })
                .Take(10)
                .ToListAsync();


            await using (StreamWriter sw = new(fullPath, true))
            {
                sw.WriteLine("Last reviews: ");
                reviews.ForEach(review =>
                {
                    sw.WriteLine("{0,15} | {1,15} | {2, 40} | {3, 1}*",
                        review.rate.Author.FullName, review.game.Name,
                        review.rate.Text, review.rate.Rating);
                });
            }
            var file = System.IO.File.ReadAllBytes(fullPath);
            return File(file, "text/plain", fileName);
        }

        public async Task<IActionResult> ReportAvgRate()
        {
            string path = _environment.WebRootPath;
            string fileName = "avg-rating-report-" + DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss") + ".txt";
            string fullPath = Path.Combine(path, "reports", fileName);

            var games = await _context.GameRates
                .Include(gr => gr.Game)
                .Include(gr => gr.Rate)
                .GroupBy(gr => gr.GameId)
                .Select(g => new
                {
                    Game = g.First().Game,
                    avgRate = (double)g.Sum(x => x.Rate.Rating) / g.Count()
                })
                .ToListAsync();


            await using (StreamWriter sw = new(fullPath, true))
            {
                sw.WriteLine("Games: ");
                sw.WriteLine("{0,30} | {1,6}", "Game", "Rating");
                games.ForEach(game =>
                {
                    sw.WriteLine("{0,30} | {1,6}*", 
                        game.Game.Name, game.avgRate.ToString("0.00"));
                });
            }
            var file = System.IO.File.ReadAllBytes(fullPath);
            return File(file, "text/plain", fileName);

        }
    }
}
