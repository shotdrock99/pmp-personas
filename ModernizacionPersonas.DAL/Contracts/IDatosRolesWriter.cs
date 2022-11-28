using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosRolesWriter
    {
        Task<int> GuardarRolAsync(Rol model);
        Task ActualizarRolAsync(Rol model);
        Task EliminarRolAsync(int codigoRol, string usuarioLog);
        Task DesactivarRolAsync(int codigoRol, string usuarioLog);
    }
}
