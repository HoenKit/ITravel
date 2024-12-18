using ITravel.Models;

namespace ITravel.Repository.Interfaces
{
    public interface IHotelRepository
    {
        public ICollection<Hotel> GetHotelByTourDateId(Guid tourDateId);
    }
}
