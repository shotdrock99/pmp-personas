using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public partial class TomadorCotizacion
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public bool EsIntermediario { get; set; }
    }

    public partial class DirectorCotizacion
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Cargo { get; set; }
        public int CodigoCargo { get; set; }
    }
}
