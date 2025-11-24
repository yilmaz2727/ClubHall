using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OgrenciKulupSistemi.Data;

namespace OgrenciKulupSistemi.Controllers
{

    public class EventController : Controller
    {
        //Define database
        private readonly ApplicationDbContext _context;
    
        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            //Get events from the db
            var events = await _context.Events.ToListAsync();
            return View(events);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if(id == null) {
                return NotFound();
            }
            
            //Get the event from the db by Id
            var _event = await _context.Events
                .FirstOrDefaultAsync(i => i.Id == id);
            if (_event == null ) {
                return NotFound();
            }

            return View(_event);
        }

    }

}