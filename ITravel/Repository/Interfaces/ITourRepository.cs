using ITravel.Models;

namespace ITravel.Repository.Interfaces
{
    public interface ITourRepository
    {
        public ICollection<TourDate> Get5RecentTours();
        public TourDate GetTourDateById(Guid id);
        public Tour GetTourByTourDateId(Guid id);
    }
}
