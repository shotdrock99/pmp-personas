using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IAuthorizationUsersDataWriter
    {
        Task SaveAuthorizationUserAsync(AuthorizationUser model);
        Task RemoveAuthorizationUsersAsync(int codigoCotizacion, int version);
        Task RemoveAuthorizationUsersByQueryAsync(int codigoCotizacion, int version);
        Task<int> GetClausulasEspAsync(int codigoCotizacion);
    }
}