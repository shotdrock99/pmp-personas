using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class AutorizacionListItem
    {
        public int CodigoCotizacion { get; set; }
        public string Agencia { get; set; }
        public string NumCotizacion { get; set; }
        public string Ramo { get; set; }
        public string Subramo { get; set; }
        public string Tomador { get; set; }
        public string UsuarioNotificado { get; set; }
        public string Rol { get; set; }
        public int Estado { get; set; }
    }
}
