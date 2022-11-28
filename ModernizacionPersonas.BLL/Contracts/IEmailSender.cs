using ModernizacionPersonas.BLL.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Contracts
{
    public interface IEmailSender
    {
        Task<bool> SendEmailUsingTemplateAsync(SendEmailArgs args);
    }
}
