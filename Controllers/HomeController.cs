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
    var model = new HomeIndexViewModel();
   model.UpcomingEvents = await _context.Events
        .Where(e => e.StartDate >= DateTime.Today)
        .OrderBy(e => e.StartDate)
        .Take(4) //4 etkinlikk
        .ToListAsync();

    model.ShowClub = await _context.Clubs
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
