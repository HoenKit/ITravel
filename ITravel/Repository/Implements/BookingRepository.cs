using ITravel.Data;
using ITravel.Models;
using ITravel.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITravel.Repository.Implements
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void CreateBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();
        }

        public Booking GetBookingById(Guid bookingId) => _context.Bookings.Where(b => b.Id == bookingId).FirstOrDefault();
        

        public ICollection<Booking> GetUnusedBookingsByTourDateIdAndUserId(Guid tourDateId, Guid userId) =>
        _context.Bookings
        .Include(n => n.TourDate)
        .Where(b => b.TourDate.Id == tourDateId
                    && b.User.Id == userId.ToString()
                    && b.Reviews.Count == 0) 
        .ToList();

    }
}
