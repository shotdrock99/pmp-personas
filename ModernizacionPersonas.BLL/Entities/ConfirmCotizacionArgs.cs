using ModernizacionPersonas.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.BLL.Entities
{
    public class ConfirmCotizacionArgs
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int Version { get; set; }
        public int TransactionId { get; set; }
        public int CausalId { get; set; }
        public string Observaciones { get; set; }
        public ConfirmCotizacionAction Action { get; set; }
        public string To { get; set; }
        public string[] WithCopy { get; set; }
    }
}
