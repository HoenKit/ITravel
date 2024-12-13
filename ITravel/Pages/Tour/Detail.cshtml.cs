using ITravel.Models;
using ITravel.Repository.Interfaces;
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

        public DetailModel(ITourRepository tourRepository, IHotelRepository hotelRepository, IRestaurantRepository restaurantRepository, IScheduleRepository scheduleRepository)
        {
            _tourRepository = tourRepository;
            _hotelRepository = hotelRepository;
            _restaurantRepository = restaurantRepository;
            _scheduleRepository = scheduleRepository;
        }
        public TourDate Tour {  get; set; }
        public ICollection<Hotel> Hotels { get; set; }
        public ICollection<Restaurant> Restaurants { get; set; }
        public ICollection<ActivitySchedule> ActivitySchedules { get; set; }
        public int Time {  get; set; }
        public void OnGet(Guid id)
        {
            Tour = _tourRepository.GetTourDateById(id);
            Hotels = _hotelRepository.GetHotelByTourDateId(id);
            Restaurants = _restaurantRepository.GetRestaurantByTourDateId(id);
            ActivitySchedules = _scheduleRepository.GetScheduleByTourDateId(id);
            TimeSpan timeSpan = Tour.EndDate.Date - Tour.StartDate.Date;
            Time = timeSpan.Days; 

        }
    }
}
