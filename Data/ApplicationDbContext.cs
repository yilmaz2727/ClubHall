using ClubHall.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OgrenciKulupSistemi.Models;

namespace OgrenciKulupSistemi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Club>().HasData(
                new Club
                {
                    Id = 1,
                    Name = "SAÜ Rock Topluluğu",
                    Description = "Kampüsün ritmini biz belirleriz. Müzik ve eğlence burada.",
                    LogoImageUrl = "",
                    CoverPhotoUrl = ""
                },
                new Club
                {
                    Id = 2,
                    Name = "Saü Bilgisayar Topluluğu",
                    Description = "Yazılım ve teknoloji meraklılarının buluşma noktası.",
                    LogoImageUrl = "",
                    CoverPhotoUrl = ""
                },
                new Club
                {
                    Id = 3,
                    Name = "SAÜ ESN",
                    Description = "Erasmus Student Network of ESN Sakarya University The Official Page of ESN SAKARYA",
                    LogoImageUrl = "",
                    CoverPhotoUrl = ""
                }
            );


            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    Title = "ASP.NET Core Hackathon",
                    Description = "Takımını oluştur, becerilerini göster!",
                    EventDate = new DateTime(2025, 11, 15),
                    Location = "Bilgisayar ve Bilişim Bilimleri Fakültesi 1109",
                    EventPhotoUrl = "",
                    ClubId = 2
                },
                new Event
                {
                    Id = 2,
                    Title = "SAÜ Rock The Band Sahnede",
                    Description = "Tiyatro Topluluğu’nun düzenlemiş olduğu 1. Tiyatro Günleri’nde biz de SaüRock olarak sahnedeyiz! ",
                    EventDate = new DateTime(2025, 11, 20),
                    Location = "Turgut Özal Kültür ve Kongre Merkezi",
                    EventPhotoUrl = "",
                    ClubId = 1
                },
                new Event
                {
                    Id = 3,
                    Title = "Spekaing CLub First Meeting",
                    Description = "Practice Engilsh and Meet with new people!",
                    EventDate = new DateTime(2025, 11, 20),
                    Location = "Saü Taş Kafe",
                    EventPhotoUrl = "",
                    ClubId = 3
                }
            );


            modelBuilder.Entity<ClubPhoto>().HasData(
                new ClubPhoto { Id = 1, ClubId = 1, ImageUrl = "https://placehold.co/600x400" },
                new ClubPhoto { Id = 2, ClubId = 1, ImageUrl = "https://placehold.co/600x400" }
            );


        }



        public DbSet<Club> Clubs => Set<Club>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<ClubPhoto> Photos => Set<ClubPhoto>();

    }
}