using ITravel.Data;
using ITravel.Models;
using ITravel.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITravel.Repository.Implements
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;
        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void CreateReview(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
        }

        public ICollection<Review> GetReviewsByTourDateId(Guid tourDateId) =>
        _context.Reviews
        .Include(r => r.Booking)
            .ThenInclude(b => b.TourDate)
        .Include(u => u.Booking)
            .ThenInclude(ub => ub.User)
        .Where(r => r.Booking.TourDate.Id == tourDateId)
        .ToList();
        
    }
}
