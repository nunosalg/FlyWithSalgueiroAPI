namespace FlyWithSalgueiroAPI.Helpers
{
    public interface IMailHelper
    {
        Task<Response> SendEmailAsync(string to, string subject, string body);
    }
}
