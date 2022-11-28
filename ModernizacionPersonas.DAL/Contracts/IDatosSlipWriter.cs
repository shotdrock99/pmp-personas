using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosSlipWriter
    {
        Task GuardarConfiguracionSlip(int codigoCotizacion, SlipVariable variable);
        Task SeleccionarClausulasSlipAsync(int codigoCotizacion, int seccion);
        Task ClearSeleccionClausulaAsync(int codigoCotizacion);
    }
}
