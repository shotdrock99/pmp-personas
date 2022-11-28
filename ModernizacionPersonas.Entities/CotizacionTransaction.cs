using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ModernizacionPersonas.Entities
{
    public class CotizacionTransaction
    {
        public int CodigoCotizacion { get; set; }
        public int Version { get; set; }
        public int CodigoTransaccion { get; set; }
        [JsonProperty("estado")]
        public int CodigoEstadoCotizacion { get; set; }
        public string CodigoUsuario { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        [JsonProperty("conteoControles")]
        public int ConteoAutorizaciones { get; set; }
        public string Initials { get; set; }
        public int CodigoRol { get; set; }
        public string NombreRol { get; set; }
        public IEnumerable<TransactionComment> Comments { get; set; }
        public IEnumerable<TransactionAttachment> Attachments { get; set; }
        public string UNotificado { get; set; }
    }

    public class TransactionAttachment
    {
        public int TransactionId { get; set; }
        public string Name { get; set; }
        [JsonProperty("type")]
        public string MimeType { get; set; }
        public string Uri { get; set; }
    }
}
