using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class VariableSlipParametrizacion
    {
        public int CodigoVariable { get; set; }
        public string NombreSeccion { get; set; }
        public string NombreVariable { get; set; }
        public string DescripcionVariable { get; set; }
        public string TipoDato { get; set; }
        public int ValorVariable { get; set; }
        public int ValorTope { get; set; }
        public int Activo { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string Movimiento { get; set; }
    }    
}
