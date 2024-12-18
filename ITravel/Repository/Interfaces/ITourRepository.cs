using ITravel.Models;
using System.Threading.Tasks;

namespace ITravel.Repository.Interfaces
{
    public interface ITourRepository
    {
        public ICollection<TourDate> Get5RecentTours();
        public TourDate GetTourDateById(Guid id);
        public Task<PageResult<TourDate>> GetToursPagedAsync(
           int page,
           int pageSize,
           int? price = null,
           DateTime? startDate = null,
           DateTime? endDate = null,
           string location = null);
       
        public Tour GetTourByTourDateId(Guid id);
    }
}
