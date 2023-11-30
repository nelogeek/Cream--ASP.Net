using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cream.Data;
using Cream.Models;

namespace Cream.Controllers
{
    public class UserGamesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserGamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserGames
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UserGames.Include(u => u.Game).Include(u => u.User);
            return View(await applicationDbContext.ToListAsync());
        }

        
    }
}
