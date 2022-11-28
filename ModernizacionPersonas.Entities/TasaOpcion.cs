using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class TasaOpcion
    {
        public int CodigoGrupoAsegurado { get; set; }
        public int IndiceOpcion { get; set; }
        public decimal SumatoriaTasa { get; set; }
        public decimal TasaComercial { get; set; }
        public decimal TasaComercialTotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Recargo { get; set; }
        public decimal PrimaIndividual { get; set; }
        public decimal PrimaTotal { get; set; }
        public decimal TasaSiniestralidad { get; set; }
        public decimal DescuentoSiniestralidad { get; set; }
        public decimal RecargoSiniestralidad { get; set; }
        public decimal TasaSiniestralidadTotal { get; set; }
    }      
}
