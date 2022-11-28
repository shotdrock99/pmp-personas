using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.DAL.Entities
{
    public class CrearCotizacionArgs
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string CodigoSucursal { get; set; }
        public string CodigoRamo { get; set; }

        public string CodigoSubRamo { get; set; }
        public string CodigoZona { get; set; }        
    }
}
