namespace ITravel.Repository.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailWithQRAsync(string email, string subject, string htmlMessage, Stream attachmentStream = null, string attachmentName = null);
    }
}
