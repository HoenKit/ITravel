namespace ITravel.Models
{
    public class Review
    {
        public Guid Id { get; set; }
        public Booking Booking { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
    }
}
