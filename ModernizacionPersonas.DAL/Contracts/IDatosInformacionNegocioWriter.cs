using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosInformacionNegocioWriter
    {
        Task InsertarInformacionNegocioAsync(int codigoCotizacion, InformacionNegocio model, decimal factorG);
        Task ActualizarInformacionNegocioAsync(int codigoCotizacion, InformacionNegocio model);
        Task ActualizarDirectorComercialAsync(int codigoCotizacion, string usuario, string nombre, string email);
        Task UpdateCotizacionSelectedTasaAsync(int codigoCotizacion, decimal TasaSeleccionada);
        Task UpdateSelfAuthorizeFlagASync(int codigoCotizacion, bool selfAuthorize);
    }
}