using ModernizacionPersonas.DAL.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IAuthorizationsDataReader
    {
        Task<GetAuthorizationControlsResponse> GetAuthorizationsByCotizacionAsync(int codigoCotizacion, int version);
    }
}
