using ClubHall.Models;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime EventDate { get; set; }
    public string Location { get; set; }
    public string? EventPhotoUrl { get; set; }


    public int ClubId { get; set; } // her bir etkinlik kaydının, Clubs tablosundaki hangi kulübe ait olduğunun Id değerini tutar
    public Club Club { get; set; } // Navigation Prop
    /* 
    ClubId sütunundaki değeri (örneğin 5) kullanarak Clubs tablosuna gider, 
    Id'si 5 olan kulübü bulur ve bu Club nesnesinin tamamını bu özelliğin içine doldurur.
        Bir etkinliği (Event) veritabanından çektik
        event.ClubId  sadece 5 sayısını verir
        event.Club ise  Id=5, Name="Satranç Kulübü" vb. tüm bilgileri içeren ilgili Club nesnesinin tamamını verir

    */


}