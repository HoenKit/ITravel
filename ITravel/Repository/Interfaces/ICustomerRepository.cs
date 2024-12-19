using ITravel.Models;

namespace ITravel.Repository.Interfaces
{
    public interface ICustomerRepository
    {
        public void CreateCustomer(Customer customer);
    }
}
