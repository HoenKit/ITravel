namespace ITravel.Models
{
    public class Hotel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string ImageURL { get; set; }
        public ICollection<HotelTour> HotelTours { get; set; } 
    }
}
