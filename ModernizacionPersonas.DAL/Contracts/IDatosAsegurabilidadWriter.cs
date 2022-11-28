using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosAsegurabilidadWriter
    {
        Task<int> CrearAsegurabilidadAsync(Asegurabilidad model, int cdigoCotiacion);

        Task<bool> ActualizarAsegurabilidadAsync(int codigoCotizacion, Asegurabilidad model);

        Task<bool> EliminarAsegurabilidadAsync(int codigoCotizacion);

        Task<bool> EliminarAsegurabilidadIdAsync(int codigoCotizacion, int codigoAsegurabilidad);
    }
}