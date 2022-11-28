using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosRangoGrupoAseguradoWriter
    {
        Task<int> CrearRangoGrupoAseguradoAsync(Rango model);
        Task ActualizarRangoGrupoAseguradoAsync(int codigoRangoGrupoAsegurado, Rango model);
        Task EliminarRangoGrupoAseguradoAsync(int codigoGrupoAsegurado);
    }
}