﻿namespace ITravel.Models
{
    public class Tour
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Image> Images { get; set; }
        public string Location { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<ActivitySchedule> ActivitySchedules { get; set; }
        public Provider Provider { get; set; }
        public ICollection<HotelTour> HotelTours { get; set; }
        public ICollection<RestaurantTour> RestaurantTours { get; set; }

    }
}