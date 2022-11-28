using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class Bloqueo
    {
        public int CodigoCotizacion { get; set; }
        public string CodigoUsuario { get; set; }
    }

    public class ParametrizacionApp
    {
        public int CodigoVariable { get; set; }
        public string NombreVariable { get; set; }
        public string ValorVariable { get; set; }
        public string TipoValorVariable { get; set; }
    }
}
