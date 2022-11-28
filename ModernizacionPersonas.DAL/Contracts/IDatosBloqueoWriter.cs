using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosBloqueoWriter
    {
        Task BloquearAsync(int codigoCotizacion, int codigoUsuario);
        Task DesbloquearAsync(int codigoCotizacion);
    }
}
