using ITravel.Data;
using ITravel.Models;
using ITravel.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
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
        private readonly IEmailSender _emailSender;
        public UserRepository(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender; 
        }

        public async Task<ICollection<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .Select(u => new AppUser
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    ProfileStatus = u.ProfileStatus,
                    Address = u.Address,
                    FullName = u.FullName,
                })
                .ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task UpdateUserAsync(AppUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<PageResult<AppUser>> GetUserPageAsync(int page, int pageSize)
        {
            var query = await GetUsersAsync();

            var totalCount = query.Count();

            var users = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PageResult<AppUser>(users, totalCount, page, pageSize);
        }
    }
}
