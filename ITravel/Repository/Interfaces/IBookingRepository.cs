using ITravel.Models;

namespace ITravel.Repository.Interfaces
{
    public interface IBookingRepository
    {
        public ICollection<Booking> GetUnusedBookingsByTourDateIdAndUserId(Guid tourDateId, Guid userId);
        public Booking GetBookingById(Guid bookingId);
        public void CreateBooking (Booking booking);
        public void UpdateBooking (Booking booking);
        public ICollection<Booking> GetBookingByUser(Guid userId);
    }
}
