using System.ComponentModel.DataAnnotations;

namespace OgrenciKulupSistemi.ViewModels
{
    public class ClubEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kulüp adı zorunludur.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        public string Description { get; set; } = string.Empty;
        //Yeni yüklenecek dosyalar
        public IFormFile? LogoImage { get; set; }
        public IFormFile? CoverPhoto { get; set; }
        public List<IFormFile>? GalleryPhotos { get; set; }
        //Hali hazırda bulunan görseller
        public string? ExistingLogoImageUrl { get; set; }
        public string? ExistingCoverPhotoUrl { get; set; }
    }
}
