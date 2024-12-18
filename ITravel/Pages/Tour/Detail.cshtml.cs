using ITravel.Models;
using ITravel.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITravel.Pages.Tour
{
    public class DetailModel : PageModel
    {
        private readonly ITourRepository _tourRepository;
        private readonly IHotelRepository _hotelRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly UserManager<AppUser> _userManager;

        public DetailModel(ITourRepository tourRepository, IHotelRepository hotelRepository, IRestaurantRepository restaurantRepository, IScheduleRepository scheduleRepository, IReviewRepository reviewRepository, UserManager<AppUser> userManager, IBookingRepository bookingRepository)
        {
            _tourRepository = tourRepository;
            _hotelRepository = hotelRepository;
            _restaurantRepository = restaurantRepository;
            _scheduleRepository = scheduleRepository;
            _reviewRepository = reviewRepository;
            _userManager = userManager;
            _bookingRepository = bookingRepository;
        }
        [BindProperty]
        public Guid TourDateId { get; set; }
        [BindProperty]
        public ICollection<Booking> Bookings { get; set; }
        [BindProperty]
        public new string Content { get; set; }

        [BindProperty]
        public int Rating { get; set; }
        public TourDate Tour { get; set; }
        public ICollection<Hotel> Hotels { get; set; }
        public ICollection<Restaurant> Restaurants { get; set; }
        public ICollection<ActivitySchedule> ActivitySchedules { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public int Time { get; set; }
        public int FullPrice { get; set; }
        [BindProperty]
        public Review Review { get; set; }
        public void OnGet(Guid id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                Bookings = _bookingRepository.GetUnusedBookingsByTourDateIdAndUserId(id, Guid.Parse(userId));
            }
            TourDateId = id;
            Tour = _tourRepository.GetTourDateById(id);
            Hotels = _hotelRepository.GetHotelByTourDateId(id);
            Restaurants = _restaurantRepository.GetRestaurantByTourDateId(id);
            ActivitySchedules = _scheduleRepository.GetScheduleByTourDateId(id);
            Reviews = _reviewRepository.GetReviewsByTourDateId(id);
            TimeSpan timeSpan = Tour.EndDate.Date - Tour.StartDate.Date;
            Time = timeSpan.Days;
            FullPrice = (int)Math.Ceiling(Tour.Tour.Price * Tour.MaxCapacity * 0.85);

        }
        public IActionResult OnPostCreateReview(Guid id, string userId)
        {
            if (!Guid.TryParse(userId, out Guid parsedUserId))
            {
                return BadRequest("Invalid User ID.");
            }

            Bookings = _bookingRepository.GetUnusedBookingsByTourDateIdAndUserId(id, parsedUserId);

            if (!Bookings.Any())
            {
                return RedirectToPage("/Error", new { message = "No unused bookings found." });
            }

            var newReview = new Review
            {
                Booking = Bookings.FirstOrDefault(),
                Content = Content,
                Rating = Rating,
            };

            _reviewRepository.CreateReview(newReview);

            return RedirectToPage("/Tour/Detail", new { id });
        }
        public IActionResult OnPost(string action, Guid reviewId, Guid id, string content, int ratingModal)
        {
            if (action == "delete")
            {
                _reviewRepository.DeleteReview(reviewId);

                return RedirectToPage("/Tour/Detail", new { id });
            }
            if (action == "edit")
            {
                var review = _reviewRepository.GetReviewById(reviewId);

                if (review != null)
                {
                    review.Content = content;
                    review.Rating = ratingModal;

                    _reviewRepository.UpdateReview(review);
                }

                return RedirectToPage("/Tour/Detail", new { id });
            }

            return RedirectToPage("/Tour/Detail", new { id });
        }

    }
}

