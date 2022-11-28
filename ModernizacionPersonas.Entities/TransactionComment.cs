using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class TransactionComment
    {
        public int TransactionId { get; set; }
        public string CodigoUsuario { get; set; }
        public string CodigoRolAutorizacion { get; set; }
        public int CodigoTipoAutorizacion { get; set; }
        public string Message { get; set; }
    }
}
