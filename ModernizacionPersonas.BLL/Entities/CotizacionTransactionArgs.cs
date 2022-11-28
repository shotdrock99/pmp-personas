using ModernizacionPersonas.Entities;
using System.Collections.Generic;

namespace ModernizacionPersonas.BLL.Entities
{
    public class CotizacionTransactionArgs
    {
        public string UserName { get; set; }
        public int CodigoCotizacion { get; set; }
        public int Version { get; set; }
        public int TransactionId { get; set; }
        public int AuthorizationsCount { get; set; }
        public IEnumerable<TransactionComment> Comments { get; set; }
        public string Description { get; internal set; }
        public string UNotificado { get; set; }
    }
}
