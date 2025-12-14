using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OgrenciKulupSistemi.Data;
using OgrenciKulupSistemi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;

namespace OgrenciKulupSistemi.Controllers
{

    public class EventController : Controller
    {
        //Define database
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? clubId, string eventSearchString, string? eventType, string? clubName)
        {
            //Get events from the db
            IEnumerable<Event> events;
            if (clubId.HasValue)
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
            if (id == null)
            {
                return NotFound();
            }

            //Get the event from the db by Id
            var _event = await _context.Events
                .Include(e => e.Club)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (_event == null)
            {
                return NotFound();
            }

            return View(_event);
        }

        [HttpPost]
        public async Task<IActionResult> Join(int eventId)
        {
            var userId = _userManager.GetUserId(User);// get user id 
            if (userId == null)
            {
                return Challenge(new AuthenticationProperties// The user is redirected to the login screen, then sent to the details section
                {
                    RedirectUri = Url.Action("Details", new { id = eventId })
                });
            }

            bool alreadyJoined = await _context.EventAttendees.AnyAsync(x => x.EventId == eventId && x.ApplicationUserId == userId);

            if (alreadyJoined)
            {
                TempData["alreadyJoined"] = "You have already joined this event";
               return RedirectToAction("Details", new { id = eventId });// retun detail page after appear toast 
             
            }
            var registration = new EventAttendee //if user have not join a club yet , it join
            {
                EventId = eventId,
                ApplicationUserId = userId,
                RegisterDate = DateTime.UtcNow
            };
            _context.EventAttendees.Add(registration);

            var ev = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            if (ev != null)
            {
                ev.NumberOfAttendance += 1; // her join işleminden sonra katılımcı sayısı 1 artırılıyor.
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "You successfully join this event.";

            return RedirectToAction("Details", new { id = eventId });
        }


    }

}