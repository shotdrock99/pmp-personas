using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IAuthorizationsDataWriter
    {
        Task SaveAuthorizationAsync(CotizacionAuthorization model);

        Task DeleteAuthorizationAsync(int codigoCotizacion, int version);

        Task DeleteAuthorizationByQueryAsync(int codigoCotizacion, int version);

        Task SaveValidationsWEBAsync(int codigoCotizacion, int numCot, int version);
    }
}