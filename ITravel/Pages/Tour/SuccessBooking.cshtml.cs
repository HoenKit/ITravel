using ITravel.Models;
using ITravel.Repository.Interfaces;
using ITravel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Drawing.Imaging;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.Encodings.Web;
using ZXing;
using ZXing.Windows.Compatibility;

namespace ITravel.Pages.Tour
{
    [Authorize(Roles = "Users")]
    public class SuccessBookingModel : PageModel
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITourRepository _tourRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmailService _emailSender;
        public SuccessBookingModel(IBookingRepository bookingRepository, UserManager<AppUser> userManager, ITourRepository tourRepository, ICustomerRepository customerRepository, IEmailService emailSender)
        {
            _bookingRepository = bookingRepository;
            _userManager = userManager;
            _tourRepository = tourRepository;
            _customerRepository = customerRepository;
            _emailSender = emailSender;
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
            tourDate.CurrentCapacity += people.Value;
            _tourRepository.UpdateTourDate(tourDate);

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
            // Tạo mã QR và chuyển thành Base64
            var barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            barcodeWriter.Options.Width = 200;
            barcodeWriter.Options.Height = 200;

            // Tạo QR Code
            using (var bitmap = barcodeWriter.Write($"{booking.Id}"))
            {
                using (var stream = new MemoryStream())
                {
                    // Lưu ảnh vào stream
                    bitmap.Save(stream, ImageFormat.Png);

                    // Đặt lại con trỏ stream về đầu
                    stream.Position = 0;

                    // Nội dung email
                    var emailBody = $@"
        <div style='font-family: Arial, sans-serif;'>
            <h2 style='color: #2c3e50;'>Vé xác nhận đặt tour ITravel!</h2>
            <p style='font-size: 16px; color: #34495e;'>
                Cảm ơn vì đã đặt tour! Dưới đây là mã đặt tour của bạn:
            </p>
            <p style='font-size: 14px; color: #7f8c8d; margin-top: 20px;'>
                Xin hãy xem mã QR được đính kèm trong email.
            </p>
        </div>";

                    // Gửi email với tệp đính kèm
                    await _emailSender.SendEmailWithQRAsync(user.Email, "Vé xác nhận đặt tour", emailBody, stream, "QRCode.png");
                    return RedirectToPage("/Tour/Detail", new { id = tourId });
                }
            }
        }
    }
}
