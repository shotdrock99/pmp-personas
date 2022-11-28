using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosPermisosRolWriter
    {
        Task GuardarPermisoRolAsync(PermisoRol model);
        Task ActualizarPermisoRolAsync(PermisoRol model);
        Task EliminarPermisoRolAsync(int codigoPermisoRol);
        Task EliminarPermisosRolAsync(int codigoRol);

    }
}
