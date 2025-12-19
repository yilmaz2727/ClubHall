using System.Collections.Generic;

namespace OgrenciKulupSistemi.Models  
{
    public class HomeIndexViewModel
    {
        public ApplicationUser? User { get; set; }
        public List<Event>? UpcomingEvents { get; set; }
        public List<Club>? ShowClub { get; set; }
    }
}
