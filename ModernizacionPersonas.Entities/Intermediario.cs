using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class Intermediario
    {
        public int CodigoCotizacion { get; set; }
        public int Codigo { get; set; }
        public int? Clave { get; set; }
        public decimal Participacion { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Email { get; set; }
        public int CodigoTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public int CodigoEstado { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public int CodigoDepartamento { get; set; }
        public int CodigoMunicipio { get; set; }
        public string  IntermediarioSlip { get; set; }
    }
}
