using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface ITransactionAttachmentsDataWriter
    {
        Task CreateTransactionAttachmentAsync(TransactionAttachment model);

        Task DeleteTransactionAttachmentAsync(int attachmentId);
    }
}
