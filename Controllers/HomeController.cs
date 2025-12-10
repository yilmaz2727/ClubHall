using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OgrenciKulupSistemi.Models;
using Microsoft.AspNetCore.Identity;
using OgrenciKulupSistemi.Data;
using Microsoft.EntityFrameworkCore;

namespace OgrenciKulupSistemi.Controllers;

public class HomeController : Controller
{
  
    private readonly UserManager<ApplicationUser> _userManager;
  private readonly ApplicationDbContext _context;

        public HomeController(UserManager<ApplicationUser> userManager,ApplicationDbContext context)
        {
            _userManager = userManager;
            _context =context;
        }

   public async Task<ActionResult> Index()
    {  int eventCount = 0;
    int membershipsCount=0;
         ApplicationUser user = null; //Give a default value so that you don't get an error if the user is not logged in.
            var userId = _userManager.GetUserId(User);
            user = await _userManager.FindByIdAsync(userId);
             if (userId != null)
    {
        eventCount = await _context.EventAttendees.CountAsync(ea => ea.ApplicationUserId == userId);
        membershipsCount = await _context.ClubMemberships.CountAsync(ea => ea.ApplicationUserId == userId);
    }
    ViewBag.ClubCount = membershipsCount;
    ViewBag.EventCount = eventCount;

     ViewBag.UpcomingEvents = await _context.Events
        .OrderBy(e => e.StartDate)
        .Take(4)
        .ToListAsync();



       return View(user);
    }

  
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
