using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cream.Data;
using Cream.Models;
using Microsoft.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Cream.Controllers
{
    public class RefundsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RefundsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Refunds
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Refunds
                .Include(r => r.Game)
                .Include(r => r.Order)
                .Include(r => r.User)
                .OrderByDescending(o => o.Date);
            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Refunds/Create
        public async Task<IActionResult> Request(int GameId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orderId = await _context.Orders.FirstOrDefaultAsync(o => o.GameId == GameId && o.UserId == userId);

            ViewData["OrderId"] = orderId.Id;
            ViewData["GameId"] = GameId;
            return View("Create");
        }

        // POST: Refunds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int OrderId, int GameId, string Reason)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Refund refund = new()
            {
                OrderId = OrderId,
                GameId = GameId,
                UserId = userId,
                Reason = Reason,
                Date = DateTime.Now
            };

            var userGame = await _context.UserGames.FirstOrDefaultAsync(ug => ug.GameId == GameId && ug.UserId == userId);
            if (userGame == null)
                return NotFound();

            _context.Remove(userGame);
            _context.Add(refund);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Games");
        }

        private bool RefundExists(int id)
        {
            return (_context.Refunds?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
