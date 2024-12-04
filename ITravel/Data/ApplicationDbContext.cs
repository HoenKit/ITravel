using ITravel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ITravel.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
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
        }
    }
}
