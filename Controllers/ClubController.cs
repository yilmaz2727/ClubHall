using System.Threading.Tasks;
using ClubHall.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using OgrenciKulupSistemi.Data;

namespace OgrenciKulupSistemi.Controllers
{
    public class ClubController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClubController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET : Clubs
        public async Task<IActionResult> Index()
        {
            var clubs = await _context.Clubs.ToListAsync();
            return View(clubs);
        }

        // GET : Club/Details/
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var club = await _context.Clubs.Include(c=>c.Events).FirstOrDefaultAsync(m => m.Id == id);
            if (club == null)
            {
                return NotFound();
            }
            return View(club);
        }





    }

}