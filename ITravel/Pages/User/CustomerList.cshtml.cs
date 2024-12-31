using ITravel.Models;
using ITravel.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITravel.Pages.User
{
    [Authorize(Roles = "Users")]
    public class CustomerListModel : PageModel
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomerListModel(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public ICollection<Customer> Customers { get; set; }
        public void OnGet(Guid id)
        {
            Customers = _customerRepository.GetCustomerByBooking(id);
        }
    }
}
