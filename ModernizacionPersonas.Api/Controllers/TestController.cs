using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernizacionPersonas.Api.Providers;
using ModernizacionPersonas.BLL.Services;
using ModernizacionPersonas.Common;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.DAL.SISEServices;
using System;
using System.Collections.Generic;

namespace ModernizacionPersonas.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly DatosSiniestralidadProvider siniestralidadPersonasWriter;
        private readonly DatosGruposAseguradosMapper gruposAseguradosMapper;
        private readonly SISEAseguradosWriter SISEWriterService;
        private readonly DatosParametrizacionReader servicioParametrizacion;
        private readonly IDatosPersonasReader informacionPersonasReader;
        private readonly AdministracionPersonasReader administracionPersonasReader;
        private readonly ResumenCotizacionProvider resumenDataBuilder;
        private readonly List<Exception> Errors;

        public TestController(IHttpContextAccessor httpContextAccessor)
        {
            this.Errors = new List<Exception>();

            try
            {
                this.siniestralidadPersonasWriter = new DatosSiniestralidadProvider();
                this.gruposAseguradosMapper = new DatosGruposAseguradosMapper();
                this.SISEWriterService = new SISEAseguradosWriter();
                this.servicioParametrizacion = new DatosParametrizacionReader();
                this.informacionPersonasReader = new InformacionPersonasReader();
                this.administracionPersonasReader = new AdministracionPersonasReader();
                this.resumenDataBuilder = new ResumenCotizacionProvider();
            }
            catch (Exception ex)
            {
                this.Errors.Add(ex);
            }
        }

        [Route("")]
        [HttpGet]
        public IActionResult TestApiAsync()
        {
            if (this.Errors.Count > 0)
            {
                return BadRequest(string.Join(",", this.Errors));
            }

            return Ok("El API de cotización se inició exitosamente.");
        }
    }
}