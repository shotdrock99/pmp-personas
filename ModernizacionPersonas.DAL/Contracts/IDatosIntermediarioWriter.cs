using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosIntermediarioWriter
    {
        Task<int> CreateIntermediarioAsync(int codigoCotizacion, Intermediario model);
        Task UpdateIntermediarioAsync(int codigoCotizacion, Intermediario model);
        Task DeleteIntermediarioAsync(int codigoIntermediario);
        Task DeleteIntermediariosCotizacionAsync(int codigoCotizacion);
    }
}