namespace OgrenciKulupSistemi.Models
{
    public class EventAttendee
    {

        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public DateTime RegisterDate { get; set; } = DateTime.Now;

    }

}