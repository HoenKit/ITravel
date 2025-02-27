using ITravel.Models;

namespace ITravel.Repository.Interfaces
{
    public interface IProviderRepository
    {
        public Task<Provider> GetProviderByUserIdAsync(string userId);
    }
}
