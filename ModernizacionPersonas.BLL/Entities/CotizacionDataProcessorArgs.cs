using ModernizacionPersonas.Entities;
using PersonasServiceReference;
using System.Collections.Generic;

namespace ModernizacionPersonas.BLL.Entities
{
    public class CotizacionDataProcessorArgs
    {
        public int CodigoTipoTasa { get; set; }
        public TipoSumaAsegurada TipoSumaAsegurada { get; set; }
        public bool TieneSiniestralidad { get; set; }
        public decimal ValorSalarioMinimo { get; set; }
        public int CodigoCotizacion { get; internal set; }
        public decimal IBNR { get; internal set; }
        public decimal FactorG { get; internal set; }
        public bool ConListaAsegurados { get; internal set; }
        public IEnumerable<Asegurado> Asegurados { get; internal set; }
    }
}
