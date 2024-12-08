namespace ITravel.Models
{
    public class Booking
    {
        public Guid Id { get; set; }
        public TourDate TourDate { get; set; }
        public AppUser User { get; set; }
        public DateTime BookingDate { get; set; }
        public int NumberOfPeople { get; set; }
        public int TotalPrice { get; set; }
        public bool IsUsed { get; set; } = false;
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Customer> Customers { get; set; }
    }
}
