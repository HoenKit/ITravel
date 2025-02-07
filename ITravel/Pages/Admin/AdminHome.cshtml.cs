using ITravel.Models;
using ITravel.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITravel.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class AdminHomeModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly ITourRepository _tourRepository;
        private readonly IHotelRepository _hotelRepository;

        public AdminHomeModel(IUserRepository userRepository, IEmailSender emailSender
            , ITourRepository tourRepository, IHotelRepository hotelRepository)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
            _tourRepository = tourRepository;
            _hotelRepository = hotelRepository;
        }

        public PageResult<AppUser> PagedUsers { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 3;
        public PageResult<Tour> PageTours { get; set; }
        public PageResult<Hotel> PageHotels { get; set; }
        public async Task OnGetAsync(int pageIndex = 1)
        {
            PagedUsers = await _userRepository.GetUserPageAsync(pageIndex, PageSize);
            PageTours = await _tourRepository.GetTourPageAsync(pageIndex, PageSize);
            PageHotels = await _hotelRepository.GetHotelPageAsync(pageIndex, PageSize);
        }
        public async Task<IActionResult> OnPostBanUserAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return NotFound();

            user.ProfileStatus = 1;
            await _userRepository.UpdateUserAsync(user);

            await _emailSender.SendEmailAsync(user.Email, "Account Banned",
                $"<p>Xin chào {user.UserName}" +
                $",</p><p>Chúng tôi nhận thấy bạn đã vi phạm những điều luật của website nên chúng tôi quyết định Ban tài khoản của bạn" +
                $",</p><p>Nếu có hiểu lầm gì xin liên hệ với chúng tôi thông qua mail này! Xin cảm ơn</p>");

            TempData["Message"] = $"User {user.UserName} has been banned.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUnbanUserAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return NotFound();

            user.ProfileStatus = 0;
            await _userRepository.UpdateUserAsync(user);

            await _emailSender.SendEmailAsync(user.Email, "Account Unbanned",
               $"<p>Xin chào {user.UserName}" +
               $",</p><p>Chào mừng tài khoản của bạn đã được mở trở lại. Hãy quay lại sử dụng website để có thể tìm kiếm những tour du lịch với giá cả phải chăng nào." +
               $",</p><p>Xin lưu ý hãy tuân thủ những điều khoản của website nhé! Xin cảm ơn</p>");

            TempData["Message"] = $"User {user.UserName} has been unbanned.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteTourAsync(Guid tourId, string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return NotFound();

            var tour = await _tourRepository.GetTourByTourIdAsync(tourId);
            if (tour == null) return NotFound();

            tour.IsDeleted = true;

            await _emailSender.SendEmailAsync(user.Email, "Tour Deleted",
               $"<p>Xin chào {tour.Provider.CompanyName}" +
               $",</p><p>Chúng tôi đã vô hiệu hóa Tour {tour.Name} theo những gì bạn yêu cầu" +
               $",</p><p>Nếu bạn muốn tour hoạt động trở lại hãy liên hệ với chúng tôi nhé! Xin cảm ơn</p>");


            TempData["Message"] = $"Tour {tour.Name} has been deleted.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddNewHotelAsync(Hotel hotel)
        {
            await _hotelRepository.CreateHotelAsync(hotel);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateNewHotelAsync(Hotel hotel)
        {
            await _hotelRepository.UpdateHotelAsync(hotel);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteHotel(Guid hotelId)
        {
            if (hotelId == Guid.Empty)
            {
                return BadRequest("Invalid hotel ID.");
            }

            await _hotelRepository.DeleteHotelAsync(hotelId);
            return RedirectToPage();
        }
    }
}
