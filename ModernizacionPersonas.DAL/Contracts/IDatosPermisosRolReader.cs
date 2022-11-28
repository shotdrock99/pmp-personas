using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosPermisosRolReader
    {
        Task<IEnumerable<Permiso>> GetPermisosAsync();
        Task<IEnumerable<Permiso>> GetPermisosRolAsync( int codigoRol);

    }
}
