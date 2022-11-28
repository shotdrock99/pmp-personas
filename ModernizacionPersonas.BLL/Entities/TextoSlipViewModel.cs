using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.BLL.Entities
{
    public class TextoSlipViewModel
    {
        public int Codigo { get; set; }
        public int CodigoRamo { get; set; }
        public string NombreRamo { get; set; }
        public int CodigoSubramo { get; set; }
        public string NombreSubramo { get; set; }
        public int CodigoSector { get; set; }
        public string NombreSector { get; set; }
        public int CodigoAmparo { get; set; }
        public string NombreAmparo { get; set; }
        public int CodigoSeccion { get; set; }
        public string NombreSeccion { get; set; }
        public string Texto { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string Movimiento { get; set; }
    }
}
