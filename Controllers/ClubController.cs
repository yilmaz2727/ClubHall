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
            var clubs = await _context.Clubs.ToListAsync();
            return View(clubs);
        }


        /*
        Eğer kullanıcı giriş yapmışsa, ASP.NET Core, kullanıcının kimlik bilgilerini (ClaimsPrincipal) doldurur ve User nesnesine atar.
        Eğer giriş yapılmamışsa, User.Identity.IsAuthenticated false olur.

        Eğer kullanıcı giriş yapmamışsa, ASP.NET Core otomatik olarak giriş sayfasına yönlendirir
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            // // 1. Manuel Mapping: ViewModel -> Club Entity
            var newClub = new Club
            {
                Name = model.Name,
                Description = model.Description,
                AdminId = User.FindFirstValue(ClaimTypes.NameIdentifier) // o anda giriş yapmış olan kullanıcıyı kulüp admini yapıyoruz.
            };


            if (model.LogoImage != null)
            {
                newClub.LogoImageUrl = await FileUploadHelper.UploadFile(model.LogoImage, "clubs");
            }

            if (model.CoverPhoto != null)
            {
                newClub.CoverPhotoUrl = await FileUploadHelper.UploadFile(model.CoverPhoto, "clubs");
            }

            _context.Clubs.Add(newClub); // ilk önce veritabanına  kulübü kaydedelim, çünkü galleryphotoyu alırken fotoğrafın hangi kulübe ait olduğunu tutmalıyız.
            await _context.SaveChangesAsync(); // artık oluşturduğmuz kulübün id'si oluştu


            // hakkmızda kısmına ait olan fotoğraflar'ı ClubPhoto da tuttuğumuz için ayrıca işleme aldık.
            if (model.GalleryPhotos != null && model.GalleryPhotos.Count > 0)
            {
                foreach (var file in model.GalleryPhotos)
                {
                    var path = await FileUploadHelper.UploadFile(file, "club-gallery");

                    var photo = new ClubPhoto
                    {
                        ImageUrl = path,
                        ClubId = newClub.Id
                    };

                    _context.Photos.Add(photo);
                }

                await _context.SaveChangesAsync();
            }

            /*
            yukarıda var newClub ... ile o anda Create New butonuna basan kullanıcıyı yeni kulbün admini yapmıştık.
            burada şunu yapıyoruz: "bu yeni oluşturulan kulübün adminini bul, o kullanıcının rolünü ClubAdmin yap."
            */
            var user = await _userManager.FindByIdAsync(newClub.AdminId);
            await _userManager.AddToRoleAsync(user, "ClubAdmin");

            return RedirectToAction("Details", new { id = newClub.Id });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(
            int id,
            [Bind(Prefix = "Event")] Event model,   
            IFormFile EventPhoto)
        {
            model.Id = 0;  
            model.ClubId = id;

            if (EventPhoto != null)
                model.EventPhotoUrl = await FileUploadHelper.UploadFile(EventPhoto, "events");

            _context.Events.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id, tab = "events" });
        }

         // GET : Club/Details/
        public async Task<IActionResult> Details(int id, string? tab, int? editEventId)
        {
            var club = await _context.Clubs
                .Include(c => c.Events)
                    .ThenInclude(e => e.Attendees)
                .Include(c => c.Memberships)
                    .ThenInclude(m => m.ApplicationUser)
                .Include(c => c.Photos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (club == null)
                return NotFound();

            var viewModel = new ClubDetailsViewModel
            {
                Club = club,
                Event = new Event() // Varsayılan boş event
            };

            if (editEventId.HasValue)
            {
                var ev = club.Events.FirstOrDefault(e => e.Id == editEventId.Value);
                if (ev == null)
                    return NotFound();

                viewModel.Event = ev; // Dolu veriyi ViewModel'e atadık

                // Eğer formda veriler görünmüyorsa ModelState'i temizlemek işe yarar
                ModelState.Clear(); 

                ViewBag.ActiveTab = "createEvent";
                ViewBag.EditMode = true; 
            }
            else
            {
                ViewBag.ActiveTab = tab ?? "about";
                ViewBag.EditMode = false;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMember(int membershipId, int clubId)
        {
            // İlgili kulübü üyeliklerle birlikte çek
            var club = await _context.Clubs
                .Include(c => c.Memberships)
                .FirstOrDefaultAsync(c => c.Id == clubId);

            if (club == null)
                return NotFound();

            // Sadece kulüp admini silebilsin
            var currentUserId = _userManager.GetUserId(User);
            if (club.AdminId != currentUserId)
                return Forbid();  // yetkisiz

            // Silinecek membership'i bul
            var membership = club.Memberships.FirstOrDefault(m => m.Id == membershipId);
            if (membership == null)
                return NotFound();

            // Admin olan kullanıcıyı silmeye çalışma (opsiyonel ama mantıklı)
            if (membership.ApplicationUserId == club.AdminId)
                return BadRequest("Admin kulüpten kaldırılamaz.");

            _context.ClubMemberships.Remove(membership);
            await _context.SaveChangesAsync();

            // Tekrar kulüp detayına dön
            return RedirectToAction("Details", new { id = clubId });
        }

        [HttpPost]
        [Authorize] // Sadece giriş yapmış kullanıcılar kullanabilsin
        public async Task<IActionResult> LeaveClub(int clubId)
        {
            var userId = _userManager.GetUserId(User);

            // Kullanıcının o kulüpteki üyeliğini buluyoruz
            var membership = await _context.ClubMemberships
                .FirstOrDefaultAsync(m => m.ClubId == clubId && m.ApplicationUserId == userId);

            if (membership != null)
            {
                _context.ClubMemberships.Remove(membership);
                await _context.SaveChangesAsync();
                TempData["Success"] = "You have successfully left the club.";
            }

            return RedirectToAction("Details", new { id = clubId });
        }

        //EDIT EVENT
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(ClubDetailsViewModel model, IFormFile EventPhoto)
        {
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.Id == model.Event.Id);

            if (ev == null)
                return NotFound();

            ev.Title = model.Event.Title;
            ev.Description = model.Event.Description;
            ev.StartDate = model.Event.StartDate;
            ev.EndDate = model.Event.EndDate;
            ev.RegistrationDeadline = model.Event.RegistrationDeadline;
            ev.EventType = model.Event.EventType;
            ev.Location = model.Event.Location;

            if (EventPhoto != null)
                ev.EventPhotoUrl = await FileUploadHelper.UploadFile(EventPhoto, "events");

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new
            {
                id = ev.ClubId,
                tab = "events"
            });
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
                return Challenge(new AuthenticationProperties// The user is redirected to the login screen, then sent to the details section
                {
                    RedirectUri = Url.Action("Details", new { id = clubId }) // if user dont log in, return detail page
                });
            }
            bool alreadyJoined = await _context.ClubMemberships.AnyAsync(x => x.ClubId == clubId && x.ApplicationUserId == userId); //check whether user member club
            if (alreadyJoined)
            {
                TempData["alreadyJoined"] = "You have already joined this Club";
               return RedirectToAction("Details", new { id = clubId }); // retun detail page after appear toast 
             
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

            if (removedEvent.Club.AdminId != currentUserId)
            {
                return Forbid();
            }

            int clubId = removedEvent.ClubId;

            _context.Events.Remove(removedEvent);
            await _context.SaveChangesAsync();
            /* bir etkinlik silindiğinde o etkinliğe üye olan kullanıcıların etkinlik kayıtları (eventattendee) da siliniyor */


            return RedirectToAction("Details", new { id = clubId });
        }






        // Bu metot, dosyayı wwwroot/img/klasoradi içine kaydeder ve veritabanına yazılacak yolu (/img/...) geri döndürür.
        public static class FileUploadHelper
        {
            public static async Task<string> UploadFile(IFormFile file, string folderName)
            {

                var extension = Path.GetExtension(file.FileName); // dosya uzantısını aldık

                var uniqueFileName = Guid.NewGuid().ToString() + extension; // aynı isimlidosyaların çakışmaması için benzersiz bir dosya ismi oluşturduk.

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", folderName);

                // Klasör yoksa oluştur
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, uniqueFileName);

                // 4. Dosyayı kaydet
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 5. Veritabanı için yolu döndür
                return $"/img/{folderName}/{uniqueFileName}";


            }
        }


    }

}