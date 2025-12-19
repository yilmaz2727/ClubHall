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
        }



        public DbSet<Club> Clubs => Set<Club>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<ClubPhoto> Photos => Set<ClubPhoto>();

        public DbSet<ClubMembership> ClubMemberships => Set<ClubMembership>();

        public DbSet<EventAttendee> EventAttendees => Set<EventAttendee>();

    }
}