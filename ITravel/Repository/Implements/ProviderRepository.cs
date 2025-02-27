using ITravel.Data;
using ITravel.Models;
using ITravel.Pages.User;
using ITravel.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITravel.Repository.Implements
{
    public class ProviderRepository: IProviderRepository
    {
        private readonly ApplicationDbContext _context;
        public ProviderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Provider> GetProviderByUserIdAsync(string userId)
        {
            return await _context.Providers
                        .AsNoTracking() 
                        .Include(p => p.User)
                        .FirstOrDefaultAsync(p => p.UserId == userId);
        }

    }
}
