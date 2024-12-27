using ITravel.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace ITravel.Pages.Provider
{
    [Authorize(Roles = "Provider")]
    public class ConfirmBookingModel : PageModel
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ILogger<ConfirmBookingModel> _logger;

        public ConfirmBookingModel(ILogger<ConfirmBookingModel> logger, IBookingRepository bookingRepository)
        {
            _logger = logger;
            _bookingRepository = bookingRepository;
        }
        public string ScanResult { get; set; }
        public string Result { get; set; }

        public void OnGet()
        {

        }
        public IActionResult OnPost(string qrCodeImage)
        {  
            if (qrCodeImage == null || qrCodeImage.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng tải lên một ảnh hợp lệ.");
                return Page();
            }

            try
            {
                byte[] imageBytes = Convert.FromBase64String(qrCodeImage.Split(',')[1]);
                using (var ms = new MemoryStream(imageBytes))
                using (var bitmap = new Bitmap(ms))
                {
                    var reader = new BarcodeReader();
                    var result = reader.Decode(bitmap);
                    if (result == null)
                    {
                        ModelState.AddModelError(string.Empty, "Không thể đọc mã QR từ ảnh.");
                        return Page();
                    }

                    // Save Qr to ScanResult
                    ScanResult = result.Text;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi trong quá trình quét: {ex.Message}");
                return Page();
            }
            // Check valid scan
            if (!Guid.TryParse(ScanResult, out var bookingId))
            {
                ModelState.AddModelError(string.Empty, "Mã QR không hợp lệ.");
                return Page();
            }
            var booking = _bookingRepository.GetBookingById(bookingId);
            if (booking == null)
            {
                ModelState.AddModelError(string.Empty, "Mã không tồn tại.");
                return Page();
            }
            if (booking.IsUsed == true)
            {
                ModelState.AddModelError(string.Empty, "Mã này đã được sử dụng.");
                return Page();
            }
            booking.IsUsed = true;
            _bookingRepository.UpdateBooking(booking);
            Result = $"Mã hợp lệ. Khách hàng: {booking.User.FullName}. Tour: {booking.TourDate.Tour.Name} {(booking.IsFullTour ? "Đặt trọn gói" : "")}";
            return Page();
        }

    }
}
