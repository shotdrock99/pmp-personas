using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.DAL.Entities
{
    public class DbActionResponse
    {
        public int CodigoCotizacion { get; set; }
        public int Version { get; set; }
        public int CodigoEstadoCotizacion { get; set; }
        public string NumeroCotizacion { get; set; }
    }
}
