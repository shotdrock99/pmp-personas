using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class Rol
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string Movimiento { get; set; }
        public IEnumerable<Permiso> Permisos { get; set; }

        public Rol()
        {
            this.Permisos = new List<Permiso>();
        }
    }
    public class RolSISE
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; }

    }
}
