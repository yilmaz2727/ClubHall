using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OgrenciKulupSistemi.Models;

namespace OgrenciKulupSistemi.Controllers
{

    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

      public async Task<IActionResult> Index()
     {
    var user = await _userManager.GetUserAsync(User);
    return View(user);   
      }


        public IActionResult MyEvents()
        {
            return View();
        }
        
        public IActionResult MyMemberShips()
        {
            return View();
        }

    }

}