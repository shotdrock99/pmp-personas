using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class Causal
    {
        public int CodigoCausal { get; set; }
        public string CausalTexto { get; set; }
        public int Activo { get; set; }
        public int Externo { get; set; }
        public int Solidaria { get; set; }
        public int TipoCausal { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string Movimiento { get; set; }
    }
}
