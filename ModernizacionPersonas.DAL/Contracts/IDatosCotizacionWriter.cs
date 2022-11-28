using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosCotizacionWriter
    {
        Task<DbActionResponse> CrearCotizacionAsync(InicializarCotizacionArgs args);
        Task<DbActionResponse> CopiarCotizacionAsync(int userId, int cotizacion, int version, string username);
        Task<DbActionResponse> CreateVersionCotizacionAsync(int userId, int cotizacion, int versionCopia, bool copia);
        Task<DbActionResponse> CreateVersionAltCotizacionAsync(int userId, int cotizacion);
        Task UpdateCotizacionTomadorAsync(int codigoCotizacion, int codigoTomador);
        Task InsertarUsuarioNotificadoAsync(int codigoCotizacion, int version, string codigoUsuario);
        Task<DbActionResponse> CambiarEstadoAsync(int codigoCotizacion, CotizacionState codigoEstado);
        Task<DbActionResponse> CloseCotizacionAsyn(int codigoCotizacion);
        Task<DbActionResponse> UpdateModifiedFlagCotizacionAsync(int codigoCotizacion, bool modificado);
        Task<DbActionResponse> UpdateEnvioSlipCotizacionAsync(int codigoCotizacion);
        Task<DbActionResponse> UpdateLastAuthorCotizacionAsync(int codigoCotizacion, int lastAuthor);
        Task<DbActionResponse> ConfirmCotizacionAsync(int codigoCotizacion, int causalId, int userId, ConfirmCotizacionAction action);
        Task<DbActionResponse> ContinueCotizacion(int codigoCotizacion, int userId, string comments);
    }
}