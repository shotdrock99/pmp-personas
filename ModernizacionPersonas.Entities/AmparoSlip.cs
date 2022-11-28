using Newtonsoft.Json;
using System.Collections.Generic;

namespace ModernizacionPersonas.Entities
{
    public partial class AmparoSlip
    {
        public int CodigoRamo { get; set; }
        public int CodigoSubramo { get; set; }
        public int CodigoAmparo { get; set; }
        public string NombreAmparo { get; set; }
        public bool Activo { get; set; }
        public List<SlipVariable> Variables { get; set; }
        [JsonProperty("descripcion")]
        public string DescripcionAmparo { get; set; }
    }
}

