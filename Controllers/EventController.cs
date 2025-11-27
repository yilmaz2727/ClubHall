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

        public async Task<IActionResult> Index(int? clubId, string eventSearchString, string? eventType, string? clubName)
        {
            //Get events from the db
            IEnumerable<Event> events;
            if(clubId.HasValue)
                events = _context.Events.Include(e => e.Club).Where(e => e.ClubId == clubId).ToList();
            else
                events = _context.Events.Include(e => e.Club);

            if (!string.IsNullOrEmpty(eventSearchString))
            {
                string searchStringLower = eventSearchString.ToLower();

                events = events.Where(e => e.Title.ToLower().Contains(searchStringLower) ||
                                           e.Description.ToLower().Contains(searchStringLower));
            }

            ViewData["EventSearchString"] = eventSearchString;

            //Gets all event types
            ViewData["EventTypes"] = _context.Events
                                            .Select(e => e.EventType)
                                            .Distinct()
                                            .OrderBy(type => type)
                                            .ToList();

            //Gets the selected event type by User 
            ViewData["SelectedEventType"] = eventType;

            //Gets all clubs
            ViewData["Clubs"] = _context.Clubs
                                        .Select(c => c.Name)
                                        .Distinct()
                                        .OrderBy(club => club)
                                        .ToList();

            //Gets the selected event type by User 
            ViewData["SelectedClub"] = clubName;
            
            if (!string.IsNullOrEmpty(eventType))
                events = events.Where(e => e.EventType == eventType);

            if (!string.IsNullOrEmpty(clubName))
                events = events.Where(c => c.Club.Name == clubName);
            
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