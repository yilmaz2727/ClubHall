using Microsoft.AspNetCore.Mvc;

namespace OgrenciKulupSistemi.Controllers
{

    public class EventController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

    }

}