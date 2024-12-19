using ITravel.Data;
using ITravel.Models;
using ITravel.Repository.Interfaces;

namespace ITravel.Repository.Implements
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;
        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void CreateCustomer(Customer customer)
        {
            _context.Add(customer);
            _context.SaveChanges();
        }
    }
}
