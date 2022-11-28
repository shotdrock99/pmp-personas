using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.DAL.Entities
{
    public class TasaAsegurado
    {
        public string NumeroDocumento { get; set; }
        public decimal ValorAsegurado { get; set; }
        public int CodigoAmparo { get; set; }
        public decimal Tasa { get; set; }
    }
}
