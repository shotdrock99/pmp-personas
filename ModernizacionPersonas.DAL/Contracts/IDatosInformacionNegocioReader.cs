using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosInformacionNegocioReader
    {
        Task<InformacionNegocio> LeerInformacionNegocioAsync(int codigoCotizacion);
    }
}
