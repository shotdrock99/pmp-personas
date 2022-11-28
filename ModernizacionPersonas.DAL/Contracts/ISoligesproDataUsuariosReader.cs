using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL
{
    public interface ISoligesproDataUsuariosReader
    {
        Task<UserExternalInfo> GetUserAsync(string usuario);
        Task<IEnumerable<UserExternalInfo>> GetUserDirectorComercialAsync(int codigoSucursal);
        Task<UserExternalInfo> GetUserDirectorTecnicoAsync(int codigoSucursal);
        Task<UserExternalInfo> GetUserGerenteAsync(int codigoSucursal);
        Task<UserExternalInfo> GetUserDirectorZonalAsync(int codigoZona);
        Task<IEnumerable<UserExternalInfo>> GetDirectoresAsync(int codigoSucursal);
        Task<IEnumerable<UserExternalInfo>> GetDirectoresAsync(int codigoSucursal, int codigoZona);
    }
}
