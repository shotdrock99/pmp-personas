using ParametrizacionServiceReference;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Common
{
    public class DatosParametrizacionReader
    {
        Service1Client clientService;

        public DatosParametrizacionReader()
        {
            clientService = ServiceConnectionFactory.GetParametrizacionClient();
        }        

        /// <summary>
        /// Consulta las sucursales
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Sucursal>> TraerSucursalesAsync()
        {
            var result = await clientService.TraerSucursalesAsync();
            return result;
        }

        /// <summary>
        /// Consulta sucursale
        /// /// </summary>
        /// <returns></returns>
        public async Task<Sucursal> TraerSucursalAsync(int codigoSucursal)
        {
            var result = await clientService.TraerSucursalAsync(codigoSucursal);
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

        /// <summary>
        /// Consulta tipos de documentos
        /// </summary>
        /// <returns>Lista tipo de Documentos</returns>
        public async Task<IEnumerable<TipoDocumento>> TraerTipoDocumentoAsync()
        {
            var result = await clientService.TraerTipoDocumentoAsync();
            return result;
        }


        /// <summary>
        /// Consulta Actividad Económica
        /// </summary>
        /// <returns>Lista Actividades Economicas</returns>
        public async Task<IEnumerable<ActividadEconomica>> TraerActividadEconomicaAsync()
        {
            var result = await clientService.TraerActividadEconomicaAsync();
            return result;
        }

        /// <summary>
        /// Consulta Paises
        /// </summary>
        /// <returns>Lista Paises</returns>
        public async Task<IEnumerable<Pais>> TraerPaisesAsync()
        {
            var result = await clientService.TraerPaisesAsync();
            return result;
        }

        /// <summary>
        /// Consulta Departamentos
        /// </summary>
        /// <returns>Lista Departamentos</returns>
        public async Task<IEnumerable<Departamento>> TraerDepartamentosAsync()
        {
            var result = await clientService.TraerDepartamentosAsync();
            return result;
        }

        /// <summary>
        /// Consulta Departamentos
        /// </summary>
        /// <returns>Departamento por Codigo</returns>
        public async Task<Departamento> TraerDepartamentoAsync(int codigoDepartamento)
        {
            var result = await clientService.TraerDepartamentoAsync(codigoDepartamento);
            return result;
        }

        /// <summary>
        /// Consulta Municipios x Departamento
        /// </summary>
        /// /// <param name="codDepartamento"></param>
        /// <returns>Lista Departamentos</returns>
        public async Task<IEnumerable<Municipio>> TraerMunicipiosxDepartamentoAsync(int codDepartamento)
        {
            var result = await clientService.TraerMunicipiosxDepartamentoAsync(codDepartamento);
            return result;
        }

        /// <summary>
        /// Consulta Municipios x Departamento
        /// </summary>
        /// /// <param name="codDepartamento"></param>
        /// /// /// <param name="codMunicipio"></param>
        /// <returns>Lista Departamentos</returns>
        public async Task<Municipio> TraerMunicipioAsync(int codDepartamento, int codMunicipio)
        {
            var result = await clientService.TraerMunicipioAsync(codDepartamento, codMunicipio);
            return result;
        }

        /// <summary>
        /// Consulta Periodos Facturación
        /// </summary>
        /// <returns>Lista Periodos Facturación</returns>
        public async Task<IEnumerable<PeriodoFacturacion>> TraerPeriodoFacturacionAsync()
        {
            var result = await clientService.TraerPeriodoFacturacionAsync();
            return result;
        }

        /// <summary>
        /// Consulta Tipo Negocio
        /// </summary>
        /// <returns>Lista Tipo Negocios</returns>
        public async Task<IEnumerable<TipoNegocio>> TraerTipoNegocioAsync()
        {
            var result = await clientService.TraerTipoNegocioAsync();
            return result;
        }
       
    }
}
