using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OgrenciKulupSistemi.Data;
using OgrenciKulupSistemi.Models;
using OgrenciKulupSistemi.ViewModels;
using Microsoft.AspNetCore.Authentication;

namespace OgrenciKulupSistemi.Controllers
{
    public class ClubController : Controller
    {
        // To access user information via Identity
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ClubController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET : Clubs
        public async Task<IActionResult> Index()
        {
            var clubs = await _context.Clubs.ToListAsync();   //We are pulling all clubs from the database.
            return View(clubs);
        }

        /*
        If the user is logged in, ASP.NET Core populates the user's credentials (ClaimsPrincipal) and assigns them to the User object.
        If the user is not logged in, User.Identity.IsAuthenticated returns false.

        If the user is not logged in, ASP.NET Core automatically redirects them to the login page.
        */
        [Authorize]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(ClubCreateViewModel model)
        {
            //Checking if the data received from the form is valid.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Manuel Mapping: ViewModel -> Club Entity
            var newClub = new Club
            {
                Name = model.Name,
                Description = model.Description,
                AdminId = User.FindFirstValue(ClaimTypes.NameIdentifier) // We make the user who is currently logged in a club admin.
            };

            if (model.LogoImage != null)
            {
                newClub.LogoImageUrl = await FileUploadHelper.UploadFile(model.LogoImage, "clubs");
            }

            if (model.CoverPhoto != null)
            {
                newClub.CoverPhotoUrl = await FileUploadHelper.UploadFile(model.CoverPhoto, "clubs");
            }

            _context.Clubs.Add(newClub); // First, let's register the club in the database, because when we retrieve the gallery photo, 
                                         // we need to keep track of which club the photo belongs to.
            await _context.SaveChangesAsync(); // The ID for the club we created has now been created.

            /*
            Above, with the `newClub ...` button, we made the user who clicked the `Create New` button at that moment the admin of the new club.
            Here's what we're doing: "Find the admin of this newly created club, and set that user's role as ClubAdmin."
            */
            var user = await _userManager.FindByIdAsync(newClub.AdminId);
            await _userManager.AddToRoleAsync(user, "ClubAdmin");

            return RedirectToAction("Details", new { id = newClub.Id });
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(int id, [Bind(Prefix = "Event")] Event model, IFormFile EventPhoto)
        {
            /*
            If a View uses more than one model, a prefix is ​​used to distinguish them from each other.
            For the data belonging to the event model to be correctly connected.
            */
            model.Id = 0;
            model.ClubId = id;

            // Removing navigation properties from validation
            ModelState.Remove("Event.Club");
            ModelState.Remove("Event.Attendees");
            ModelState.Remove("EventPhoto");

            if (!ModelState.IsValid)
            {
                // We show errors using TempData.
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Message"] = "Hata oluştu: " + string.Join(", ", errors);

                Console.WriteLine("ModelState HATALI -> " +
                    string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)));

                return RedirectToAction("Details", new { id = model.ClubId });
            }

            // Security check
            var currentUserId = _userManager.GetUserId(User);
            var club = await _context.Clubs.FirstOrDefaultAsync(c => c.Id == model.ClubId);

            if (club == null)
                return NotFound();

            //Only the club admin can create events.
            if (club.AdminId != currentUserId)
                return Unauthorized();

            // Uploading photos
            if (EventPhoto != null)
            {
                model.EventPhotoUrl = await FileUploadHelper.UploadFile(EventPhoto, "events");
            }

