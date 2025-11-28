using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OgrenciKulupSistemi.Models;

namespace OgrenciKulupSistemi.Controllers
{

    public class ProfileController : Controller
    {


        // UserManager her zaman veritabanı asıl sınıfı (ApplicationUser) ile çalışır.
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


        [HttpGet]
        public async Task<IActionResult> EditPersonalInfo()
        {
            var user = await _userManager.GetUserAsync(User);

            // Veritabanından (ApplicationUser) -> Ekrana (UserEditViewModel) dönüştürüyoruz
            var model = new UserEditViewModel
            {
                City = user.City,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                BirthPlace = user.BirthPlace,
                BirthDate = user.BirthDate,
                Gender = user.Gender,

            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EditProfile(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EditPersonalInfo", model);
            }

            var user = await _userManager.GetUserAsync(User);


            // Ekrandan gelen verileri (ViewModel) -> Veritabanı nesnesine (ApplicationUser) aktarıyoruz
            user.City = model.City;
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            user.BirthPlace = model.BirthPlace;
            user.BirthDate = model.BirthDate;
            user.Gender = model.Gender;

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index");
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