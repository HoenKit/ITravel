using ITravel.Models;

namespace ITravel.Repository.Interfaces
{
    public interface IReviewRepository
    {
    public ICollection<Review> GetReviewsByTourDateId(Guid tourDateId);
    public void CreateReview(Review review);
    }
}
