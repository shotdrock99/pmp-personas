using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosGrupoAseguradoWriter
    {
        Task<int> CrearGrupoAseguradoAsync(GrupoAsegurado model);
        Task ActualizarGrupoAseguradoAsync(int codigoGrupoAsegurado, GrupoAsegurado model);
        Task ActualizarConListadoAsync(int codigoGrupoAsegurado, bool conListado);
        Task InsertarNumAseguradosAsync(int codigoGrupoAsegurado, int numeroAsegurados, int edadPromedio, int porcentajeAsegurado);
        Task EliminarGrupoAseguradoAsync(int codigoGrupoAsegurado);
        Task LimpiarGrupoAseguradoAsync(int codigoGrupoAsegurado);
        Task ActualizarGrupoAseguradoAsync(GrupoAsegurado model);
        Task UpdateNombreGrupoAseguradoAsync(int codigoCotizacion, GrupoAsegurado model);
        Task UpdateTipoEstructuraGrupoAseguradoAsync(int codigoCotizacion, GrupoAsegurado model);
    }
}
