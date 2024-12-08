namespace ITravel.Models
{
    public class Image
    {
        public Guid Id { get; set; }
        public string URL { get; set; }
        public Tour Tour { get; set; }
    }
}
