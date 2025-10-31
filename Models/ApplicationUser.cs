using Microsoft.AspNetCore.Identity;

namespace OgrenciKulupSistemi.Models
{

    // !!!!!! ÖNEMLİ
    /* 
    Burada IdentityUser sınıfından miras alıyoruz. IdentityUser sınıfı içerisinde bazı propları halihazırda barındırır. id Email, Username, PhoneNumber gibi.
    dolayısıtyla bunları tekrar burada tanımlamamıza gerek yok. 
    */

    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

}