using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosAmparoGrupoAseguradoWriter
    {
        Task<int> CrearAmparoGrupoAseguradoAsync(AmparoGrupoAsegurado model);
        Task ActualizarAmparoGrupoAseguradoAsync(int codigoAmparoGrupoAsegurado, AmparoGrupoAsegurado model);
        Task EliminarAmparoGrupoAseguradoAsync(int codigoAmparoGrupoAsegurado);
    }
}