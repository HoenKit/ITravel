namespace ITravel.Models.PayOs
{
    public record Response(
     int error,
     String message,
     object? data
 );
}
