using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosAseguradoReader
    {
        Task<IEnumerable<Asegurado>> LeerAseguradosAsync(int codigoGrupoAsegurado);        
    }
}
