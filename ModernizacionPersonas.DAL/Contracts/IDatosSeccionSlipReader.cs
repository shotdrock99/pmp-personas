using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosSeccionSlipReader
    {
        Task<IEnumerable<SeccionSlip>> GetSeccionesAsync();
        Task<IEnumerable<SeccionSlip>> GetSeccionAsync( int codigoSeccion);

    }
}
