using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosCotizacionReader
    {
        Task<IEnumerable<CotizacionItemList>> GetCotizacionesAsync();
        Task<IEnumerable<CotizacionItemList>> GetCotizacionesAsync(string codigoUsuario);
        Task<IEnumerable<VersionCotizacion>> GetVersionesCotizacionAsync(int codigoCotizacion);
        Task<VersionCotizacion> GetCotizacionPadreAsync(int codigoCotizacion, int versionPadre);
        Task<IEnumerable<VersionCotizacion>> GetVersionesCotizacionQueryAsync(int codigoCotizacion);
        Task<IEnumerable<VersionCotizacion>> GetVersionesCotizacionQueryAsync(int codigoCotizacion, int version);
        Task<IEnumerable<CotizacionItemList>> GetCotizacionesAsync(CotizacionFilter filtros);
        Task<IEnumerable<CotizacionItemList>> GetPendingAuthorizationCotizacionesAsync(CotizacionFilter filtros);
    }
}
