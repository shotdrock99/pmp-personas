using ModernizacionPersonas.Entities;
using Newtonsoft.Json;

namespace ModernizacionPersonas.BLL.Entities
{
    public class CotizacionViewModel
    {
        public int CodigoCotizacion { get; set; }
        public int Version { get; set; }
        [JsonProperty("numero")]
        public string NumeroCotizacion { get; set; }
        [JsonProperty("estado")]
        public int CodigoEstadoCotizacion { get; set; }
        public string UsuarioNotificado { get; set; }
        public ApplicationUser User { get; set; }
        public int LastAuthorId { get; set; }
        public string LastAuthorName { get; set; }
        public InformacionBasicaViewModel InformacionBasica { get; set; }
        public InformacionBasicaTomadorViewModel InformacionBasicaTomador { get; set; }
        public InformacionNegocioViewModel InformacionNegocio { get; set; }
        public GastosCompaniaViewModel GastosCompania { get; set; }
        public InformacionIntermediariosViewModel InformacionIntermediarios { get; set; }
        public InformacionGruposAseguradosViewModel InformacionGruposAsegurados { get; set; }
        public InformacionSiniestralidadViewModel InformacionSiniestralidad { get; set; }
        public bool Blocked { get; set; }
    }
}
