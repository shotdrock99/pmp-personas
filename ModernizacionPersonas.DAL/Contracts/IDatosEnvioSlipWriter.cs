using ModernizacionPersonas.DAL.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosEnvioSlipWriter
    {
        Task<int> CrearEnvioSlipAsync(int codigoCotizacion, string emailTo, string emailCC, string emailComments);
        Task<int> CrearAdjuntoEnvioSlipAsync(int codigoCotizacion, byte[] adjunto, string fileName);
        Task<int> BorrarAdjuntoEnvioSlipByNamesAsync(int codigoCotizacion, string fileName);
    }
}
