using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
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
        private readonly IWebHostEnvironment _env;

        public ClubController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _context = context;
            _env = env;
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
        public async Task<IActionResult> CreateEvent(int id, [Bind(Prefix = "Event")] Event model, IFormFile EventPhoto)
        {

            /* 
            Eğer bir View'de birden fazla model kullanılıyorsa, bu modellerin birbirinden ayrılması için prefix kullanılır.
            Event modeline ait verilerin doğru şekilde bağlanması için
            */
            model.Id = 0;
            model.ClubId = id;

            // Navigation property'lerin validasyondan çıkarılması
            ModelState.Remove("Event.Club");
            ModelState.Remove("Event.Attendees");
            ModelState.Remove("EventPhoto");

            if (!ModelState.IsValid)
            {


                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Message"] = "Hata oluştu: " + string.Join(", ", errors);

                Console.WriteLine("ModelState HATALI -> " +
                    string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)));

                return RedirectToAction("Details", new { id = model.ClubId });
            }

            // Güvenlik kontrolü
            var currentUserId = _userManager.GetUserId(User);
            var club = await _context.Clubs.FirstOrDefaultAsync(c => c.Id == model.ClubId);

            if (club == null)
                return NotFound();

            if (club.AdminId != currentUserId)
                return Unauthorized();

            // Fotoğraf yükleme
            if (EventPhoto != null)
            {
                model.EventPhotoUrl = await FileUploadHelper.UploadFile(EventPhoto, "events");
            }

            // Veritabanına kaydet
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
                Club = club, // yukarıda sorguda elde ettiğimiz db'den gelen kulüp nesnesi
                Event = new Event() // etkinlik oluştur formu için boş bir event nesnesi
            };

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


        public async Task<IActionResult> ClubJoin(int clubId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Challenge(new AuthenticationProperties// kullanıcı → login ekranına yönlendir daha sornasında detaile gönderir
                {
                    RedirectUri = Url.Action("Details", new { id = clubId })
                });
            }
            bool alreadyJoined = await _context.ClubMemberships.AnyAsync(x => x.ClubId == clubId && x.ApplicationUserId == userId);
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