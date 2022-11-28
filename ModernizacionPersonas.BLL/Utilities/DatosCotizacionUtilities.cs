using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Utilities
{
    public class DatosCotizacionUtilities
    {
        private readonly InformacionPersonasReader datosPersonasReader;

        public DatosCotizacionUtilities(IDatosPersonasReader datosPersonasReader)
        {
            this.datosPersonasReader = new InformacionPersonasReader();
        }

        public async Task AgreggateCotizacionModelAsync(IEnumerable<CotizacionItemList> cotizaciones)
        {
            var sucursales = await this.datosPersonasReader.TraerSucursalesAsync();
            if (sucursales.Count() == 0)
            {
                throw new Exception("La consulta de sucursales no retorno ningun dato.");
                // LOG exception
            }

            var ramos = await this.datosPersonasReader.TraerRamosAsync();
            if (sucursales.Count() == 0)
            {
                throw new Exception("La consulta de ramos no retorno ningun dato.");
                // LOG exception
            }

            foreach (var cotizacion in cotizaciones)
            {
                var sucursal = sucursales.Where(x => x.CodigoSucursal == cotizacion.CodigoSucursal).FirstOrDefault();
                var ramo = ramos.Where(x => x.CodigoRamo == cotizacion.CodigoRamo).FirstOrDefault();
                var subramos = await this.datosPersonasReader.TraerSubRamosPorRamosAsync(cotizacion.CodigoRamo);
                var subramo = subramos.Where(x => x.CodigoSubRamo == cotizacion.CodigoSubramo).FirstOrDefault();
                //var tomadorResponse = await this.datosTomadorReader.LeerTomadorAsync(cotizacion.CodigoCotizacion);
                //var tomador = tomadorResponse.Tomador;
                //var nombreTomador = $"{tomador.Nombres} {tomador.PrimerApellido} {tomador.SegundoApellido}";

                cotizacion.CodigoZona = sucursal.CodigoZona;
                cotizacion.NombreZona = sucursal.NombreZona;
                cotizacion.CodigoSucursal = sucursal.CodigoSucursal;
                cotizacion.NombreSucursal = sucursal.NombreSucursal;
                cotizacion.CodigoRamo = ramo.CodigoRamo;
                cotizacion.NombreRamo = ramo.NombreAbreviado;
                cotizacion.CodigoSubramo = subramo.CodigoSubRamo;
                cotizacion.NombreSubramo = subramo.NombreSubRamo;
            }
        }
    }
}
