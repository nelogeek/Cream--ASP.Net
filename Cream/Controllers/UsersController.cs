using Cream.Data;
using Cream.DTO;
using Cream.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cream.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UsersController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = _context.Users
                .Select(u => new UserDTO()
                {
                    Id = u.Id,
                    FullName= u.FullName,
                    Email = u.Email
                });

            return View(users);
        }
    }
}
