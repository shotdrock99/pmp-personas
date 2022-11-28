using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.DAL.Entities
{
    public class CotizacionFilter
    {
        public string CodigoSucursal { get; set; }
        public int? CodigoRamo { get; set; }
        public int? CodigoSubramo { get; set; }
        public int? CodigoZona { get; set; }
        public int? CodigoCotizacion { get; set; }
        public string NumeroCotizacion { get; set; }
        public string CodigoUsuario { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int? CodigoEstado { get; set; }
        public int? CodigoTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
    }
}
