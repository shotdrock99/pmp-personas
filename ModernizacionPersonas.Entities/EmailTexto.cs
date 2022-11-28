using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class EmailParametrizacion
    {
        public int CodigoParametrizacionEmail { get; set; }
        public int CodigoSeccion { get; set; }
        public string NombreSeccion { get; set; }
        public int CodigoTemplate { get; set; }
        public int CodigoRamo { get; set; }
        public int CodigoSubramo { get; set; }
        public int CodigoTomadorComercial { get; set; }
        public string Texto { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string Movimiento { get; set; }
    }
}
