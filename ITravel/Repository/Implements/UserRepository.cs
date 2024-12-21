using ITravel.Data;
using ITravel.Models;
using ITravel.Repository.Interfaces;

namespace ITravel.Repository.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void UpdateUser(AppUser user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}
