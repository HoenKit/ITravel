namespace ITravel.Models
{
    public class TourDate
    {
        public Guid Id { get; set; }
        public Tour Tour { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxCapacity { get; set; }
        public int CurrentCapacity { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<Booking> Bookings { get; set; }
    }
}
