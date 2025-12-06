using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using OgrenciKulupSistemi.Data;
using Microsoft.AspNetCore.Identity;
using OgrenciKulupSistemi.Models;

using Microsoft.AspNetCore.Authentication;
namespace OgrenciKulupSistemi.Controllers
{
    public class ClubController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClubController(ApplicationDbContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager=userManager;
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
            var club = await _context.Clubs.Include(c => c.Events).FirstOrDefaultAsync(m => m.Id == id);
            if (club == null)
            {
                return NotFound();
            }
            return View(club);
        }

        public async Task<IActionResult> ClubJoin(int clubId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Challenge(new AuthenticationProperties// kullanıcı → login ekranına yönlendir daha sornasında detaile gönderir
                {
                    RedirectUri= Url.Action("Details",new {id=clubId}) 
                });
            }   
            bool alreadyJoined = await _context.ClubMemberships.AnyAsync(x => x.ClubId  == clubId && x.ApplicationUserId == userId);
            if (alreadyJoined)
            {
                TempData["Message"] = "You already join this Club";
               return RedirectToAction("Details", new { id = clubId });
             
            }
            var registration = new ClubMembership
            {
                ClubId = clubId,
                ApplicationUserId = userId,
                JoinDate = DateTime.UtcNow
            };
            _context.ClubMemberships.Add(registration);
            await _context.SaveChangesAsync();
             TempData["Message2"] = "You successfully join this Club.";
 
            return RedirectToAction("Details", new { id = clubId });
        }

    
































    }

}