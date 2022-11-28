using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosUsuariosReader
    {
        Task<IEnumerable<ApplicationUserDTO>> GetUsuariosPersonasAsync();
        Task<ApplicationUserDTO> GetUsuarioPersonasById(int codigoUsuario);
        Task<ApplicationUserDTO> GetUsuarioPersonasByName();
    }
}
