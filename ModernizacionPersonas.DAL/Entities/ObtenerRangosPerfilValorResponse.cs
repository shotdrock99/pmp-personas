using PersonasServiceReference;
using System.Collections.Generic;

namespace ModernizacionPersonas.DAL.Entities
{
    public class ObtenerRangosPerfilValorResponse
    {
        public IEnumerable<RangoValor> Rangos { get; set; }
        public decimal ValorMinimo { get; set; }
        public decimal ValorMaximo { get; set; }
    }
}
