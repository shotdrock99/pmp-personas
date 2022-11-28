using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosSiniestralidadWriter
    {
        Task<int> CrearSiniestralidadAsync(Siniestralidad model);
        Task ActualizarSiniestralidadAsync(Siniestralidad model);
        Task EliminarSiniestralidadAsync(int codigoSiniestralidad);
    }
}