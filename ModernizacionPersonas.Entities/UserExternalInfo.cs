using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class UserExternalInfo
    {
        public string LoginUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string EmailUsuario { get; set; }
        public int? Zona { get; set; }
        public string Descripcion { get; set; }
        public int? Sucursal { get; set; }
        public string NombreSucursal { get; set; }
        public int CodigoAreaDependencia { get; set; }
        public string NombreDependencia { get; set; }
        public int CodigoArea { get; set; }
        public string Area { get; set; }
        public int? CodigoCargo { get; set; }
        public string Cargo { get; set; }
        public int CodigoTipoDependencia { get; set; }
        public string TipoDependencia { get; set; }

        public UserExternalInfo()
        {
            this.EmailUsuario = "";
            this.Zona = null;
            this.Sucursal = null;
            this.CodigoCargo = null;
        }
    }
}
