using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class VariableValorSlip
    {
        public int CodigoSeccion { get; set; }
        public int CodigoAmparo { get; set; }
        public int CodigoRamo { get; set; }
        public int CodigoSubRamo { get; set; }
        public int CodigoVariable { get; set; }
        public int CodigoTipoSeccion { get; set; }
        public string NombreSeccion { get; set; }
        public string NombreVariable { get; set; }
        public string TipoDato { get; set; }
        public string ValorVariable { get; set; }
        public string ValorTope { get; set; }
        public string Activo { get; set; }
    }    
}
