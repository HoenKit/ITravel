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

        public void DeleteReview(Guid id)
        {
            var review = GetReviewById(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                _context.SaveChanges();
            }
        }

        public Review GetReviewById(Guid id) =>
        _context.Reviews
        .Where(r => r.Id == id)
        .FirstOrDefault();


        public ICollection<Review> GetReviewsByTourDateId(Guid tourDateId)
        {
            var tourDate = _context.ToursDate
                .Where(td => td.Id == tourDateId)
                .FirstOrDefault();

            if (tourDate != null)
            {
                // Get TourId
                var tourId = tourDate.Tour.Id;

                // Get All TourDate From TourId
                var tourDates = _context.ToursDate
                    .Where(td => td.Tour.Id == tourId) 
                    .ToList();

                // Get All Booking from TourDates
                var bookings = _context.Bookings
                    .Where(b => tourDates.Select(td => td.Id).Contains(b.TourDate.Id)) 
                    .ToList();

                // Get All Review from Booking
                var reviews = _context.Reviews
                    .Include(r => r.Booking)
                        .ThenInclude(b => b.TourDate)
                    .Include(r => r.Booking)
                        .ThenInclude(b => b.User)
                    .Where(r => bookings.Select(b => b.Id).Contains(r.Booking.Id)) 
                    .ToList();
                return reviews;
            }
            return null;
        }


        public void UpdateReview(Review review)
        {
            var updateReview = GetReviewById(review.Id);
            if (updateReview != null)
            {
                _context.Reviews.Update(updateReview);
                _context.SaveChanges();
            }
        }
    }
}
