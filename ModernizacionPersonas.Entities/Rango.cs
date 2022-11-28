using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class Rango
    {
        public int CodigoRangoGrupoAsegurado { get; set; }
        public int CodigoGrupoAsegurado { get; set; }        
        public int EdadMinAsegurado { get; set; }
        public int EdadMaxAsegurado { get; set; }
        public int EdadMaxRango { get; set; }
        public int NumeroAsegurados { get; set; }
        public decimal ValorAsegurado { get; set; }     
        public decimal TasaRiesgo { get; set; }
        public decimal TasaComercial { get; set; }
        public decimal ValorPrimaBasico { get; set; }
    }
}
