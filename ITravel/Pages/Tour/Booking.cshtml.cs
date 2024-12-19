using ITravel.Models;
using ITravel.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITravel.Pages.Tour
{
    [Authorize(Roles = "Users")]
    public class BookingModel : PageModel
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITourRepository _tourRepository;
        private readonly ICustomerRepository _customerRepository;

        public BookingModel(IBookingRepository bookingRepository, UserManager<AppUser> userManager, ITourRepository tourRepository, ICustomerRepository customerRepository)
        {
            _bookingRepository = bookingRepository;
            _userManager = userManager;
            _tourRepository = tourRepository;
            _customerRepository = customerRepository;
        }
        public Guid TourDateId { get; set; }
        public AppUser Auth { get; private set; }
        public TourDate Tour { get; set; }
        [BindProperty]
        public List<Customer> Customers { get; set; }
        [BindProperty]
        public int People { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Auth = await _userManager.GetUserAsync(User);
            TourDateId = id;
            Tour = _tourRepository.GetTourDateById(id);
            return Page();
        }

        public async Task<IActionResult> OnPost(Guid id, int people)
        {
            var tourDate = _tourRepository.GetTourDateById(id);
            var booking = new Booking
            {
                TourDate = tourDate,
                User = await _userManager.GetUserAsync(User),
                BookingDate = DateTime.Now,
                NumberOfPeople = people,
                TotalPrice = tourDate.Tour.Price * people
            };
            
            if (Customers == null || Customers.Count != People)
            {
                ModelState.AddModelError("", "Số lượng khách không khớp hoặc dữ liệu không hợp lệ.");
                return Page();
            }
            _bookingRepository.CreateBooking(booking);

            foreach (var customer in Customers)
            {
                customer.Booking = booking;
                _customerRepository.CreateCustomer(customer);
            }
            return RedirectToPage("/Index");
        }
    }

}
