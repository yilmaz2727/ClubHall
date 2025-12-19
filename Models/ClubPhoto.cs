
namespace OgrenciKulupSistemi.Models
{

    public class ClubPhoto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }

        public int ClubId { get; set; }
        public Club Club;


    }

}