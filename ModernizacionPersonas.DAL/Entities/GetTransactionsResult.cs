using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.DAL.Entities
{
    public class GetTransactionsResult : DbActionResponse
    {
        public List<CotizacionTransaction> Transactions { get; set; }
    }
}
