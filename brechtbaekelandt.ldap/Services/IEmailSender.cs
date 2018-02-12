using System.Threading.Tasks;

namespace brechtbaekelandt.ldap.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
