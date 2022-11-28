using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosTomadorWriter
    {
        Task<int> CrearTomadorAsync(int codigoCotizacion, Tomador model);
        Task ActualizarTomadorAsync(int codigoCotizacion, Tomador model);

        Task DeleteTomadorAsync(int codigoCotizacion);
    }
}