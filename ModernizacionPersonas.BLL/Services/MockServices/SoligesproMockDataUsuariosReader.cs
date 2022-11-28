using ModernizacionPersonas.DAL;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class SoligesproMockDataUsuariosReader : ISoligesproDataUsuariosReader
    {
        public async Task<IEnumerable<UserExternalInfo>> GetDirectoresAsync(int codigoSucursal)
        {
            var result = new List<UserExternalInfo>();
            result.Add(new UserExternalInfo { EmailUsuario = "director1@solidaria.com.co", LoginUsuario = "director1" });
            result.Add(new UserExternalInfo { EmailUsuario = "director2@solidaria.com.co", LoginUsuario = "director2" });

            return await Task.FromResult(result);
        }

        public async Task<UserExternalInfo> GetUserAsync(string usuario)
        {
            var result = new UserExternalInfo { EmailUsuario = "ogutierrez@solidaria.com.co", LoginUsuario = "OGUTIERREZ", Sucursal = 376, Zona = 29, CodigoCargo = 8 };
            return await Task.FromResult(result);
        }

        public async Task<IEnumerable<UserExternalInfo>> GetUserDirectorComercialAsync(int codigoSucursal)
        {
            var result = new List<UserExternalInfo>();
            return await Task.FromResult(result);
        }

        public async Task<UserExternalInfo> GetUserDirectorTecnicoAsync(int codigoSucursal)
        {
            var result = new UserExternalInfo { EmailUsuario = "dirtecnico@solidaria.com.co", LoginUsuario = "dirtecnico" };
            return await Task.FromResult(result);
        }

        public async Task<UserExternalInfo> GetUserDirectorZonalAsync(int codigoZona)
        {
            var result = new UserExternalInfo { EmailUsuario = "dirtecnico@solidaria.com.co", LoginUsuario = "dirtecnico" };
            return await Task.FromResult(result);
        }

        public async Task<UserExternalInfo> GetUserGerenteAsync(int codigoSucursal)
        {
            var result = new UserExternalInfo { EmailUsuario = "gerente@solidaria.com.co", LoginUsuario = "gerente" };
            return await Task.FromResult(result);
        }

        public async Task<IEnumerable<UserExternalInfo>> GetDirectoresAsync(int codigoSucursal, int codigoZona)
        {
            var result = new List<UserExternalInfo>();

            var directorComercial = await GetUserDirectorComercialAsync(codigoSucursal);
            result.AddRange(directorComercial);
            var directorTecnico = await GetUserDirectorTecnicoAsync(codigoSucursal);
            result.Add(directorTecnico);
            var gerente = await GetUserGerenteAsync(codigoSucursal);
            result.Add(gerente);
            var directorZonal = await GetUserDirectorZonalAsync(codigoZona);
            result.Add(directorZonal);

            return result;
        }
    }
}
