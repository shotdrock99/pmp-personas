using PersonasServiceReference;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosPersonasReader
    {
        Task<IEnumerable<Zona>> GetZonasAsync();
        Task<Zona> GetZonaByCodigoAsync(int codigoZona);
        Task<IEnumerable<PersonasServiceReference.Sucursal>> TraerSucursalesAsync();
        Task<IEnumerable<PersonasServiceReference.Sucursal>> GetSucursalesByZonaAsync(int codigoZona);
        Task<PersonasServiceReference.Sucursal> TraerSucursalAsync(int codigoSucursal);
        Task<IEnumerable<PersonasServiceReference.Ramo>> TraerRamosAsync();
        Task<PersonasServiceReference.Ramo> TraerRamoAsync(int codiogoRamo);
        Task<IEnumerable<PersonasServiceReference.SubRamo>> TraerSubRamosPorRamosAsync(int codRamo);
        Task<PersonasServiceReference.SubRamo> TraerSubRamoAsync(int codRamo, int codSubRamo);
        Task<IEnumerable<Sector>> TraerSectoresAsync(int codRamo, int codSubRamo);
        Task<IEnumerable<RiesgoActividad>> TraerRiesgoActividadesAsync();
        Task<IEnumerable<TipoContratacion>> TraerTiposContratacionAsync();
        Task<IEnumerable<TipoContratacion>> TraerTiposContratacionxNegocioAsync(int codigotTipoNegocio);
        Task<IEnumerable<PersonasServiceReference.TipoNegocio>> TraerTiposNegocioAsync();
        Task<IEnumerable<PersonasServiceReference.TipoDocumento>> TraerTiposDocumentoAsync();
        Task<IEnumerable<TipoSumaAsegurada>> TraerTiposSumaAsegurada(int codigoRamo, int codigoSubramo);
        Task<IEnumerable<Amparo>> TraerAmparosAsync(int codigoRamo, int codigoSubramo, int codigoSector);
        Task<IEnumerable<Amparo>> TraerAmparosxRamoAsync(int codigoRamo);
        Task<Amparo> TraerAmparoxCodigoAsync(int codigoRamo, int codigoSubramo, int codigoAmparo, int codigoSector);
        Task<Intermediario> TraerIntermediarioAsync(int codigoSucursal, int codigoIntermediario);
        Task<IEnumerable<Tasa>> TraerTasasAsync(int codigoRamo, int codigoSubramo, int codigoSector);
        Task<Tasa> TraerTasaAsync(int codigoRamo, int codigoSubramo, int codigoSector, int codigoTasa);
        Task<decimal> TraerTasaSiniestralidadAsync(int codigoRamo);
        Task<RiesgoActividad> TraerRiesgoActividadAsync(int codigoRiesgo);
        Task<PersonasServiceReference.TipoNegocio> TraerTipoNegocioxCodigoAsync(int codigoTipoNegocio);
        Task<TipoContratacion> TraerTipoContratacionxCodigoAsync(int codigoTipoContratacion);
        Task<Intermediario> TraerIntermediarioPorDocumentoAsync(int codigoSucursal, int codigoTipoDocumento, string numeroDocumento);
        Task<IEnumerable<PerfilxEdad>> TraerPerfilesPorEdadAsync();
        Task<IEnumerable<RangoEdad>> TraerRangosPorPerfilEdadAsync(int codigoPerfil);
        Task<IEnumerable<RangoValor>> TraerRangosPorPerfilValorAsync(int codigoPerfil);
        Task<IEnumerable<PerfilxValor>> TraerPerfilesPorValorAsync();
        Task<IEnumerable<PersonasServiceReference.Sucursal>> TraerSucursalesPorIntermediarioAsync(string codigoIntermediario);
    }
}
