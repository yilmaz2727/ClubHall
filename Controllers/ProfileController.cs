using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OgrenciKulupSistemi.Models;
using OgrenciKulupSistemi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
namespace OgrenciKulupSistemi.Controllers
{

    public class ProfileController : Controller
    {


        // UserManager her zaman veritabanı asıl sınıfı (ApplicationUser) ile çalışır.
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }



        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User); // Retrieves the currently logged-in user
            return View(user);
        }

        /* 
         Important Note: The ApplicationUser.cs class represents the table in the database. UserEditViewModel is a temporary container.
         This class is only used when displaying a form to the user and retrieving data from the user.

         User Clicks "Edit" (GET):
         You retrieve ApplicationUser from the DB.
         You copy the data inside to UserEditViewModel.
         You show the user the HTML page filled with UserEditViewModel (the user never sees the DB object).

          User Changes Information and Clicks "Save" (POST):
          The data returns to the Controller as UserEditViewModel.
          You call ApplicationUser (the original record) from the DB again.
          This time you perform a reverse copy: You write the new data in ViewModel to ApplicationUser.
        */


        [HttpGet]
        public async Task<IActionResult> EditPersonalInfo()
        {
            var user = await _userManager.GetUserAsync(User); // Retrieves all information of the user currently logged into the site (ApplicationUser object)

           // The `user` variable contains many fields, including confidential information like `Password`. We cannot use it directly. We should only use the information it contains, such as `City` and `Birthplace`, for privacy reasons.
            var model = new UserEditViewModel // --> Veritabanından (ApplicationUser) -> Ekrana (UserEditViewModel) dönüştürüyoruz
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


           // We transfer the data coming from the screen (ViewModel) to the database object (ApplicationUser).
            user.City = model.City;
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            user.BirthPlace = model.BirthPlace;
            user.BirthDate = model.BirthDate;
            user.Gender = model.Gender;

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index");
        }


        public IActionResult MyEvents() // Events page that user's joined 
        {

            var userId = _userManager.GetUserId(User); // get user id 

            var events = _context.EventAttendees
                .Where(ea => ea.ApplicationUserId == userId)
                .Include(ea => ea.Event)
                .Select(ea => ea.Event)
                .ToList();   // Include accesses the bridge, while select accesses the event.

            var now = DateTime.Now;
            var UpCaming = events.Where(e => e.StartDate >= now).ToList(); // Take Upcoming Events 

            return View(UpCaming);
        }

        public IActionResult MyMemberShips() // Clubs that user's membered
        {
            var userId = _userManager.GetUserId(User); //get user id

            var Clubs = _context.ClubMemberships // found clubs that user's membered by ClubMemberships table 
                .Where(ea => ea.ApplicationUserId == userId)
                .Include(ea => ea.Club)
                .Select(ea => ea.Club)
                .ToList();   // Include accesses the bridge, while select` accesses the event..
            return View(Clubs);
        }

    }

}