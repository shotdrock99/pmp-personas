using ModernizacionPersonas.Common;
using PersonasServiceReference;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class GastosCompaniaReader
    {
        public static async Task<GastosCompannia> ReadAsync(int codigoRamo)
        {
            var clientService = ServiceConnectionFactory.GetParametrizacionPersonasClient();
            return await clientService.TraerGastosCompanniaAsync(codigoRamo);
        }
    }
}
