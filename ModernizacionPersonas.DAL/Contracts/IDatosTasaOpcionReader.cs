using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosTasaOpcionReader
    {
        Task<TasaOpcion> LeerTasaOpcionAsync(int codigoGrupoAsegurado, int indiceOpcion);
    }
}
