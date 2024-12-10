namespace ITravel.Models
{
    public class RestaurantTour
    {   
        public Guid RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }
        public Guid TourId { get; set; }
        public Tour? Tour { get; set; }  
    }
}
