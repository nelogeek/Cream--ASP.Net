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
using System.Security.Claims;
using Microsoft.Extensions.Hosting;

namespace Cream.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _environment;

        public OrdersController(ApplicationDbContext context, UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Orders
                .Include(o => o.Game)
                .Include(o => o.User)
                .OrderByDescending(o => o.Date);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Game)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GameId,Date")] Order order)
        {
            order.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            order.Date = DateTime.Now;
            if (ModelState.IsValid)
            {
                UserGame userGame = new UserGame()
                {
                    GameId = order.GameId,
                    UserId = order.UserId
                };

                _context.Add(userGame);
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Games");
            }
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name", order.GameId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", order.UserId);

            return RedirectToAction("Index", "Games");
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> Report()
        {
            string path = _environment.WebRootPath;
            string fileName = "last-orders-report-" + DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss") + ".txt";
            string fullPath = Path.Combine(path, "reports", fileName);

            var orders = await _context.Orders
                .Include(o => o.Game)
                .Include(o => o.User)
                .Select(o => new
                {
                    orderId = o.Number,
                    user = o.User.Id,
                    game  = o.Game.Name,
                    date = o.Date
                })
                .Take(10)
                .ToListAsync();


            await using (StreamWriter sw = new(fullPath, true))
            {
                sw.WriteLine("Last orders: ");
                orders.ForEach(order =>
                {
                    sw.WriteLine("{0,10} | {1,34} | {2, 20} | {3, 6}", 
                        order.date.ToShortDateString(), order.user,  order.game, order.orderId);
                });
            }
            var file = System.IO.File.ReadAllBytes(fullPath);
            return File(file, "text/plain", fileName);
        }
    }
}
