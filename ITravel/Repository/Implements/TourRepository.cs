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
        .Where(td => !td.IsDeleted && !td.Tour.IsDeleted)
        .ToList();

        public async Task<PageResult<TourDate>> GetToursPagedAsync(
            int page,
            int pageSize,
            int? price = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string location = null)
        {
            var query = _context.ToursDate
                .Where(td => !td.IsDeleted && !td.Tour.IsDeleted)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(td => td.StartDate >= startDate.Value);
            }
            var priceRanges = new Dictionary<int, (int min, int max)>
            {
                { 1, (1000000, 3000000) },  
                { 2, (3000000, 5000000) },   
                { 3, (5000000, 10000000) },  
                { 4, (10000000, int.MaxValue) } 
            };

            if (price.HasValue && priceRanges.ContainsKey(price.Value))
            {
                var range = priceRanges[price.Value];
                query = query.Where(td => td.Tour.Price >= range.min && td.Tour.Price <= range.max);
            }

            if (endDate.HasValue)
            {
                query = query.Where(td => td.EndDate <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(td => td.Tour.Location.Contains(location));
            }

            var totalCount = await query.CountAsync();

            var tours = await query
                .Include(td => td.Tour)
                .ThenInclude(tour => tour.Images)
                .OrderByDescending(td => td.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResult<TourDate>(tours, totalCount, page, pageSize);
        }

        public Tour GetTourByTourDateId(Guid id) =>
            _context.Tours
           .Include(t => t.TourDates)
           .Where(td => !td.IsDeleted)
           .FirstOrDefault(t => t.TourDates.Any(td => td.Id == id));

        public TourDate GetTourDateById(Guid id) =>
        _context.ToursDate
        .Include(t => t.Tour)
            .ThenInclude(tour => tour.Images)
        .Where(td => !td.IsDeleted && !td.Tour.IsDeleted)
        .FirstOrDefault(t => t.Id == id);

        public void UpdateTourDate(TourDate newTourDate)
        {
            var tourDate = GetTourDateById(newTourDate.Id);
            _context.ToursDate.Update(tourDate);
            _context.SaveChanges();
        }

        public async Task<PageResult<TourDate>> GetToursPagedRecommendAsync(
    int page,
    int pageSize,
    int? price = null,
    DateTime? startDate = null,
    DateTime? endDate = null,
    List<string> suggestionList = null) 
        {
            var query = _context.ToursDate
                .Where(td => !td.IsDeleted && !td.Tour.IsDeleted)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(td => td.StartDate >= startDate.Value);
            }

            var priceRanges = new Dictionary<int, (int min, int max)>
    {
        { 1, (1000000, 3000000) },
        { 2, (3000000, 5000000) },
        { 3, (5000000, 10000000) },
        { 4, (10000000, int.MaxValue) }
    };

            if (price.HasValue && priceRanges.ContainsKey(price.Value))
            {
                var range = priceRanges[price.Value];
                query = query.Where(td => td.Tour.Price >= range.min && td.Tour.Price <= range.max);
            }

            if (endDate.HasValue)
            {
                query = query.Where(td => td.EndDate <= endDate.Value);
            }

            if (suggestionList != null && suggestionList.Any())
            {
                query = query.Where(td => suggestionList.Any(s => td.Tour.Location.Contains(s)));
            }

            var totalCount = await query.CountAsync();

            var tours = await query
                .Include(td => td.Tour)
                .ThenInclude(tour => tour.Images)
                .OrderByDescending(td => td.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResult<TourDate>(tours, totalCount, page, pageSize);
        }

    }
}
