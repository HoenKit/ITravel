using ITravel.Models;

namespace ITravel.Repository.Interfaces
{
    public interface IBookingRepository
    {
        public ICollection<Booking> GetUnusedBookingsByTourDateIdAndUserId(Guid tourDateId, Guid userId);
        public Booking GetBookingById(Guid bookingId);
    }
}
