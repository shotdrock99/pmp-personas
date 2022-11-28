using Newtonsoft.Json;
using PersonasServiceReference;
using System;
using System.Collections.Generic;

namespace ModernizacionPersonas.Entities
{
    public partial class FichaTecnica
    {
        public int CodigoCotizacion { get; internal set; }
        public string NumeroCotizacion { get; internal set; }
        public CotizacionState EstadoCotizacion { get; set; }
        public string Zona { get; set; }
        public string Sucursal { get; set; }
        public string Ramo { get; set; }
        public string Subramo { get; set; }
        public Tasa TipoTasa { get; set; }
        public string Sector { get; set; }
        public InformacionTomadorFichaTecnica InformacionTomador { get; set; }
        public bool TieneSiniestralidad { get; set; }
        public IEnumerable<GrupoAseguradoFichaTecnica> GruposAsegurados { get; set; }
        public InformacionFactorG InformacionFactorG { get; set; }
        public PygAnualFichaTecnica PygAnual { get; set; }
        public PerfilEdades PerfilEdades { get; set; }
        public PerfilValores PerfilValores { get; set; }
        public InformacionSiniestralidad InformacionSiniestralidad { get; set; }
        public DirectorComercialInfo DirectorComercialInfo { get; set; }
    }

    public class DirectorComercialInfo
    {
        public string Usuario { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
    }

