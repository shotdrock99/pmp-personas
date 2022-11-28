using Newtonsoft.Json;
using System;

namespace ModernizacionPersonas.BLL.Entities
{
    public class InformacionNegocioViewModel
    {
        public string NombreAseguradora { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int CodigoPeriodoFacturacion { get; set; }
        public int CodigoTipoRiesgo { get; set; }
        public int CodigoTipoNegocio { get; set; }
        public int CodigoTipoContratacion { get; set; }
        public int Sector { get; set; }
        [JsonProperty("tipoTasa1")]
        public int CodigoTipoTasa1 { get; set; }
        [JsonProperty("tipoTasa2")]
        public int CodigoTipoTasa2 { get; set; }
        public decimal PorcentajeRetorno { get; set; }
        public decimal PorcentajeOtrosGastos { get; set; }
        public string OtrosGastos { get; set; }
        public decimal GastosCompania { get; set; }
        public decimal UtilidadesCompania { get; set; }
        public decimal PorcentajeComision { get; set; }
        public bool EsNegocioDirecto { get; set; }
        public bool ConListaAsegurados { get; set; }
        [JsonProperty("perfilEdad")]
        public int CodigoPerfilEdad { get; set; }
        [JsonProperty("perfilValor")]
        public int CodigoPerfilValor { get; set; }
        public int LastAuthorId { get; set; } 
        // Ultimo usuario modificador
        public string LastAuthorName { get; set; }
        public string UsuarioDirectorComercial { get; set; }
        public string NombreDirectorComercial { get; set; }
        public string EmailDirectorComercial { get; set; }
        public string Actividad { get; set; }
        public int AnyosSiniestralidad { get; set; }
    }
}
