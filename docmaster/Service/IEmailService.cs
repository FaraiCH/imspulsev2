using docmaster.Models;

namespace docmaster.Service
{
    public interface IEmailService
    {
        bool SendEmail(EmailDataModel emailData);
    }
}
