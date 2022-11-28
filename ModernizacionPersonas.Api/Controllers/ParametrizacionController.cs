using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModernizacionPersonas.Common;
using ModernizacionPersonas.DAL;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.Entities;
using ParametrizacionServiceReference;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ParametrizacionController : ControllerBase
    {
        DatosParametrizacionReader servicioParametrizacion;
        DatosParametrizacionAppTableReader servicioApp;
        FichaTecnicaDataProvider fichaTecnicaData;

        public ParametrizacionController()
        {
            servicioParametrizacion = new DatosParametrizacionReader();
            servicioApp = new DatosParametrizacionAppTableReader();
            fichaTecnicaData = new FichaTecnicaDataProvider();
        }

        // GET: api/parametrizacion/sucursales
        [HttpGet("sucursales")]
        public async Task<ActionResult<IEnumerable<Sucursal>>> ObtenerSucursales()
        {
            var result = await servicioParametrizacion.TraerSucursalesAsync();
            return Ok(result);
        }

        // GET: api/parametrizacion/smmlv
        [HttpGet("smmlv")]
        public async Task<ActionResult<decimal>> ObtenerSmmlv()
        {
            var result = await fichaTecnicaData.getSalarioMinimoAsync();
            return Ok(result);
        }

        // GET: api/parametrizacion/siniestralidad
        [HttpGet("siniestralidad")]
        public async Task<ActionResult<IEnumerable<ParametrizacionApp>>> ObtenerSiniestralidad()
        {
            var result = await servicioApp.GetValorVariableAsync(3);
            return Ok(result);
        }

        // GET: api/parametrizacion/tipodocumento
        [HttpGet("tipodocumento")]
        public async Task<ActionResult<IEnumerable<Sucursal>>> ObtenerTipoDocumento()
        {
            var result = await servicioParametrizacion.TraerTipoDocumentoAsync();
            return Ok(result);
        }

        // GET: api/parametrizacion/actividadeconomica
        [HttpGet("actividadeconomica")]
        public async Task<ActionResult<IEnumerable<ActividadEconomica>>> ObtenerActividadEconomica()
        {
            var result = await servicioParametrizacion.TraerActividadEconomicaAsync();
            return Ok(result);
        }

        // GET: api/parametrizacion/paises
        [HttpGet("paises")]
        public async Task<ActionResult<IEnumerable<Pais>>> ObtenerPaises()
        {
            var result = await servicioParametrizacion.TraerPaisesAsync();
            return Ok(result);
        }

        // GET: api/parametrizacion/departamentos
        [HttpGet("departamentos")]
        public async Task<ActionResult<IEnumerable<Departamento>>> ObtenerDepartamentos()
        {
            var result = await servicioParametrizacion.TraerDepartamentosAsync();
            return Ok(result);
        }

        // GET: api/parametrizacion/municipios
        [HttpGet("municipios")]
        public async Task<ActionResult<IEnumerable<Municipio>>> ObtenerMunicipioPorDepartamento(int codDepartamento)
        {
            var result = await servicioParametrizacion.TraerMunicipiosxDepartamentoAsync(codDepartamento);
            return Ok(result);
        }

        // GET: api/parametrizacion/periodofacturacion
        [HttpGet("periodofacturacion")]
        public async Task<ActionResult<IEnumerable<Departamento>>> ObtenerPeriodoFacturacion()
        {
            var result = await servicioParametrizacion.TraerPeriodoFacturacionAsync();
            return Ok(result);
        }

        // GET: api/parametrizacion/tiponegocio
        [HttpGet("tiponegocio")]
        public async Task<ActionResult<IEnumerable<TipoNegocio>>> ObtenerTipoNegocio()
        {
            var result = await servicioParametrizacion.TraerTipoNegocioAsync();
            return Ok(result);
        }

        // GET: api/parametrizacion/sucursales
        [HttpGet("sucursales/{codSucursal}/ramos")]
        public async Task<ActionResult<IEnumerable<Sucursal>>> ObtenerRamosPorSucursal(int codSucursal)
        {
            var result = await servicioParametrizacion.TraerRamosPorSucursalAsync(codSucursal);
            return Ok(result);
        }

        // GET: api/parametrizacion/ramos
        [HttpGet("ramos")]
        public async Task<ActionResult<IEnumerable<Ramo>>> ObtenerRamos()
        {
            var result = await servicioParametrizacion.TraerRamosAsync();
            return Ok(result);
        }

        // GET: api/parametrizacion/ramos/5
        [HttpGet("ramos/{codRamo}")]
        public async Task<ActionResult<Ramo>> ObtenerRamo(int codRamo)
        {
            var result = await servicioParametrizacion.TraerRamoPorCodigoAsync(codRamo);
            return Ok(result);
        }

        // GET: api/parametrizacion/ramos/5/subramos
        [HttpGet("ramos/{codRamo}/subramos")]
        public async Task<ActionResult<IEnumerable<SubRamo>>> ObtenerSubRamosPorRamo(int codRamo)
        {
            var result = await servicioParametrizacion.TraerSubRamosPorRamoAsync(codRamo);
            return Ok(result);
        }        
    }
}