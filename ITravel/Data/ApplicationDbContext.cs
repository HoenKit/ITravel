using ITravel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ITravel.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<ActivitySchedule> ActivitySchedules { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<HotelTour> HotelTours { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<RestaurantTour> RestaurantTours { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourDate> ToursDate { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //var admin = new IdentityRole("Administrator");
            //admin.NormalizedName = "Administrator";

            //var users = new IdentityRole("Users");
            //users.NormalizedName = "Users";

            //var provider = new IdentityRole("Provider");
            //provider.NormalizedName = "Provider";
            //builder.Entity<IdentityRole>().HasData(admin, users, provider);

            //Provider-AppUser Relationship
            builder.Entity<Provider>(entity =>
            {
                // Configuring the relationship between Recruiter and AppUser
                entity.HasOne(r => r.User)
                      .WithOne()
                      .HasForeignKey<Provider>(r => r.UserId);
            });
                //Hotel-Tour Relationship
                builder.Entity<HotelTour>(entity =>
                {
                entity.HasKey(ht => new { ht.HotelId, ht.TourId });

                entity.HasOne(ht => ht.Hotel)
                      .WithMany(h => h.HotelTours) 
                      .HasForeignKey(ht => ht.HotelId)
                      .OnDelete(DeleteBehavior.Cascade); 

                entity.HasOne(ht => ht.Tour)
                      .WithMany(t => t.HotelTours) 
                      .HasForeignKey(ht => ht.TourId)
                      .OnDelete(DeleteBehavior.Cascade); 
                });
            //Restaurant-Tour Relationship
            builder.Entity<RestaurantTour>(entity =>
            {
                entity.HasKey(ht => new { ht.RestaurantId, ht.TourId });

                entity.HasOne(ht => ht.Restaurant)
                    .WithMany(h => h.RestaurantTours)
                    .HasForeignKey(ht => ht.RestaurantId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ht => ht.Tour)
                    .WithMany(t => t.RestaurantTours)
                    .HasForeignKey(ht => ht.TourId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
             builder.Entity<Tour>()
            .HasOne(t => t.Provider) 
            .WithMany(p => p.Tours)   
            .HasForeignKey(t => t.ProviderId) 
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
