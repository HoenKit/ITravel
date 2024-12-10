namespace ITravel.Models
{
    public class HotelTour
    {
        public Guid HotelId { get; set; }
        public Hotel? Hotel { get; set; }
        public Guid TourId { get; set; }
        public Tour? Tour { get; set; }
    }
}
