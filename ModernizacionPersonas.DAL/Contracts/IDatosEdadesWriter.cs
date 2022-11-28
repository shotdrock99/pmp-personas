using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosEdadesWriter
    {
        Task<int> InsertarEdadAmparoAsync(int codigoGrupoasegurado, int codigoGrupoAmparo, EdadAmparoGrupoAsegurado model);

        Task ActualizarEdadesAsync(int codigoEdades, EdadAmparoGrupoAsegurado model);

        Task EliminarEdadesAsync(int codigoEdades);
    }
}