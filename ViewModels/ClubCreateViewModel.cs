using System.ComponentModel.DataAnnotations;

namespace OgrenciKulupSistemi.ViewModels
{

    public class ClubCreateViewModel
    {

        // !!! ÖNEMLİİİ
        /* Why did I create this model?
        Because when adding a club in Club/Create.cshtml, we need to include certain fields. Some of these fields come from the Club.cs model, and some come from the ClubPhoto model. 
        However, I'm only allowed to use `@model` once in a view file, so I need a model that can use data from both models in common.
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