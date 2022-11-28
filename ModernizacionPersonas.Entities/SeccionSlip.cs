using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class SeccionSlip
    {
        public int Codigo { get; set; }
        public string Seccion { get; set; }
        public int Grupo { get; set; }
        public int Especial { get; set; }
        public int Activo { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string Movimiento { get; set; }
    }
}
