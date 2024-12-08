namespace ITravel.Models
{
    public class Restaurant
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string ImageURL { get; set; }
        public ICollection<RestaurantTour> RestaurantTours { get; set; }
    }
}
