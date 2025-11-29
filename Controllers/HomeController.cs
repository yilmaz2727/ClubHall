using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OgrenciKulupSistemi.Models;
using Microsoft.AspNetCore.Identity;


namespace OgrenciKulupSistemi.Controllers;

public class HomeController : Controller
{
  
    private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

   public async Task<ActionResult> Index()
    {
         ApplicationUser user = null; //Give a default value so that you don't get an error if the user is not logged in.
       
            var userId = _userManager.GetUserId(User);
            user = await _userManager.FindByIdAsync(userId);
       return View(user);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
