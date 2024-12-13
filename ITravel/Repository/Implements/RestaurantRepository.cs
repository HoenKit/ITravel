using ITravel.Data;
using ITravel.Models;
using ITravel.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITravel.Repository.Implements
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ITourRepository _tourRepository;
        public RestaurantRepository(ITourRepository tourRepository, ApplicationDbContext context)
        {
            _tourRepository = tourRepository;
            _context = context;
        }
        public ICollection<Restaurant> GetRestaurantByTourDateId(Guid tourDateId)
        {
            var tour = _tourRepository.GetTourByTourDateId(tourDateId);
            return _context.Restaurants
            .Include(ht => ht.RestaurantTours)
            .Where(h => h.RestaurantTours.Any(ht => ht.TourId == tour.Id))
            .ToList();
        }
    }
}
