using ITravel.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace ITravel.Pages.Provider
{
    public class ConfirmBookingModel : PageModel
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ILogger<ConfirmBookingModel> _logger;

        public ConfirmBookingModel(ILogger<ConfirmBookingModel> logger, IBookingRepository bookingRepository)
        {
            _logger = logger;
            _bookingRepository = bookingRepository;
        }
        [BindProperty]
        public IFormFile QrCodeImage { get; set; }

        [BindProperty]
        public string QrCodeImageBase64 { get; set; }
        public string ScanResult { get; set; }
        public string Result { get; set; }

        public IActionResult OnPost()
        {
            if (QrCodeImage == null && string.IsNullOrEmpty(QrCodeImageBase64))
            {
                return BadRequest("Không có dữ liệu ảnh gửi lên.");
            }
            // Kiểm tra nếu có ảnh base64, xử lý ảnh đó
            if (!string.IsNullOrEmpty(QrCodeImageBase64))
            {
                try
                {
                    byte[] imageBytes = Convert.FromBase64String(QrCodeImageBase64.Split(',')[1]);
                    // Xử lý ảnh (Lưu, giải mã QR, v.v.)
                    var result = DecodeQrCodeFromImage(imageBytes);

                    if (result == null)
                    {
                        ModelState.AddModelError(string.Empty, "Không thể đọc mã QR từ ảnh base64.");
                        return Page();
                    }

                    // Gán kết quả quét vào biến ScanResult
                    ScanResult = result.Text;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Lỗi khi xử lý ảnh base64: {ex.Message}");
                    return Page();
                }
            }
            // Nếu không có ảnh base64, kiểm tra ảnh tải lên
            else if (QrCodeImage != null && QrCodeImage.Length > 0)
            {
                try
                {
                    using (var stream = QrCodeImage.OpenReadStream())
                    using (var bitmap = new Bitmap(stream))
                    {
                        var result = DecodeQrCodeFromImage(bitmap);

                        if (result == null)
                        {
                            ModelState.AddModelError(string.Empty, "Không thể đọc mã QR từ ảnh tải lên.");
                            return Page();
                        }

                        // Gán kết quả quét vào biến ScanResult
                        ScanResult = result.Text;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Lỗi khi quét mã QR: {ex.Message}");
                    return Page();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Vui lòng tải lên một ảnh hợp lệ.");
                return Page();
            }

            // Kiểm tra mã QR hợp lệ (Booking ID)
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

            // Kiểm tra nếu mã đã được sử dụng
            if (booking.IsUsed)
            {
                ModelState.AddModelError(string.Empty, "Mã này đã được sử dụng.");
                return Page();
            }

            // Đánh dấu mã đã được sử dụng và lưu lại
            booking.IsUsed = true;
            _bookingRepository.UpdateBooking(booking);

            // Gửi kết quả thành công
            Result = "Mã hợp lệ.";
            return Page();
        }

        // Phương thức giải mã mã QR từ ảnh
        private Result DecodeQrCodeFromImage(byte[] imageBytes)
        {
            using (var ms = new MemoryStream(imageBytes))
            using (var bitmap = new Bitmap(ms))
            {
                var reader = new BarcodeReader();
                return reader.Decode(bitmap);
            }
        }

        // Phương thức giải mã mã QR từ đối tượng Bitmap
        private Result DecodeQrCodeFromImage(Bitmap bitmap)
        {
            var reader = new BarcodeReader();
            return reader.Decode(bitmap);
        }

    }
}
