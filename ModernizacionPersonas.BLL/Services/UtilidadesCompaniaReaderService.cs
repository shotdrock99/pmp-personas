using ModernizacionPersonas.Common;
using PersonasServiceReference;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class UtilidadesCompaniaReaderService
    {
        public static async Task<UtilidadCompannia> ReadAsync(int codigoRamo)
        {
            var clientService = ServiceConnectionFactory.GetParametrizacionPersonasClient();
            return await clientService.TraerUtilidadCompanniaAsync(codigoRamo);
        }
    }
}
