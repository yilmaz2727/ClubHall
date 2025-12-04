namespace OgrenciKulupSistemi.Models;

public class Club
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? LogoImageUrl { get; set; }
    public string? CoverPhotoUrl { get; set; }


    // İlişkiler

    public string? AdminId { get; set; }
    public ApplicationUser Admin { get; set; } // navigation prop


    public ICollection<Event> Events { get; set; } // --> NAvigation prop. bir kulübün birden çok etkinliği olabilir. events tablosuna bir FK ekleyecek

    public ICollection<ClubPhoto> Photos { get; set; }

    public ICollection<ClubMembership> Memberships { get; set; }

}