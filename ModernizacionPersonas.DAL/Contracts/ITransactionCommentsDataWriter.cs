using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface ITransactionCommentsDataWriter
    {
        Task CreateTransactionCommentAsync(TransactionComment model);
    }
}
