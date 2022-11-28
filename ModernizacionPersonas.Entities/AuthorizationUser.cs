using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class AuthorizationUser
    {
        public int CodigoCotizacion { get; set; }
        public int VersionCotizacion { get; set; }
        public string Codigo { get; set; }
        public int CodigoRol { get; set; }
        public string NombreRol { get; set; }
        public int CodigoNivel { get; set; }
        public int CodigoTipoAutorizacion { get; set; }
        public bool Activo { get; set; }
        public bool Notificado { get; set; }
        public bool Delegacion { get; set; }
        public bool Especial { get; set; }
    }
}
