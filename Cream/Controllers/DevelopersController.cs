using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cream.Data;
using Cream.Models;
using Microsoft.Extensions.Hosting;

namespace Cream.Controllers
{
    public class DevelopersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DevelopersController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Developers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Developers.Include(d => d.Genre);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Developers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Developers == null)
            {
                return NotFound();
            }

            var developer = await _context.Developers
                .Include(d => d.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (developer == null)
            {
                return NotFound();
            }

            return View(developer);
        }

        // GET: Developers/Create
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name");
            return View();
        }

        // POST: Developers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,GenreId")] Developer developer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(developer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", developer.GenreId);
            return View(developer);
        }

        // GET: Developers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Developers == null)
            {
                return NotFound();
            }

            var developer = await _context.Developers.FindAsync(id);
            if (developer == null)
            {
                return NotFound();
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", developer.GenreId);
            return View(developer);
        }

        // POST: Developers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,GenreId")] Developer developer)
        {
            if (id != developer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(developer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeveloperExists(developer.Id))
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
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", developer.GenreId);
            return View(developer);
        }

        // GET: Developers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Developers == null)
            {
                return NotFound();
            }

            var developer = await _context.Developers
                .Include(d => d.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (developer == null)
            {
                return NotFound();
            }

            return View(developer);
        }

        // POST: Developers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Developers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Developers'  is null.");
            }
            var developer = await _context.Developers.FindAsync(id);
            if (developer != null)
            {
                _context.Developers.Remove(developer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeveloperExists(int id)
        {
          return (_context.Developers?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> ReportAvgRate()
        {
            string path = _environment.WebRootPath;
            string fileName = "avg-rating-report-" + DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss") + ".txt";
            string fullPath = Path.Combine(path, "reports", fileName);

            var developers = await _context.DevelopersRates
                .Include(gr => gr.Developer)
                .Include(gr => gr.Rate)
                .GroupBy(gr => gr.DeveloperId)
                .Select(g => new
                {
                    Developer = g.First().Developer,
                    avgRate = (double)g.Sum(x => x.Rate.Rating) / g.Count()
                })
                .ToListAsync();


            await using (StreamWriter sw = new(fullPath, true))
            {
                sw.WriteLine("{0,30} | {1,6}", "Game", "Rating");
                developers.ForEach(game =>
                {
                    sw.WriteLine("{0,30} | {1,6}*",
                        game.Developer, game.avgRate.ToString("0.00"));
                });
            }
            var file = System.IO.File.ReadAllBytes(fullPath);
            return File(file, "text/plain", fileName);

        }
    }
}
