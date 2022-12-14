using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosRolesReader
    {
        Task<IEnumerable<Rol>> GetRolesAsync();
        Task<IEnumerable<RolSISE>> GetRolesSiseAsync();
    }
}
