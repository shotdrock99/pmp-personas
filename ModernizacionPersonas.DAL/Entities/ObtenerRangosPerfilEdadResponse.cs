using PersonasServiceReference;
using System.Collections.Generic;

namespace ModernizacionPersonas.DAL.Entities
{
    public class ObtenerRangosPerfilEdadResponse
    {
        public IEnumerable<RangoEdad> Rangos { get; set; }
        public int ValorMinimo { get; set; }
        public int ValorMaximo { get; set; }
    }
}
