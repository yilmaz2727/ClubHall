using System.ComponentModel.DataAnnotations;

namespace OgrenciKulupSistemi.ViewModels
{

    public class ClubCreateViewModel
    {

        // !!! ÖNEMLİİİ
        /* Neden bu modeli oluşturdum?
        Çünkü Club/Creaete.cshtml içerisinde kulüp eklerken eklememiz gereken alanlar var. bu alanlardan bazıları Club.cs modelinden bazıları ClubPhoto modelinden geliyor. 
        ancak bir view dosyasında sadece bir kez @model yapmama izin veriliyor, dolayısıyla her iki modelden gelen verileri  ortak olarak kullanabileceğim 
        bir modele ihtiyacım var.
        */


        [Required(ErrorMessage = "Club Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }


        public IFormFile? LogoImage { get; set; }

        public IFormFile? CoverPhoto { get; set; }

        public List<IFormFile>? GalleryPhotos { get; set; } // hakkımızda kısmında gösterilecek olan fotoğraflar




    }


}