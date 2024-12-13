using ITravel.Models;

namespace ITravel.Repository.Interfaces
{
    public interface IRestaurantRepository
    {
        public ICollection<Restaurant> GetRestaurantByTourDateId(Guid tourDateId);
    }
}
