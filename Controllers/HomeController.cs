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
{
    var model = new HomeIndexViewModel();// get userid and upcoming event
   model.UpcomingEvents = await _context.Events
        .Where(e => e.StartDate >= DateTime.Today)
        .OrderBy(e => e.StartDate)
        .Take(6) //6 event
        .ToListAsync();

    model.ShowClub = await _context.Clubs // order by event 
    .OrderBy(c => c.Name)
    .Take(6)
    .ToListAsync();   

    return View(model);
}

  
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
