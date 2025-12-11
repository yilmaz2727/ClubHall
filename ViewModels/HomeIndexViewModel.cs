using System.Collections.Generic;

namespace OgrenciKulupSistemi.Models   // 🔴 Burası senin projendeki namespace ile aynı olmalı
{
    public class HomeIndexViewModel
    {
        public ApplicationUser? User { get; set; }
        public List<Event>? UpcomingEvents { get; set; }
        public List<Club>? ShowClub { get; set; }
    }
}
