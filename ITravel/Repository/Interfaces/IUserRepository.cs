using ITravel.Models;

namespace ITravel.Repository.Interfaces
{
    public interface IUserRepository
    {
        public void UpdateUser(AppUser user);
        public  Task<ICollection<AppUser>> GetUsersAsync();
        public Task<PageResult<AppUser>> GetUserPageAsync(int page, int pageSize);
        public Task<AppUser> GetUserByIdAsync(string userId);
        public Task UpdateUserAsync(AppUser user);
    }
}
