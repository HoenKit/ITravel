namespace ITravel.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string IdentifyId { get; set; }
        public Booking? Booking { get; set; }
    }
}
