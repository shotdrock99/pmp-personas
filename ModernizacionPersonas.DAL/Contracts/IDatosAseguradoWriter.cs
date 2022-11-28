using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosAseguradoWriter
    {
        Task<InsertarBloqueAseguradosResponse> InsertarBloqueAseguradosAsync(IEnumerable<Asegurado> model, int codigoGrupoAsegurado);
        Task ActualizarAseguradoAsync(int codigoAsegurado, Asegurado model);
        Task EliminarAseguradosAsync(int codigoAsegurado);
        Task<InsertarBloqueAseguradosResponse> ValidarAseguradosAsync(int codigoGrupoAsegurado, string numeroDocumento);
    }
}
