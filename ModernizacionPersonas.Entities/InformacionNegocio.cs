using Newtonsoft.Json;
using System;

namespace ModernizacionPersonas.Entities
{
    public class InformacionNegocio
    {
        public int CodigoCotizacion { get; set; }
        public int Version { get; set; }
        public string NumeroCotizacion { get; set; }
        public string NombreAseguradora { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int CodigoPeriodoFacturacion { get; set; }
        public int CodigoTipoRiesgo { get; set; }
        public int CodigoTipoNegocio { get; set; }
        public int CodigoTipoContratacion { get; set; }
        [JsonProperty("perfilEdad")]
        public int CodigoPerfilEdad { get; set; }
        [JsonProperty("perfilValor")]
        public int CodigoPerfilValor { get; set; }
        public int CodigoSector { get; set; }
        public decimal PorcentajeRetorno { get; set; }
        public decimal PorcentajeOtrosGastos { get; set; }
        public string OtrosGastos { get; set; }
        public decimal PorcentajeComision { get; set; }
        public bool EsNegocioDirecto { get; set; }
        public bool Bloqueado { get; set; }
        public string BloqueadoBy { get; set; }
        public bool ConListaAsegurados { get; set; }
        public decimal UtilidadCompania { get; set; }
        public decimal PorcentajeGastosCompania { get; set; }
        public decimal FactorG { get; set; }
        public decimal IBNR { get; set; }
        public int CodigoSucursal { get; set; }
        public int CodigoRamo { get; set; }
        public int CodigoSubramo { get; set; }
        public int CodigoZona { get; set; }
        public int CodigoTipoTasa1 { get; set; }
        public int CodigoTipoTasa2 { get; set; }
        [JsonProperty("estado")]
        public int CodigoEstadoCotizacion { get; set; }
        public string UsuarioDirectorComercial { get; set; }
        public string NombreDirectorComercial { get; set; }
        public string EmailDirectorComercial { get; set; }
        public int anyosSiniestralidad { get; set; }
        public int VersionCopia { get; set; }

        public CotizacionState CotizacionState
        {
            get
            {
                return (CotizacionState)this.CodigoEstadoCotizacion;
            }
        }
        public string UsuarioNotificado { get; set; }
        public int LastAuthorId { get; set; }
        public string LastAuthorName { get; set; }
        public bool CotizacionChanged { get; set; }
        public decimal PorcentajeIvaComision { get; set; }
        public decimal PorcentajeIvaRetorno { get; set; }
        public bool SelfAuthorize { get; set; }
        [JsonProperty("actividad")]
        public string Actividad { get; set; }

        public override int GetHashCode()
        {
            var hash = 0;
            var hash0 = CodigoTipoRiesgo.GetHashCode();
            var hash1 = CodigoTipoTasa1.GetHashCode();
            var hash2 = CodigoTipoTasa2.GetHashCode();
            hash = (hash * 397) ^ hash0 ^ hash1 ^ hash2;

            return hash;
        }

        public override bool Equals(object obj)
        {
            var model = obj as InformacionNegocio;
            if (model.PorcentajeRetorno == 0) return false;

            var isEqual = this.GetHashCode() == obj.GetHashCode();
            return false;
        }
    }
}
