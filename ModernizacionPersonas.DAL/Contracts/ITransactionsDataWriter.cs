using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface ITransactionsDataWriter
    {
        Task<int> CreateTransactionAsync(CotizacionTransaction model);
        Task UpdateTransactionAsync(CotizacionTransaction model);
        Task DeleteTransactionAsync(int codigoCotizacion);
    }
}