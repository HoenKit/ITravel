using ITravel.Models;
using ITravel.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace ITravel.Pages.Tour
{
    public class SuccessBookingModel : PageModel
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITourRepository _tourRepository;
        private readonly ICustomerRepository _customerRepository;
        public SuccessBookingModel(IBookingRepository bookingRepository, UserManager<AppUser> userManager, ITourRepository tourRepository, ICustomerRepository customerRepository)
        {
            _bookingRepository = bookingRepository;
            _userManager = userManager;
            _tourRepository = tourRepository;
            _customerRepository = customerRepository;
        }
        public async Task<IActionResult> OnGetAsync(Guid tourId)
        {
            // Retrieve the customer list and people count from TempData
            var customersJson = TempData["Customers"] as string;
            var people = TempData["People"] as int?;

            if (string.IsNullOrEmpty(customersJson) || people == null)
            {
                ModelState.AddModelError(string.Empty, "Không có dữ liệu khách hàng hoặc số lượng người.");
                return RedirectToPage("/Error"); // Or any error handling page
            }

            // Deserialize the customer list
            var customers = JsonConvert.DeserializeObject<List<Customer>>(customersJson);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var tourDate = _tourRepository.GetTourDateById(tourId);
            if (tourDate == null)
            {
                ModelState.AddModelError("", "Không tìm thấy thông tin chuyến tham quan.");
                return Page();
            }

            var booking = new Booking
            {
                TourDate = tourDate,
                User = user,
                BookingDate = DateTime.Now,
                NumberOfPeople = people.Value,
                TotalPrice = tourDate.Tour.Price * people.Value
            };

            _bookingRepository.CreateBooking(booking);

            foreach (var customer in customers)
            {
                customer.Booking = booking;
                _customerRepository.CreateCustomer(customer);
            }

            return RedirectToPage("/Index");
        }
    }
}
