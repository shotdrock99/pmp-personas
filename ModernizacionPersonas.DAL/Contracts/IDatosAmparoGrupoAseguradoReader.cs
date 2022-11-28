using ModernizacionPersonas.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosAmparoGrupoAseguradoReader
    {
        Task<IEnumerable<AmparoGrupoAsegurado>> LeerAmparoGrupoAseguradoAsync(int codigoGrupoAsegurado);        
    }
}
