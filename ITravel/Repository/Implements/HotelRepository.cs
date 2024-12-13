using ITravel.Data;
using ITravel.Models;
using ITravel.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITravel.Repository.Implements
{
    public class HotelRepository : IHotelRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ITourRepository _tourRepository;
        public HotelRepository(ApplicationDbContext context, ITourRepository tourRepository)
        {
            _context = context;
            _tourRepository = tourRepository;
        }

        public ICollection<Hotel> GetHotelByTourDateId(Guid tourDateId)
        {
            var tour = _tourRepository.GetTourByTourDateId(tourDateId);
            return _context.Hotels
            .Include(ht => ht.HotelTours)
            .Where(h => h.HotelTours.Any(ht => ht.TourId == tour.Id))
            .ToList();
        }
    }
}
