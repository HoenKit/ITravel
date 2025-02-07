using ITravel.Models;
using ITravel.Models.PayOs;
using ITravel.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ITravel.Pages.Tour
{
    [Authorize(Roles = "Users")]
    public class BookingModel : PageModel
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITourRepository _tourRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly PayOS _payOS;
        private readonly ILogger<BookingModel> _logger;

        public BookingModel(IBookingRepository bookingRepository, UserManager<AppUser> userManager, ITourRepository tourRepository, ICustomerRepository customerRepository, IUserRepository userRepository, ILogger<BookingModel> logger, PayOS payOS)
        {
            _bookingRepository = bookingRepository;
            _userManager = userManager;
            _tourRepository = tourRepository;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _logger = logger;
            _payOS = payOS;
        }
        public Guid TourDateId { get; set; }
        public AppUser Auth { get; set; }
        public TourDate Tour { get; set; }
        [BindProperty]
        public List<Customer> Customers { get; set; }
        [BindProperty]
        public int People { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Xin hãy nhập số điện thoại hợp lệ")]
        [Phone(ErrorMessage = "Xin hãy nhập số điện thoại hợp lệ")]
        [Display(Name = "Phone number")]
        [RegularExpression(@"^(\+84\d{9,10}|0\d{9,10})$", ErrorMessage = "Xin hãy nhập số điện thoại hợp lệ")]
        public string PhoneNumber { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Xin hãy nhập đầy đủ họ tên")]
        [Display(Name = "Full name")]
        public string FullName { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Xin hãy nhập địa chỉ")]
        [Display(Name = "Full Address")]
        public string Address { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid tourId)
        {
            Auth = await _userManager.GetUserAsync(User);
            TourDateId = tourId;
            Tour = _tourRepository.GetTourDateById(tourId);
            return Page();
        }

        public async Task<IActionResult> OnPost(Guid tourId, int people)
        {
            Auth = await _userManager.GetUserAsync(User);
            TourDateId = tourId;
            Tour = _tourRepository.GetTourDateById(tourId);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError($"Model error: {modelError.ErrorMessage}");  // Log the error
                }
                return Page();
            }

            // Handle FullName and Address update
            bool isUpdated = false;
            if (!string.IsNullOrEmpty(FullName) && FullName != user.FullName)
            {
                user.FullName = FullName;
                isUpdated = true;
            }
            if (!string.IsNullOrEmpty(Address) && Address != user.Address)
            {
                user.Address = Address;
                isUpdated = true;
            }

            if (isUpdated)
            {
                _userRepository.UpdateUser(user);
            }

            // Handle phone number update
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    _logger.LogError("Xảy ra lỗi khi cập nhật số điện thoại.");
                    return Page();
                }
            }

            // Use the TourDateId property to fetch the tour date
            var tourDate = _tourRepository.GetTourDateById(TourDateId);
            if (tourDate == null)
            {
                ModelState.AddModelError("", "Không tìm thấy thông tin chuyến tham quan.");
                return Page();
            }

            // Validate People and Customers
            if (People <= 0)
            {
                ModelState.AddModelError("", "Số lượng người phải lớn hơn 0.");
                return Page();
            }

            if (Customers == null || Customers.Count != People || Customers.Any(c => c == null))
            {
                ModelState.AddModelError("", "Số lượng khách không khớp hoặc dữ liệu không hợp lệ.");
                return Page();
            }
            if(tourDate.CurrentCapacity + People > tourDate.MaxCapacity) 
            {
                ModelState.AddModelError("", "Số lượng khách không được vượt số lượng tối đa.");
                return Page();
            }


            //// Create booking
            //var booking = new Booking
            //{
            //    TourDate = tourDate,
            //    User = user,
            //    BookingDate = DateTime.Now,
            //    NumberOfPeople = people,
            //    TotalPrice = tourDate.Tour.Price * people
            //};

            //_bookingRepository.CreateBooking(booking);

            //// Create customers
            //foreach (var customer in Customers)
            //{
            //    customer.Booking = booking;
            //    _customerRepository.CreateCustomer(customer);
            //}
            try
            {
                CreatePaymentLinkRequest PaymentRequest = new CreatePaymentLinkRequest();
                int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
                PaymentRequest.ProductName = "Thanh toán - ITravel";
                PaymentRequest.Description = "Thanh toán cho chuyến đi";
                PaymentRequest.CancelUrl = $"http://localhost:5121/Tour/Booking?tourId={tourId}";
                PaymentRequest.ReturnUrl = $"http://localhost:5121/Tour/SuccessBooking?tourId={tourId}";

                ItemData item = new ItemData(PaymentRequest.ProductName, People, tourDate.Tour.Price * people);
                List<ItemData> items = new List<ItemData> { item };
                PaymentData paymentData = new PaymentData(orderCode, item.price, PaymentRequest.Description, items, PaymentRequest.CancelUrl, PaymentRequest.ReturnUrl);

                // Store customer data and people count in TempData
                TempData["Customers"] = JsonConvert.SerializeObject(Customers);
                TempData["People"] = People;

                CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);
                return Redirect(createPayment.checkoutUrl);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error creating payment link");
                ModelState.AddModelError(string.Empty, "Lỗi khi tạo yêu cầu thanh toán.");
                return Page();
            }

        }
    }
}
