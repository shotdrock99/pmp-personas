using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface ITransactionCommentsDataReader
    {
        Task<IEnumerable<TransactionComment>> GetTransactionsCommentsAsync(int codigoTransaction);
    }
}
