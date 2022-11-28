using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class OpcionValorAsegurado
    {
        public int CodigoOpcionValorAsegurado { get; set; }
        public int CodigoAmparoGrupoAsegurado { get; set; }
        public decimal PorcentajeCobertura { get; set; }
        public decimal NumeroSalarios { get; set; }
        public decimal TasaRiesgo { get; set; }
        public decimal TasaComercial { get; set; }
        public decimal TasaRiesgoSiniestralidad { get; set; }
        public decimal ValorAsegurado { get; set; }
        public decimal Prima { get; set; }
        public decimal PrimaSinAplicar { get; set; }
        public decimal ValorDiario { get; set; }
        public decimal NumeroDias { get; set; }
        public decimal ValorAseguradoDias { get; set; }
        public int IndiceOpcion { get; set; }
        public int NumeroAsegurados { get; set; }
    }
}
