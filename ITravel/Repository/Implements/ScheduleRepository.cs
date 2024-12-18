using ITravel.Data;
using ITravel.Models;
using ITravel.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITravel.Repository.Implements
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly ITourRepository _tourRepository;
        private readonly ApplicationDbContext _context;
        public ScheduleRepository(ITourRepository tourRepository, ApplicationDbContext context)
        {
            _tourRepository = tourRepository;
            _context = context;
        }
        public ICollection<ActivitySchedule> GetScheduleByTourDateId(Guid tourDateId)
        {
            var tour = _tourRepository.GetTourByTourDateId(tourDateId);
            return _context.ActivitySchedules
            .Include(ht => ht.Tour)
            .Where(h => h.Tour.Id == tour.Id)
            .ToList();
        }
    }
}
