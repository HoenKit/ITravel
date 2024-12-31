using ITravel.Models;
using ITravel.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITravel.Pages.User
{
    [Authorize(Roles = "Users")]
    public class BookingListModel : PageModel
    {
        
        private readonly IBookingRepository _bookingRepository;
        private readonly UserManager<AppUser> _userManager;
        public BookingListModel(IBookingRepository bookingRepository, UserManager<AppUser> userManager)
        {
            _bookingRepository = bookingRepository;
            _userManager = userManager;
        }
        public ICollection<Booking> Bookings { get; set; }
        public void OnGet()
        {
            var userId = _userManager.GetUserId(User);
            Bookings = _bookingRepository.GetBookingByUser(Guid.Parse(userId));
        }
    }
}
