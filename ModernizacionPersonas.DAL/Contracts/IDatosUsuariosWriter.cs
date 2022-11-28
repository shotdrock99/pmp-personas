using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosUsuariosWriter
    {
        Task<int> CrearUsuarioAsync(ApplicationUser model);
        Task ActualizarUsuarioAsyn(ApplicationUser model);
        Task ActivarDesactivarUsuarioAsync(int codigoUsuario, string usuarioLog);
    }
}
