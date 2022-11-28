using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosParametrizacionEmailReader
    {
        Task<GetEmailTextoResponse> LeerParametrizacionEmailAsync(int tomadorIntermediario, int codigoRamo);
        Task<EmailParametrizacion> LeerParametrizacionEmailCodigoAsync(int codigoSeccion, int codigoTemplate);
    }
}