            // Save to database
            _context.Events.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = model.ClubId });
        }

        // GET : Club/Details/
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // The reason for using "Include/ThenInclude" is that all the related data needed in the View is retrieved here.
            var club = await _context.Clubs.Include(c => c.Events)
                                           .ThenInclude(c => c.Attendees)
                                           .Include(c => c.Memberships)
                                           .ThenInclude(m => m.ApplicationUser)
                                           .Include(c => c.Photos)
                                           .FirstOrDefaultAsync(m => m.Id == id);

            if (club == null)
            {
                return NotFound();
            }

            var viewModel = new ClubDetailsViewModel
            {
                Club = club, // The club object we obtained from the database in the query above.
                Event = new Event() // An empty event object for the event creation form.
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMember(int membershipId, int clubId)
        {
            // Check the relevant club along with the memberships.
            var club = await _context.Clubs
                .Include(c => c.Memberships)
                .FirstOrDefaultAsync(c => c.Id == clubId);

            if (club == null)
                return NotFound();

            // Only the club admin should be able to delete it.
            var currentUserId = _userManager.GetUserId(User);
            if (club.AdminId != currentUserId)
                return Forbid();  // yetkisiz

            // Find the membership to be deleted.
            var membership = club.Memberships.FirstOrDefault(m => m.Id == membershipId);
            if (membership == null)
                return NotFound();

            // Don't try to delete the user who is the admin.
            if (membership.ApplicationUserId == club.AdminId)
                return BadRequest("Admin kulüpten kaldırılamaz.");

            _context.ClubMemberships.Remove(membership);
            await _context.SaveChangesAsync();

            // Let's go back to the club details.
            return RedirectToAction("Details", new { id = clubId });
        }

        // GET : Club/Edit/5
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var club = await _context.Clubs
                .Include(c => c.Photos)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (club == null)
                return NotFound();

            // Authorization control
            var currentUserId = _userManager.GetUserId(User);
            if (club.AdminId != currentUserId)
                return Unauthorized();

            var vm = new ClubEditViewModel
            {
                Id = club.Id,
                Name = club.Name,
                Description = club.Description,
                ExistingLogoImageUrl = club.LogoImageUrl,
                ExistingCoverPhotoUrl = club.CoverPhotoUrl
            };
            return View(vm);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClubEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // The photos are being re-uploaded so they don't get lost.
                var clubForReload = await _context.Clubs
                    .Include(c => c.Photos)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == model.Id);

                if (clubForReload != null)
                {
                    model.ExistingLogoImageUrl = clubForReload.LogoImageUrl;
                    model.ExistingCoverPhotoUrl = clubForReload.CoverPhotoUrl;
                }
                return View(model);
            }

            var club = await _context.Clubs.Include(c => c.Photos).FirstOrDefaultAsync(c => c.Id == model.Id);
            if (club == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            if (club.AdminId != currentUserId)
            {
                return Unauthorized();
            }

            club.Name = model.Name;
            club.Description = model.Description;

            if (model.LogoImage != null) { club.LogoImageUrl = await FileUploadHelper.UploadFile(model.LogoImage, "clubs"); }
            if (model.CoverPhoto != null) { club.CoverPhotoUrl = await FileUploadHelper.UploadFile(model.CoverPhoto, "clubs"); }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = club.Id });

        }

        public async Task<IActionResult> ClubJoin(int clubId)
        {
            var userId = _userManager.GetUserId(User); // get user id
            if (userId == null)
            {
                return Challenge(new AuthenticationProperties// The user is redirected to the login screen, and then details are sent.
                {
                    RedirectUri = Url.Action("Details", new { id = clubId }) // if user dont log in, return detail page
                });
            }
            // Check if they've participated before.
            bool alreadyJoined = await _context.ClubMemberships.AnyAsync(x => x.ClubId == clubId && x.ApplicationUserId == userId);
            if (alreadyJoined)
            {
                TempData["alreadyJoined"] = "You have already joined this Club";
                return RedirectToAction("Details", new { id = clubId }); // return detail page after appear toast

            }
            var registration = new ClubMembership //if user have not join a club yet , it join.
            {
                ClubId = clubId,
                ApplicationUserId = userId,
                JoinDate = DateTime.UtcNow
            };
            _context.ClubMemberships.Add(registration);
            await _context.SaveChangesAsync();
            TempData["Success"] = "You successfully join this Club.";

            return RedirectToAction("Details", new { id = clubId });
        }


        [HttpPost]
        public async Task<IActionResult> RemoveEvent(int eventId)
        {

            var currentUserId = _userManager.GetUserId(User);
            var removedEvent = await _context.Events.Include(e => e.Club).FirstOrDefaultAsync(k => k.Id == eventId);

            if (removedEvent == null)
            {
                return NotFound();
            }
            // Only the admin can delete it.
            if (removedEvent.Club.AdminId != currentUserId)
            {
                return Forbid();
            }

            int clubId = removedEvent.ClubId;

            _context.Events.Remove(removedEvent);
            await _context.SaveChangesAsync();
            /* When an event is deleted, the event records (eventattendees) of the users who were members of that event are also deleted. */

            return RedirectToAction("Details", new { id = clubId });
        }

        // This method saves the file to the folder named wwwroot/img/ and returns the path to write to the database (/img/...).
        public static class FileUploadHelper
        {
            public static async Task<string> UploadFile(IFormFile file, string folderName)
            {

                var extension = Path.GetExtension(file.FileName); // We got the file extension.

                var uniqueFileName = Guid.NewGuid().ToString() + extension; // We created a unique file name to prevent conflicts between files with the same name.

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", folderName);

                // Create the folder if it doesn't exist.
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, uniqueFileName);

                // Save the file.
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return the path to the database.
                return $"/img/{folderName}/{uniqueFileName}";
            }
        }
    }
}