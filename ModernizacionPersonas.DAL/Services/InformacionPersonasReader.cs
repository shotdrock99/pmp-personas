using ModernizacionPersonas.Common;
using PersonasServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class InformacionPersonasReader : IDatosPersonasReader
    {
        Service1Client clientService;

        public InformacionPersonasReader()
        {
            clientService = ServiceConnectionFactory.GetParametrizacionPersonasClient();
        }

        public async Task<IEnumerable<Zona>> GetZonasAsync()
        {
            var result = await clientService.TraerZonasAsync();
            return result;
        }

        public async Task<Zona> GetZonaByCodigoAsync(int codigoZona)
        {
            var result = await clientService.TraerZonaxCodigoAsync(codigoZona);
            return result;
        }

        public async Task<IEnumerable<Sucursal>> TraerSucursalesAsync()
        {
            var result = await clientService.TraerSucursalesAsync();
            return result;
        }

        public async Task<IEnumerable<Sucursal>> TraerSucursalesPorIntermediarioAsync(string codigoIntermediario)
        {
            var clave = int.Parse(codigoIntermediario);
            var result = await clientService.TraerSucursalesxIntermediarioAsync(clave);
            return result;
        }

        public async Task<IEnumerable<Sucursal>> GetSucursalesByZonaAsync(int codigoZona)
        {
            var result = await clientService.TraerSucursalesxZonaAsync(codigoZona);
            return result;
        }

        public async Task<Sucursal> TraerSucursalAsync(int codigoSucursal)
        {
            var result = await clientService.TraerSucursalxCodigoAsync(codigoSucursal);
            return result;
        }

        public async Task<IEnumerable<Ramo>> TraerRamosAsync()
        {
            var result = await clientService.TraerRamosAsync();
            return result;
        }

        public async Task<IEnumerable<SubRamo>> TraerSubRamosPorRamosAsync(int codRamo)
        {
            var result = await clientService.TraerSubRamosxRamoAsync(codRamo);
            return result;
        }

        public async Task<IEnumerable<Sector>> TraerSectoresAsync(int codigoRamo, int CodigoSubramo)
        {
            var result = await clientService.TraerSectoresAsync(codigoRamo, CodigoSubramo);
            return result;
        }

        public async Task<IEnumerable<Tasa>> TraerTasasAsync(int codigoRamo, int codigoSubramo, int codigoSector)
        {
            var result = await clientService.TraerTasasAsync(codigoRamo, codigoSubramo, codigoSector);
            return result;
        }

        public async Task<IEnumerable<RiesgoActividad>> TraerRiesgoActividadesAsync()
        {
            var result = await clientService.TraerRiesgoActividadesAsync();
            return result;
        }

        public async Task<IEnumerable<TipoContratacion>> TraerTiposContratacionAsync()
        {
            var result = await clientService.TraerTipoContratacionAsync();
            return result;
        }

        public async Task<IEnumerable<TipoContratacion>> TraerTiposContratacionxNegocioAsync(int codigotTipoNegocio)
        {
            var result = await clientService.TraerTipoContratacionxNegocioAsync(codigotTipoNegocio);
            return result;
        }

        public async Task<IEnumerable<TipoNegocio>> TraerTiposNegocioAsync()
        {
            var result = await clientService.TraerTipoNegocioAsync();
            return result;
        }

        public async Task<IEnumerable<TipoDocumento>> TraerTiposDocumentoAsync()
        {
            var result = await clientService.TraerTipoDocumentoAsync();
            return result;
        }

        public async Task<IEnumerable<TipoSumaAsegurada>> TraerTiposSumaAsegurada(int codigoRamo, int codigoSubramo)
        {
            var result = await clientService.TraerTipoSumaAseguradaAsync(codigoRamo, codigoSubramo);
            return result;
        }

        public async Task<IEnumerable<Amparo>> TraerAmparosAsync(int codigoRamo, int codigoSubramo, int codigoSector)
        {
            if (codigoSector == 0)
            {
                return await clientService.TraerAmparosxRamoAsync(codigoRamo);
            }

            var result = await clientService.TraerAmparosAsync(codigoRamo, codigoSubramo, codigoSector);
            return result;
        }

        public async Task<IEnumerable<Amparo>> TraerAmparosxRamoAsync(int codigoRamo)
        {
            var result = await clientService.TraerAmparosxRamoAsync(codigoRamo);
            return result;
        }

        public async Task<Amparo> TraerAmparoxCodigoAsync(int codigoRamo, int codigoSubramo, int codigoAmparo, int codigoSector)
        {
            var result = await clientService.TraerAmparoxCodigoAsync(codigoRamo, codigoSubramo, codigoAmparo, codigoSector);
            return result;
        }

        public async Task<Intermediario> TraerIntermediarioAsync(int codigoSucursal, int codigoIntermediario)
        {
            var result = await clientService.TraerIntermediarioxCodigoAsync(codigoSucursal, codigoIntermediario);
            return result;
        }

        public async Task<Intermediario> TraerIntermediarioPorDocumentoAsync(int codigoSucursal, int codigoTipoDocumento, string numeroDocumento)
        {
            var result = await clientService.TraerIntermediarioxDocumentoAsync(codigoSucursal, codigoTipoDocumento, numeroDocumento);
            return result;
        }

        public async Task<IEnumerable<PerfilxEdad>> TraerPerfilesPorEdadAsync()
        {
            var result = await clientService.TraerPerfilesxEdadAsync();
            return result;
        }

        public async Task<IEnumerable<RangoValor>> TraerRangosPorPerfilValorAsync(int codigoPerfil)
        {
            var result = await clientService.TraerRangosxPerfilValorAsync(codigoPerfil);
            return result;
        }

        public async Task<IEnumerable<PerfilxValor>> TraerPerfilesPorValorAsync()
        {
            var result = await clientService.TraerPerfilesxValorAsync();
            return result;
        }

        public async Task<IEnumerable<RangoEdad>> TraerRangosPorPerfilEdadAsync(int codigoPerfil)
        {
            var result = await clientService.TraerRangosxPerfilEdadAsync(codigoPerfil);
            return result;
        }

        public async Task<Ramo> TraerRamoAsync(int codiogoRamo)
        {
            var result = await clientService.TraerRamoAsync(codiogoRamo);
            return result;
        }

        public async Task<SubRamo> TraerSubRamoAsync(int codRamo, int codSubRamo)
        {
            var result = await clientService.TraerSubRamoAsync(codRamo, codSubRamo);
            return result;
        }

        public async Task<Sector> TraerSectorAsync(int codigoRamo, int codigoSubramo, int codigoSector)
        {
            var result = await clientService.TraerSectorAsync(codigoRamo, codigoSubramo, codigoSector);
            return result;
        }

        public async Task<Tasa> TraerTasaAsync(int codigoRamo, int codigoSubramo, int codigoSector, int codigoTasa)
        {
            var result = await clientService.TraerTasaAsync(codigoRamo, codigoSubramo, codigoSector, codigoTasa);
            return result;
        }

        public async Task<RiesgoActividad> TraerRiesgoActividadAsync(int codigoRiesgo)
        {
            var result = await clientService.TraerRiesgoActividadAsync(codigoRiesgo);
            return result;
        }

        public async Task<TipoContratacion> TraerTipoContratacionxCodigoAsync(int codigoTipoContratacion)
        {
            var result = await clientService.TraerTipoContratacionxCodigoAsync(codigoTipoContratacion);
            return result;
        }

        public async Task<TipoNegocio> TraerTipoNegocioxCodigoAsync(int codigoTipoNegocio)
        {
            var result = await clientService.TraerTipoNegocioxCodigoAsync(codigoTipoNegocio);
            return result;
        }

        public async Task<decimal> TraerTasaSiniestralidadAsync(int codigoRamo)
        {
            var result = await clientService.TraerTasaSiniestralidadAsync(codigoRamo);
            return result;
        }
    }
}
