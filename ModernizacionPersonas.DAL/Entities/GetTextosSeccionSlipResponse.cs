using ModernizacionPersonas.Entities;
using Newtonsoft.Json;

namespace ModernizacionPersonas.DAL.Entities
{
    public partial class GetTextosSeccionSlipResponse
    {
        public InfoGeneralTextosSeccionSlip InfoGeneral { get; set; }
        public AmparosTextosSeccionSlip Amparos { get; set; }
        public EdadesSlipSection Edades { get; set; }
        public ClausulasTextosSeccionSlip Clausulas { get; set; }
        public string Actividad { get; set; }
        public CondicionesTextosSeccionSlip Condiciones { get; set; }
        public DisposicionesTextosSeccionSlip Disposiciones { get; set; }
        [JsonProperty("diasValidez")]
        public string Validez { get; set; }

    }
}
