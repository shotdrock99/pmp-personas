using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosOpcionValorAseguradoReader
    {
        Task<IEnumerable<OpcionValorAsegurado>> LeerOpcionValorAseguradoAsync(int codigoAmparoGrupoAsegurado);

        Task<decimal> TraerSumatoriaOpcionValorAseguradoAsync(int codigoGrupoAsegurado, int opcionIndice);
    }
}
