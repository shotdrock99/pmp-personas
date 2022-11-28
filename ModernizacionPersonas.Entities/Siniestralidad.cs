using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class Siniestralidad
    {
        public int CodigoSiniestralidad { get; set; }
        public int CodigoCotizacion { get; set; }
        public int Anno { get; set; }
        public DateTime FechaInicial { get; set; }
        public DateTime FechaFinal { get; set; }
        public decimal ValorIncurrido { get; set; }
        public int NumeroCasos { get; set; }        
    }    
}
