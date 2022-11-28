using Newtonsoft.Json;

namespace ModernizacionPersonas.Entities
{
    public partial class TextosSeccionSlip
    {
        public string CodigoSeccion { get; set; }
        [JsonProperty("tituloSeccion")]
        public string Seccion { get; set; }
        public string CodigoAmparo { get; set; }
        public string Texto { get; set; }
        public bool Basico { get; set; }
        public bool Adicional { get; set; }
        public bool Especial { get; set; }
    }
}