    public class GrupoAseguradoFichaTecnica
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; }
        public TipoSumaAsegurada TipoSumaAsegurada { get; set; }
        public bool ConListaAsegurados { get; set; }        
        public int NumeroAsegurados { get; set; }
        public int EdadPromedio { get; set; }
        public decimal PorcentajeEsperado { get; set; }
        public IEnumerable<AmparoGrupoAseguradoFichaTecnica> Amparos { get; set; }
        public IEnumerable<AmparoGrupoAseguradoFichaTecnica> Asistencias { get; set; }
        public PrimasGrupoAsegurado Primas { get; set; }
        public int NumeroOpciones { get; set; }
        public decimal TasaSiniestralidad { get; set; }
        public IEnumerable<TotalValorAseguradoOpcion> ValoresAseguradosTotales { get; set; }
        public IEnumerable<ProyeccionFinanciera> ProyeccionesFinancieras { get; set; }
        public bool ConDistribucionAsegurados { get; set; }
        public int AseguradosOpcion1 { get; set; }
        public int AseguradosOpcion2 { get; set; }
        public int AseguradosOpcion3 { get; set; }
        public IEnumerable<ValorAseguradoOpcionResumen> Opciones { get; set; }

    }

    public class PrimasGrupoAsegurado
    {
        [JsonProperty("primaIndividualAnual")]
        public List<ValorOpcionKeyValue> PrimaIndividualAnual { get; set; }

        [JsonProperty("primaIndividualTotal")]
        public List<ValorOpcionKeyValue> PrimaIndividualTotal { get; set; }

        [JsonProperty("primaTotalAnual")]
        public List<ValorOpcionKeyValue> PrimaTotalAnualxTasa { get; set; }

        [JsonProperty("totalPrimaAnual")]
        public List<ValorOpcionKeyValue> PrimaTotalAnual { get; set; }

        public PrimasGrupoAsegurado()
        {
            this.PrimaIndividualAnual = new List<ValorOpcionKeyValue>();
            this.PrimaIndividualTotal = new List<ValorOpcionKeyValue>();
            this.PrimaTotalAnualxTasa = new List<ValorOpcionKeyValue>();
            this.PrimaTotalAnual = new List<ValorOpcionKeyValue>();
        }
    }

    public class ValorOpcionKeyValue
    {
        [JsonProperty("indiceOpcion")]
        public int IndiceOpcion { get; set; }

        [JsonProperty("valor")]
        public decimal Valor { get; set; }
    }

    public class ProyeccionFinanciera
    {
        public bool EsProyeccionSiniestralidad { get; set; }
        [JsonProperty("tasaComercialAnual")]
        public List<ValorOpcionKeyValue> TasaComercialAnual { get; set; }

        [JsonProperty("porcentajeDescuento")]
        public List<ValorOpcionKeyValue> PorcentajeDescuento { get; internal set; }

        [JsonProperty("porcentajeRecargo")]
        public List<ValorOpcionKeyValue> PorcentajeRecargo { get; internal set; }

        [JsonProperty("tasaComercialTotal")]
        public List<ValorOpcionKeyValue> TasaComercialTotal { get; set; }

        public ProyeccionFinanciera()
        {
            this.TasaComercialAnual = new List<ValorOpcionKeyValue>();
            this.PorcentajeDescuento = new List<ValorOpcionKeyValue>();
            this.PorcentajeRecargo = new List<ValorOpcionKeyValue>();
            this.TasaComercialTotal = new List<ValorOpcionKeyValue>();
        }
    }

    public class ProyeccionFinancieraSiniestralidad
    {
        [JsonProperty("tasaRiesgoSiniestralidad")]
        public decimal TasaRiesgo { get; set; }

        [JsonProperty("tasaComercialAnual")]
        public decimal TasaComercialAnual { get; set; }

        [JsonProperty("porcentajeDescuento")]
        public decimal PorcentajeDescuento { get; set; }

        [JsonProperty("porcentajeRecargo")]
        public decimal PorcentajeRecargo { get; set; }

        [JsonProperty("tasaComercialTotal")]
        public decimal TasaComercialTotal { get; set; }
    }

    public class AmparoGrupoAseguradoFichaTecnica
    {
        public int CodigoAmparoGrupoAsegurado { get; set; }
        public int CodigoGrupoAsegurado { get; set; }
        public int CodigoAmparo { get; set; }
        public string NombreAmparo { get; set; }
        public IEnumerable<OpcionValorAseguradoFichaTecnica> OpcionesValores { get; set; }
        public int CodigoGrupoAmparo { get; set; }
        public string NombreGrupoAmparo { get; set; }
        public Amparo AmparoInfo { get; set; }
    }

    public class OpcionValorAseguradoFichaTecnica
    {
        [JsonProperty("indiceOpcion")]
        public int IndiceOpcion { get; set; }

        [JsonProperty("codigoOpcionValorAsegurado")]
        public int CodigoOpcionValorAsegurado { get; set; }

        [JsonProperty("codigoAmparoGrupoAsegurado")]
        public int CodigoAmparoGrupoAsegurado { get; set; }

        [JsonProperty("valorAsegurado")]
        public decimal ValorAsegurado { get; set; }

        public decimal TasaRiesgo { get; set; }

        [JsonProperty("tasaComercial")]
        public decimal TasaComercial { get; set; }

        [JsonProperty("primaIndividualAnual")]
        public decimal PrimaAnual { get; set; }
    }

    public class TotalValorAseguradoOpcion
    {
        [JsonProperty("indiceOpcion")]
        public int IndiceOpcion { get; set; }

        [JsonProperty("valorAseguradoTotal")]
        public decimal ValorAseguradoTotal { get; set; }
    }

    public partial class SiniestralidadFichaTecnica
    {
        [JsonProperty("vigenciaDesde")]
        public DateTime VigenciaDesde { get; set; }

        [JsonProperty("vigenciaHasta")]
        public DateTime VigenciaHasta { get; set; }

        [JsonProperty("valorIncurrido")]
        public decimal ValorIncurrido { get; set; }

        [JsonProperty("numeroCasos")]
        public int NumeroCasos { get; set; }

        [JsonProperty("siniestralidadPromedio")]
        public decimal SiniestralidadPromedio { get; set; }

        [JsonProperty("IBNR")]
        public decimal IBNR { get; set; }

        [JsonProperty("sumValorIncurridoIBNR")]
        public decimal SumValorIncurridoIBNR { get; set; }
    }

    public partial class InformacionFactorG
    {
        [JsonProperty("comisionIntermediario")]
        public decimal ComisionIntermediario { get; set; }

        [JsonProperty("gastosRetorno")]
        public decimal GastosRetorno { get; set; }

        [JsonProperty("otroGgastos")]
        public decimal OtroGgastos { get; set; }

        [JsonProperty("gastosCompania")]
        public decimal GastosCompania { get; set; }

        [JsonProperty("utilidad")]
        public decimal Utilidad { get; set; }

        [JsonProperty("totalFactorG")]
        public decimal TotalFactorG { get; set; }
        public decimal IvaComisionIntermediario { get; set; }
        public decimal IvaGastosRetorno { get; set; }
    }

    public partial class InformacionTomadorFichaTecnica
    {
        [JsonProperty("tomador")]
        public string Tomador { get; set; }

        [JsonProperty("vigencia")]
        public Vigencia Vigencia { get; set; }

        [JsonProperty("actividadRiesgo")]
        public string ActividadRiesgo { get; set; }

        [JsonProperty("ciudad")]
        public string Ciudad { get; set; }

        [JsonProperty("intermediarios")]
        public IEnumerable<IntermediarioFichaTecnica> Intermediarios { get; set; }

        [JsonProperty("aseguradoraActual")]
        public string AseguradoraActual { get; set; }

        [JsonProperty("tipoNegocio")]
        public string TipoNegocio { get; set; }

        [JsonProperty("tipoContratacion")]
        public string TipoContratacion { get; set; }

        [JsonProperty("conListadoAsegurados")]
        public bool ConListadoAsegurados { get; set; }
    }

    public partial class IntermediarioFichaTecnica
    {
        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("porcentajeParticipacion")]
        public decimal PorcentajeParticipacion { get; set; }
    }

    public partial class Vigencia
    {
        [JsonProperty("desde")]
        public string Desde { get; set; }

        [JsonProperty("hasta")]
        public string Hasta { get; set; }
    }

    public partial class PerfilEdades
    {
        [JsonProperty("rangos")]
        public List<PerfilEdadesRango> Rangos { get; set; }

        [JsonProperty("totales")]
        public PerfilTotales Totales { get; set; }

        public PerfilEdades()
        {
            this.Rangos = new List<PerfilEdadesRango>();
        }
    }

    public partial class InformacionSiniestralidad
    {
        public decimal TasaRiesgo { get; set; }

        [JsonProperty("siniestralidad")]
        public List<SiniestralidadFichaTecnica> Siniestros { get; set; }

        public SiniestralidadTotales Totales { get; set; }

        public ProyeccionFinancieraSiniestralidad ProyeccionFinanciera { get; set; }

        public TasasSiniestralidad TasasSiniestralidad { get; set; }

        public InformacionSiniestralidad()
        {
            this.Siniestros = new List<SiniestralidadFichaTecnica>();
            this.TasasSiniestralidad = new TasasSiniestralidad();
        }
    }

    public class TasasSiniestralidad
    {
        public decimal TasaPuraRiesgo { get; set; }
        public decimal TasaComercial { get; set; }
        public decimal PrimaAnualComercial { get; set; }
        public decimal PrimaIndividualComercial { get; set; }
    }

    public partial class PerfilEdadesRango
    {
        [JsonProperty("indice")]
        public int Indice { get; set; }
        [JsonProperty("edadDesde")]
        public decimal EdadDesde { get; set; }

        [JsonProperty("edadHasta")]
        public decimal EdadHasta { get; set; }

        [JsonProperty("cantidadAsegurados")]
        public decimal CantidadAsegurados { get; set; }

        [JsonProperty("porcentajeParticipacionAsegurados")]
        public decimal PorcentajeParticipacionAsegurados { get; set; }

        [JsonProperty("valorAseguradoTotal")]
        public decimal ValorAseguradoTotal { get; set; }

        [JsonProperty("porcentajeParticipacionValorAsegurado")]
        public decimal PorcentajeParticipacionValorAsegurado { get; set; }

        [JsonProperty("promedioValorAsegurado")]
        public decimal PromedioValorAsegurado { get; set; }

        [JsonProperty("aseguradoMayorEdad")]
        public decimal AseguradoMayorEdad { get; set; }
    }

    public partial class SiniestralidadTotales
    {
        [JsonProperty("sumValorincurrido")]
        public decimal SumValorIncurrido { get; set; }

        [JsonProperty("sumNumeroCasos")]
        public decimal SumNumeroCasos { get; set; }

        [JsonProperty("sumValorIBNR")]
        public decimal SumValorPlusIBNR { get; set; }
        public SiniestralidadTotales()
        {
            this.SumNumeroCasos = 0;
            this.SumValorIncurrido = 0;
            this.SumValorPlusIBNR = 0;
        }
    }

    public partial class PerfilTotales
    {
        [JsonProperty("sumCantidadAsegurados")]
        public decimal SumCantidadAsegurados { get; set; }

        [JsonProperty("sumPorcentajeParticipacionAsegurados")]
        public decimal SumPorcentajeParticipacionAsegurados { get; set; }

        [JsonProperty("sumValorAseguradoTotal")]
        public decimal SumValorAseguradoTotal { get; set; }

        [JsonProperty("sumPorcentajeParticipacionValorAsegurado")]
        public decimal SumPorcentajeParticipacionValorAsegurado { get; set; }

        [JsonProperty("sumPromedioValorAsegurado")]
        public decimal SumPromedioValorAsegurado { get; set; }
    }

    public partial class PerfilValores
    {
        [JsonProperty("rangos")]
        public List<PerfilValoresRango> Rangos { get; set; }

        [JsonProperty("totales")]
        public PerfilTotales Totales { get; set; }

        public PerfilValores()
        {
            this.Rangos = new List<PerfilValoresRango>();
        }
    }

    public partial class PerfilValoresRango
    {
        [JsonProperty("indice")]
        public int Indice { get; set; }
        [JsonProperty("valorAseguradoDesde")]
        public decimal ValorAseguradoDesde { get; set; }

        [JsonProperty("valorAseguradoHasta")]
        public decimal ValorAseguradoHasta { get; set; }

        [JsonProperty("cantidadAsegurados")]
        public decimal CantidadAsegurados { get; set; }

        [JsonProperty("porcentajeParticipacionAsegurados")]
        public decimal PorcentajeParticipacionAsegurados { get; set; }

        [JsonProperty("valorAseguradoTotal")]
        public decimal ValorAseguradoTotal { get; set; }

        [JsonProperty("porcentajeParticipacionValorAsegurado")]
        public decimal PorcentajeParticipacionValorAsegurado { get; set; }

        [JsonProperty("promedioValorAsegurado")]
        public decimal PromedioValorAsegurado { get; set; }
    }

    public partial class PygAnualFichaTecnica
    {
        public decimal PrimaTotal { get; set; }
        public decimal Asistencia { get; set; }
        public decimal SiniestrosIncurridos { get; set; }
        public decimal Siniestralidad { get; set; }
        public decimal PorcentajeSinietralidad { get; set; }
        public decimal ComisionIntermediario { get; set; }
        public decimal IvaComisionIntermediario { get; set; }
        public decimal GastosRetorno { get; set; }
        public decimal IvaGastosRetorno { get; set; }
        public decimal OtrosGastos { get; set; }
        public decimal GastosCompania { get; set; }
        public decimal Utilidad { get; set; }
        public decimal PorcentajeUtilidadAnno { get; set; }
    }
}
