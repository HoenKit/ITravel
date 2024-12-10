using ITravel.Data;
using ITravel.Models;
using ITravel.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITravel.Repository.Implements
{
    public class TourRepository : ITourRepository
    {
        private readonly ApplicationDbContext _context;
        public TourRepository(ApplicationDbContext context) 
        {
            _context = context;
        }
        public ICollection<TourDate> Get5RecentTours() =>
        _context.ToursDate
        .Include(t => t.Tour) 
        .ThenInclude(tour => tour.Images) 
        .OrderByDescending(t => t.StartDate) 
        .Take(5) 
        .ToList();

        public TourDate GetTourDateById(Guid id) =>
        _context.ToursDate
        .Include(t => t.Tour)
            .ThenInclude(tour => tour.Images) 
        .Include(t => t.Tour)
            .ThenInclude(tour => tour.HotelTours) 
                .ThenInclude(hotelTour => hotelTour.Hotel) 
        .Include(t => t.Tour)
            .ThenInclude(tour => tour.RestaurantTours) 
                .ThenInclude(restaurantTour => restaurantTour.Restaurant) 
        .FirstOrDefault(t => t.Id == id); 

    }
}
