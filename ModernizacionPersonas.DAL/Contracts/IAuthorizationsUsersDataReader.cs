using ModernizacionPersonas.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IAuthorizationsUsersDataReader
    {
        Task<IEnumerable<AuthorizationUser>> GetAuthorizationsUsersAsync(int codigoCotizacion, int version);
    }
}
