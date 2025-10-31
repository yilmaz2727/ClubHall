using Microsoft.AspNetCore.Mvc;

namespace OgrenciKulupSistemi.Controllers
{

    public class ProfileController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

    }

}