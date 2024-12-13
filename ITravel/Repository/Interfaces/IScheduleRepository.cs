using ITravel.Models;

namespace ITravel.Repository.Interfaces
{
    public interface IScheduleRepository
    {
        public ICollection<ActivitySchedule> GetScheduleByTourDateId(Guid tourDateId);
    }
}
