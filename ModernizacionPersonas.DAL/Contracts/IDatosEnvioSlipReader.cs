using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Services;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosEnvioSlipReader
    {
        Task<IEnumerable<EnvioSlip>> LeerEnvioSlipAsync(int codigoCotizacion);
        Task<IEnumerable<AdjuntoEnvioSlip>> LeerAdjuntoEnvioSlipAsync(int codigoCotizacion);
       
        
    }
}
