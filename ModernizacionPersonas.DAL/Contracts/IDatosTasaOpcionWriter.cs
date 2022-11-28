using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosTasaOpcionWriter
    {
        Task CrearTasaOpcionAsync(TasaOpcion model);
        Task ActualizarTasaOpcionAsync(TasaOpcion model);        
        Task EliminarTasaOpcionAsync(int codigoGrupoAsegurado, int indiceOpcion);
    }
}