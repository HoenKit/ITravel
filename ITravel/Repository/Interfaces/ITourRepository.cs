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
        public Task<ICollection<Tour>> GetAllTourAsync();
        public Task<PageResult<Tour>> GetTourPageAsync(int page, int pageSize);
        public void UpdateTourDate(TourDate newTourDate);
        public Task<PageResult<TourDate>> GetToursPagedRecommendAsync(
           int page,
           int pageSize,
           int? price = null,
           DateTime? startDate = null,
           DateTime? endDate = null,
           List<string> suggestionList = null);
        public Task<Tour> GetTourByTourIdAsync(Guid tourId);
    }
}
