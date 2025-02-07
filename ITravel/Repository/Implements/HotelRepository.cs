using ITravel.Data;
using ITravel.Models;
using ITravel.Repository.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

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

        public async Task CreateHotelAsync(Hotel hotel)
        {
            _context.Hotels.Add(hotel); 
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHotelAsync(Guid id)
        {
            var hotel = await _context.Hotels.FirstOrDefaultAsync(ht => ht.Id == id);
            if (hotel != null)
            {
                _context.Hotels.Remove(hotel);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ICollection<Hotel>> GetAllHotelAsync()
        {
            return await _context.Hotels.ToListAsync();
        }

        public ICollection<Hotel> GetHotelByTourDateId(Guid tourDateId)
        {
            var tour = _tourRepository.GetTourByTourDateId(tourDateId);
            return _context.Hotels
            .Include(ht => ht.HotelTours)
            .Where(h => h.HotelTours.Any(ht => ht.TourId == tour.Id))
            .ToList();
        }

        public async Task<PageResult<Hotel>> GetHotelPageAsync(int page, int pageSize)
        {
            var query = await GetAllHotelAsync();

            var totalCount = query.Count();
            var hotels = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
            .ToList();

            return new PageResult<Hotel>(hotels, totalCount, page, pageSize);
        }

        public async Task UpdateHotelAsync(Hotel hotel)
        {
            _context.Hotels.Update(hotel);
            await _context.SaveChangesAsync();
        }
    }
}
