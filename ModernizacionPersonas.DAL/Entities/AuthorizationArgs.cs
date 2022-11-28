using ModernizacionPersonas.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ModernizacionPersonas.DAL.Entities
{
    public class AuthorizationArgs
    {
        public int TransactionId { get; set; }
        [JsonProperty("action")]
        public AuthorizationAction AutorizacionAction { get; set; }

        public string CodigoUsuarioAutorizador { get; set; }

        public AuthorizationResult AuthorizationResult { get; set; }
        public string UserName { get; set; }
        public string NotifyUser { get; set; }
    }

    public class ChangesArgs
    {
        public int CodigoCotizacion { get; set; }
        public int Version { get; set; }
        public decimal GastosCompania { get; set; }
        public decimal UtilidadesCompania { get; set; }
    }

    public class AuthorizationResult
    {
        public IEnumerable<TransactionComment> Comments { get; set; }

        public decimal GastosCompania { get; set; }

        public decimal UtilidadesCompania { get; set; }

        public AuthorizationTasa Tasa { get; set; }
    }

    public enum AuthorizationAction
    {
        Accept = 1,
        Reject,
        Modify
    }

    public class NotifyCotizacionArgs
    {
        public int TransactionId { get; set; }
        public IEnumerable<CotizacionAuthorization> AuthorizationControls { get; set; }
        public AuthorizationUser AuthorizationUser { get; set; }
        public IEnumerable<TransactionComment> Comments { get; set; }
        public string UserName { get; set; }
    }

    public class GetAuthorizationControlsResponse : DbActionResponse
    {
        public IEnumerable<CotizacionAuthorization> Authorizations { get; set; }
    }
}
