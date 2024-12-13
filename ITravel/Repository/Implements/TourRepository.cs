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

        public Tour GetTourByTourDateId(Guid id) =>
        _context.Tours
            .Include(t => t.TourDates) 
            .FirstOrDefault(t => t.TourDates.Any(td => td.Id == id));


        public TourDate GetTourDateById(Guid id) =>
        _context.ToursDate
        .Include(t => t.Tour)
            .ThenInclude(tour => tour.Images) 
        .FirstOrDefault(t => t.Id == id); 

    }
}
