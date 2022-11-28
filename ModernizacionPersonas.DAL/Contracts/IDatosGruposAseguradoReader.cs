using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosGruposAseguradoReader
    {
        // TODO debe recibir version
        Task<IEnumerable<GrupoAsegurado>> GetGruposAseguradosAsync(int codigoCotizacion);
        Task<GrupoAsegurado> GetGrupoAseguradoAsync(int codigoGrupoAsegurado);
    }
}
