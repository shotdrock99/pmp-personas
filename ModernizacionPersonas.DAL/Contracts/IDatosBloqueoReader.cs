using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosBloqueoReader
    {
        Task<IEnumerable<Bloqueo>> GetBloqueosAsync();
        Task<IEnumerable<Bloqueo>> GetBloqueosByCodigoCotizacionAsync(int codigoCotizacion);
    }
}
