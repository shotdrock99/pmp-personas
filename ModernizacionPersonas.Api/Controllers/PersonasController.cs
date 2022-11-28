using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.Common;
using ModernizacionPersonas.DAL;
using ModernizacionPersonas.DAL.ReportesWebServices;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.DAL.Soligespro;
using ModernizacionPersonas.Entities;
using ParametrizacionServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PersonasController : ControllerBase
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IDatosPersonasReader informacionPersonasReader;
        private readonly AdministracionPersonasReader administracionPersonasReader;
        private readonly ReportesWebDataZonasReader reportesWebDataZonasReader;
        private readonly ISoligesproDataUsuariosReader soligesproUsersDataReader;
        private readonly InformacionTomadorDataProvider informacionTomadorDataProvider;

        public PersonasController(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.informacionTomadorDataProvider = new InformacionTomadorDataProvider();

            this.informacionPersonasReader = new InformacionPersonasReader();
            //this.informacionPersonasReader = new DatosPersonasReaderMock();
            this.administracionPersonasReader = new AdministracionPersonasReader();
            this.reportesWebDataZonasReader = new ReportesWebDataZonasReader();
            this.soligesproUsersDataReader = new SoligesproDataUsuariosReader();
        }

        [HttpGet("zonas")]
        public async Task<ActionResult> GetZonas()
        {
            //var result = await this.reportesWebDataZonasReader.GetZonasAsync();
            var roleId = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.RoleId).Value;
            var codigoZonaValue = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.ZonaId).Value;
            var codigoSucursalValue = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.SucursalId).Value;
            var codigoZona = int.Parse(codigoZonaValue);
            var result = await this.informacionPersonasReader.GetZonasAsync();
            // Dir. Técnico Zonal
            if(codigoSucursalValue == "800")
            {
                if (codigoZona == 0)
                {
                    return Ok(result);
                }
                else
                {
                    result = result.Where(x => x.CodigoZona == codigoZona);
                }
            }
            else
            {
                result = result.Where(x => x.CodigoZona == codigoZona);
            }
            
            //if (roleId == "4")
            //{
            //    result = result.Where(x => x.CodigoZona == codigoZona);
            //}
            //else
            //{
            //    if (codigoSucursalValue != "800")
            //    {
            //        // TODO validar comportamiento por roles con GCortes
            //        result = result.Where(x => x.CodigoZona == codigoZona);
            //    }
            //}

            return Ok(result);
        }

        [HttpGet("sucursales")]
        public async Task<ActionResult> ObtenerSucursalesAsync(int codigoZona)
        {
            var result = new List<PersonasServiceReference.Sucursal>();
            var roleId = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.RoleId).Value;
            var codigoZonaValue = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.ZonaId).Value;
            var codigoZonaUsuario = int.Parse(codigoZonaValue);
            var sucursalId = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.SucursalId).Value;
            var sucursales = await informacionPersonasReader.TraerSucursalesAsync();

            if(codigoZona == 0)
            {
                if(sucursalId == "800")
                {
                    if(codigoZonaUsuario == 0)
                    {
                        return Ok(sucursales.ToList());
                    }
                    else
                    {
                        var result0 = await informacionPersonasReader.GetSucursalesByZonaAsync(codigoZonaUsuario);
                        return Ok(result0);
                    }
                }
                else
                {
                    var filtered = sucursales.Where(x => x.CodigoSucursal.ToString() == sucursalId);
                    result = filtered.ToList();
                    if (result.Count() == sucursales.Count())
                    {
                        var emptyList = new List<PersonasServiceReference.Sucursal>();
                        return Ok(emptyList);
                    }
                }

            }
            else
            {
                var result0 = await informacionPersonasReader.GetSucursalesByZonaAsync(codigoZona);
                return Ok(result0);
            }
            //if (codigoZona == 0)
            //{
            //    codigoZona = codigoZonaUsuario;
            //}

            //if (codigoZona != codigoZonaUsuario)
            //{
            //    // TODO check gicortes, resultados diferentes en filtro(result) y metodo(result0) 
            //    var result0 = await informacionPersonasReader.GetSucursalesByZonaAsync(codigoZona);
            //    result = sucursales.Where(x => x.CodigoZona == codigoZona).ToList();
            //    return Ok(result);
            //}

            //// Intermediario
            //if (roleId == "9")
            //{
            //    var codigoIntermediario = this.httpContextAccessor.HttpContext.User.FindFirst(ModernizacionPersonasClaimNames.UserName).Value;
            //    var response = await informacionPersonasReader.TraerSucursalesPorIntermediarioAsync(codigoIntermediario);
            //    result = response.ToList();
            //}
            //// Dir. Técnico Zonal
            //else if (roleId == "4")
            //{
            //    // TODO GetSucursalesByZonaAsync no retorna valores
            //    var response = await informacionPersonasReader.GetSucursalesByZonaAsync(codigoZonaUsuario);
            //    if (response.Count() == 0)
            //    {
            //        response = sucursales.Where(x => x.CodigoZona == codigoZona).ToList();
            //    }

            //    result = response.ToList();
            //}
            //else
            //{
            //    var response = await informacionPersonasReader.TraerSucursalesAsync();
            //    result = response.ToList();
            //    if (sucursalId != "800")
            //    {
            //        var filtered = response.Where(x => x.CodigoSucursal.ToString() == sucursalId);
            //        result = filtered.ToList();
            //        if (result.Count() == sucursales.Count())
            //        {
            //            var emptyList = new List<PersonasServiceReference.Sucursal>();
            //            return Ok(emptyList);
            //        }
            //    }
            //}

            return Ok(result);
        }

        // GET: api/parametrizacion/ramos
        [HttpGet("ramos")]
        public async Task<ActionResult> ObtenerRamosAsync()
        {
            var result = await informacionPersonasReader.TraerRamosAsync();
            return Ok(result);
        }

        // GET: api/parametrizacion/ramos/5/subramos
        [HttpGet("ramos/{codRamo}/subramos")]
        public async Task<ActionResult<IEnumerable<SubRamo>>> ObtenerSubRamosPorRamoAsync(int codRamo)
        {
            var result = await informacionPersonasReader.TraerSubRamosPorRamosAsync(codRamo);
            return Ok(result);
        }

        [HttpGet("tiposriesgo")]
        public async Task<ActionResult> ObtenerTiposRiesgoAsync()
        {
            var result = await informacionPersonasReader.TraerRiesgoActividadesAsync();
            return Ok(result);
        }

        [HttpGet("sectores")]
        public async Task<ActionResult> ObtenerSectoresAsync(int codigoRamo, int codigoSubramo)
        {
            var result = await informacionPersonasReader.TraerSectoresAsync(codigoRamo, codigoSubramo);
            return Ok(result);
        }

        [HttpGet("tasas")]
        public async Task<ActionResult> ObtenerTasasAsync(int codigoRamo, int codigoSubramo, int codigoSector)
        {
            var result = await informacionPersonasReader.TraerTasasAsync(codigoRamo, codigoSubramo, codigoSector);
            return Ok(result);
        }

        [HttpGet("tiposcontratacion")]
        public async Task<ActionResult> ObtenerTiposContratacion(int codigoTipoNegocio)
        {
            var result = await informacionPersonasReader.TraerTiposContratacionxNegocioAsync(codigoTipoNegocio);
            return Ok(result);
        }

        [HttpGet("tomador")]
        public async Task<ActionResult> TraerTomadorAsync(int codigoTipoDocumento, string numeroDocumento)
        {
            try {
                var data = await informacionTomadorDataProvider.ValidarTomador(codigoTipoDocumento, numeroDocumento);
                if (data.ErrorCode == "OK")
                {
                    var response = await administracionPersonasReader.TraerTomadorAsync(codigoTipoDocumento, numeroDocumento);
                    return Ok(response);
                }
                else
                {
                    return Ok(data);
                }
                
            } catch (Exception ex) {
                return Ok(ex.StackTrace + "ERROR:" + ex.Message);
            }
            /*
            var response = await administracionPersonasReader.TraerTomadorAsync(codigoTipoDocumento, numeroDocumento);
            return Ok(response);*/
        }

        [HttpGet("tiposnegocio")]
        public async Task<ActionResult> ObtenerTiposNegocioAsync()
        {
            var result = await informacionPersonasReader.TraerTiposNegocioAsync();
            return Ok(result);
        }

        [HttpGet("tiposdocumento")]
        public async Task<ActionResult> ObtenerTiposDocumentoAsync()
        {
            var result = await informacionPersonasReader.TraerTiposDocumentoAsync();
            return Ok(result);
        }

        [HttpGet("tipossumaasegurada")]
        public async Task<ActionResult> ObtenerTiposSumaAsegurada(int codigoRamo, int codigoSubramo)
        {
            var result = await informacionPersonasReader.TraerTiposSumaAsegurada(codigoRamo, codigoSubramo);
            return Ok(result);
        }

        [HttpGet("amparos")]
        public async Task<ActionResult> ObtenerAmparosAsync(int codigoRamo, int codigoSubramo = 0, int codigoSector = 0)
        {
            var result = await informacionPersonasReader.TraerAmparosAsync(codigoRamo, codigoSubramo, codigoSector);
            return Ok(result);
        }

        [HttpGet("intermediario")]
        public async Task<ActionResult> ConsultarIntermediario(int codigoSucursal, int codigoIntermediario)
        {
           var result = await informacionPersonasReader.TraerIntermediarioAsync(codigoSucursal, codigoIntermediario);
            // return Ok(result);
            if (result.NumeroDocumento == null)
            {
                return Ok(result);
            }
            else
            {
                var data = await informacionTomadorDataProvider.ValidarTomador(result.CodigoTipoDocumento, result.NumeroDocumento);
                if (data.ErrorCode == "OK")
                {
                    return Ok(result);
                }
                else
                {
                    return Ok(data);
                }
            }
        }

        [HttpGet("intermediariopordocumento")]
        public async Task<ActionResult> ConsultarIntermediarioPorDocumento(int codigoSucursal, int codigoTipoDocumento, string numeroDocumento)
        {
            var result = await informacionPersonasReader.TraerIntermediarioPorDocumentoAsync(codigoSucursal, codigoTipoDocumento, numeroDocumento);
            return Ok(result);
        }

        [HttpGet("directorescomerciales")]
        public async Task<IEnumerable<UserExternalInfo>> LoadDirectoresComercialesAsync(int codigoSucursal)
        {
            var contacts = await this.soligesproUsersDataReader.GetDirectoresAsync(codigoSucursal);
            var gerente = await this.soligesproUsersDataReader.GetUserGerenteAsync(codigoSucursal);
            var comerciales = contacts.Where(x => x.CodigoCargo == 16).ToList();
            comerciales.Add(gerente);
            return comerciales;
        }

        [HttpGet("perfiles/edad")]
        public async Task<ActionResult> ConsultarPerfilesPorEdad()
        {
            var result = await informacionPersonasReader.TraerPerfilesPorEdadAsync();
            return Ok(result);
        }

        [HttpGet("perfiles/edad/rangos")]
        public async Task<ActionResult> ConsultarRangosPorPerfilEdad(int codigoPerfil)
        {
            var result = await informacionPersonasReader.TraerRangosPorPerfilEdadAsync(codigoPerfil);
            return Ok(result);
        }

        [HttpGet("perfiles/valor")]
        public async Task<ActionResult> ConsultarPerfilesPorValor()
        {
            var result = await informacionPersonasReader.TraerPerfilesPorValorAsync();
            return Ok(result);
        }

        [HttpGet("perfiles/valor/rangos")]
        public async Task<ActionResult> ConsultarRangosPorPerfilValor(int codigoPerfil)
        {
            var result = await informacionPersonasReader.TraerRangosPorPerfilValorAsync(codigoPerfil);
            return Ok(result);
        }

        [Route("test")]
        [HttpGet]
        public ActionResult TestMethod()
        {
            return Ok("Conexión exitosa a PersonasController :: TestMethod");
        }
    }
}