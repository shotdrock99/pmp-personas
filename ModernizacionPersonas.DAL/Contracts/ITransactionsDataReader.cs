using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface ITransactionsDataReader
    {
        Task<GetTransactionsResult> GetTransactionsAsync(int codigoCotizacion, int version);
        Task<IEnumerable<CotizacionTransaction>> GetAuthorizationTransactionsAsync(int codigoCotizacion, int version);
    }
}