using Microsoft.AspNetCore.Identity;

namespace ITravel.Models
{
    public class AppUser : IdentityUser
    {
        public int ProfileStatus { get; set; } = 0;

        public string? Avatar { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}
