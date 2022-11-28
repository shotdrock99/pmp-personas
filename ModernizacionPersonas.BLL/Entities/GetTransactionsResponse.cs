using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.BLL.Entities
{
    public class GetTransactionsResponse : ActionResponseBase
    {
        public IEnumerable<CotizacionTransaction> Transactions { get; set; }
    }
}
