using Newtonsoft.Json;
using System.Collections.Generic;

namespace ModernizacionPersonas.Entities
{
    public class Resumen
    {
        public PersonasServiceReference.Tasa TipoTasa { get; set; }
        public bool TieneSiniestralidad { get; set; }
        public decimal Comision { get; set; }
        public decimal GRetorno { get; set; }
        [JsonProperty("otrosGastos")]
        public decimal PorcentajeOtrosGastos { get; set; }
        public decimal GastosCompania { get; set; }
        public decimal Utilidad { get; set; }
        public decimal FactorG { get; set; }
        public IEnumerable<GrupoAseguradoResumen> GruposAsegurados { get; set; }
        public decimal IvaComision { get; set; }
        [JsonProperty("ivagRetorno")]
        public decimal IvaGRetorno { get; set; }
        public decimal PorcentajeIvaComision { get; set; }
        public decimal PorcentajeIvaRetorno { get; set; }
    }

    public class ValorAseguradoOpcionResumen
    {
        public int IndiceOpcion { get; set; }
        public decimal ValorAseguradoTotal { get; set; }
        public decimal ValorAsegurado { get; set; }
        public PersonasServiceReference.Amparo Amparo { get; set; }
        public decimal TasaRiesgo { get; set; }
        public decimal TasaComercialAnual { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public decimal PorcentajeRecargo { get; set; }
        public decimal TasaComercialAplicar { get; set; }
        public decimal PrimaAnualIndividual { get; set; }
        public decimal PrimaAnualTotal { get; set; }
        public SiniestralidadResumen Siniestralidad { get; set; }
        public bool Configurado { get; set; }
        public decimal PorcentajeDescuentoSiniestralidad { get; set; }
        public decimal PorcentajeRecargoSiniestralidad { get; set; }
        public decimal TasaSiniestralidadAplicar { get; set; }
        public int NumeroAsegurados { get; set; }
    }

    public class SiniestralidadResumen
    {
        public PersonasServiceReference.Amparo Amparo { get; set; }
        public decimal TasaRiesgo { get; set; }
        public decimal TasaComercial { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public decimal PorcentajeRecargo { get; set; }
        public decimal TasaComercialAplicar { get; set; }
        // public decimal PrimaAnualIndividual { get; set; }
        public decimal PrimaAnualTotal { get; set; }
    }
}