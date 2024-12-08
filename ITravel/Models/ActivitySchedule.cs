namespace ITravel.Models
{
    public class ActivitySchedule
    {
        public Guid Id { get; set; }
        public Tour Tour { get; set; }
        public int DayNumber { get; set; }
        public string ActivityName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
    }
}
