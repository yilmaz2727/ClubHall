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

        /* 
        Önemli Not: ApplciationUser.cs sınıfı veritabanındaki tabloyu temsil eder. UserEditViewModel ise geçici bir taşıyıcı.
        Bu sınıf sadece kullanıcıya ekranda form gösterirken ve kullanıcıdan veriyi alırken kullanılır.
        
        Kullanıcı "Düzenle"ye Tıklar (GET):
            DB'den ApplicationUser'ı çekersin.
            İçindeki verileri UserEditViewModel'e kopyalarsın.
            Kullanıcıya UserEditViewModel ile dolu HTML sayfasını gösterirsin (Kullanıcı DB nesnesini hiç görmez).

        Kullanıcı Bilgileri Değiştirip "Kaydet" Der (POST):
            Veriler UserEditViewModel olarak Controller'a geri gelir.
            Sen tekrar DB'den ApplicationUser'ı (orijinal kaydı) çağırırsın.
            Bu sefer tersine kopyalama yaparsın: ViewModel'deki yeni verileri -> ApplicationUser üzerine yazarsın.
        */


        [HttpGet]
        public async Task<IActionResult> EditPersonalInfo()
        {
            var user = await _userManager.GetUserAsync(User); // o an siteye giriş yapmış olan kullanıcını tüm bilgilerini alır (ApplicationUser nesnesi)

            // user değişkenni PAssword gibi gizli bilgiler dahil birçok alanı içerir. bunu doğrudan kulanamayız. sadce içerisinde bulunan City,, BirthPlace gibi bilgileri kullanmalıyız burada, gizlilik açısından
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