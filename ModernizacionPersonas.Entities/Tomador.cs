using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class Tomador
    {
        public int CodigoCotizacion { get; set; }
        public int CodigoTomador { get; set; }
        public int CodigoTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Nombres { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string Direccion { get; set; }
        public int CodigoPais { get; set; }
        public int CodigoDepartamento { get; set; }
        public int CodigoMunicipio { get; set; }
        public string Email { get; set; }
        public string NombreContacto { get; set; }
        public string Telefono1Contacto { get; set; }
        public string Telefono2Contacto { get; set; }
        public int CodigoActividad { get; set; }
        public string Actividad { get; set; }
        public bool Licitacion { get; set; }
        public string AseguradoraActual { get; set; }
        public string DescripcionTipoRiesgo { get; set; }
        public string TomadorSlip { get; set; }

        public override int GetHashCode()
        {
            var hash = 0;
            hash = (hash * 397) ^ NumeroDocumento.GetHashCode();

            return hash;
        }

        public override bool Equals(object obj)
        {
            Tomador tomador = obj as Tomador;
            if (tomador.CodigoTomador == 0) return false;

            var isEqual = this.GetHashCode() == obj.GetHashCode();
            return false;
        }
    }
}
