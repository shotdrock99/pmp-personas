using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class DatosSiniestralidadProvider
    {

        private readonly IDatosSiniestralidadWriter siniestralidadWriterService;
        private readonly IDatosSiniestralidadReader siniestralidadReaderService;
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly CotizacionStateWriter cotizacionStateWriter;

        public DatosSiniestralidadProvider()
        {
            this.siniestralidadWriterService = new DatosSiniestralidadTableWriter();
            this.siniestralidadReaderService = new DatosSiniestralidadTableReader();
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.cotizacionStateWriter = new CotizacionStateWriter();
        }

        public async Task<ActionResponseBase> InsertarSiniestralidadCotizacionAsync(int codigoCotizacion, int version, List<Siniestralidad> model)
        {
            try
            {
                // Consulta informacion de negocio
                var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
                await this.siniestralidadWriterService.EliminarSiniestralidadAsync(codigoCotizacion);
                foreach (Siniestralidad siniestro in model)
                {
                    siniestro.CodigoCotizacion = codigoCotizacion;
                    var response = await this.siniestralidadWriterService.CrearSiniestralidadAsync(siniestro);
                }

                // Actualizar estado de la cotizacion                                 
                if (informacionNegocio.CotizacionState < CotizacionState.OnSiniestralidad)
                {
                    await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.OnSiniestralidad);
                }

                return new ActionResponseBase()
                {
                    CodigoCotizacion = codigoCotizacion,
                    CodigoEstadoCotizacion = (int)CotizacionState.OnSiniestralidad
                };
            }
            catch (Exception ex)
            {
                throw new Exception("SiniestralidadPersonasWriter :: InsertarSiniestralidadCotizacionAsync", ex);
            }
        }

        public async Task<IEnumerable<Siniestralidad>> LeerSiniestralidadCotizacionAsync(int codigoCotizacion)
        {
            try
            {
                var response = await this.siniestralidadReaderService.GetSiniestralidadAsync(codigoCotizacion);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("SiniestralidadPersonasWriter :: LeerSiniestralidadCotizacionAsync", ex);
            }
        }
    }
}
