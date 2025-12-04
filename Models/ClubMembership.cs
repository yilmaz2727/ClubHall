namespace OgrenciKulupSistemi.Models
{
    public class ClubMembership
    {

        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int ClubId { get; set; }
        public Club Club { get; set; }

        public DateTime JoinDate { get; set; } = DateTime.Now;

    }

}
