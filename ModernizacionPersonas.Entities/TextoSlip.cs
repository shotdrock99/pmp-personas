using System;

namespace ModernizacionPersonas.Entities
{
    public class TextoSlip
    {
        public int Codigo { get; set; }
        public int CodigoRamo { get; set; }
        public int CodigoSubramo { get; set; }
        public int CodigoAmparo { get; set; }
        public int CodigoSeccion { get; set; }
        public int CodigoSector { get; set; }
        public string NombreSeccion { get; set; }
        public string Texto { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string Movimiento { get; set; }
    }
}

