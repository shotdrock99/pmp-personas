using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class CotizacionItemList
    {
        public int CodigoCotizacion { get; set; }
        public int Version { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string NumeroCotizacion { get; set; }
        public int CodigoZona { get; set; }
        [JsonProperty("zona")]
        public string NombreZona { get; set; }
        public int CodigoSucursal { get; set; }
        [JsonProperty("sucursal")]
        public string NombreSucursal { get; set; }
        public int CodigoRamo { get; set; }
        [JsonProperty("ramo")]
        public string NombreRamo { get; set; }
        public int CodigoSubramo { get; set; }
        [JsonProperty("subramo")]
        public string NombreSubramo { get; set; }
        public int CodigoTomador { get; set; }
        [JsonProperty("tomador")]
        public string NombreTomador { get; set; }
        public string UsuarioNotificado { get; set; }
        public int CodigoEstado { get; set; }
        public bool Closed { get; set; }
        public string LastAuthor { get; set; }
        public bool Locked { get; set; }
        public int UsuarioRealNotifica { get; set; }
        public string UsuarioAutorizador { get; set; }
        public int BtnFichaAlterna { get; set; }
    }

    public class VersionCotizacion {
        public int CodigoCotizacion { get; set; }
        public int CodigoCotizacionPadre { get; set; }
        public int Version { get; set; }
        public int VersionPadre { get; set; }
        public int CodigoEstado { get; set; }
        public bool Closed { get; set; }
    }
}
