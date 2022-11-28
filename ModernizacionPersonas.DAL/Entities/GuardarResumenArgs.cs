using ModernizacionPersonas.Entities;
using System.Collections.Generic;

namespace ModernizacionPersonas.DAL.Entities
{
    public class GuardarResumenArgs
    {   
        public decimal PorcentajeRetorno { get; set; }
        public decimal PorcentajeOtrosGastos { get; set; }
        public decimal PorcentajeComision { get; set; }
        public decimal UtilidadCompania { get; set; }
        public decimal GastosCompania { get; set; }
        public decimal FactorG { get; set; }

        public IEnumerable<TasaOpcion> TasaOpciones { get; set; }

        public GuardarResumenArgs()
        {
            TasaOpciones = new List<TasaOpcion>();
        }
    }

    public class GuardarResumenArgs1
    {
        public decimal PorcentajeComision { get; set; }
        public decimal PorcentajeIvaComision { get; set; }
        public decimal PorcentajeRetorno { get; set; }
        public decimal PorcentajeIvaRetorno { get; set; }
        public decimal PorcentajeOtrosGastos { get; set; }
        public decimal GastosCompania { get; set; }
        public decimal UtilidadCompania { get; set; }
        public decimal FactorG { get; set; }
        public IEnumerable<TasaOpcion> TasaOpciones { get; set; }
        public GuardarResumenArgs1()
        {
            TasaOpciones = new List<TasaOpcion>();
        }
    }
}
