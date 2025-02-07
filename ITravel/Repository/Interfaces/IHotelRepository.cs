using ITravel.Models;

namespace ITravel.Repository.Interfaces
{
    public interface IHotelRepository
    {
        public ICollection<Hotel> GetHotelByTourDateId(Guid tourDateId);
        public Task<ICollection<Hotel>> GetAllHotelAsync();
        public Task<PageResult<Hotel>> GetHotelPageAsync(int page, int pageSize);
        public Task CreateHotelAsync(Hotel hotel);
        public Task UpdateHotelAsync(Hotel hotel);
        public Task DeleteHotelAsync(Guid id);
    }
}
