using SolidariaParametrizacion.Servicios.ParametrizacionServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidariaParametrizacion.Servicios
{
    public class DatosParametrizacionReader
    {
        Service1Client clientService = new Service1Client();

        /// <summary>
        /// Consulta las sucursales
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Sucursal>> TraerSucursalesAsync()
        {
            var result = await clientService.TraerSucursalesAsync();
            return result;
        }

        public async Task<IEnumerable<Ramo>> TraerRamosPorSucursalAsync(int codSucursal)
        {
            var result = await clientService.TraerRamosxSucursalAsync(codSucursal);
            return result;
        }

        /// <summary>
        /// Consulta los ramos
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Ramo>> TraerRamosAsync()
        {
            var result = await clientService.TraerRamosAsync();
            return result;
        }

        /// <summary>
        /// Consulta un ramo por codigo de ramo
        /// </summary>
        /// <param name="codRamo"></param>
        /// <returns></returns>
        public async Task<Ramo> TraerRamoPorCodigoAsync(int codRamo)
        {
            var result = await clientService.TraerRamoAsync(codRamo);
            return result;
        }

        /// <summary>
        /// Consulta los subramos de un ramo
        /// </summary>
        /// <param name="codRamo"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SubRamo>> TraerSubRamosPorRamoAsync(int codRamo)
        {
            var result = await clientService.TraerSubRamosxRamoAsync(codRamo);
            return result;
        }

        /// <summary>
        /// Consulta los agentes
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Agente>> TraerAgentesAsync()
        {
            var result = await clientService.TraerAgentesAsync();
            return result;
        }
    }
}
