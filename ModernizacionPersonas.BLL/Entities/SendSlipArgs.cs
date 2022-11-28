using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Entities
{
    public class SendSlipArgs
    {
        public int CodigoCotizacion { get; set; }
        public int? Version { get; set; }
        public string NumeroCotizacion { get; set; }
        public string[] Recipients { get; set; }
        public string Comments { get; set; }
        public string[] WithCopy { get; set; }
        public bool Resend { get; set; }
    }
}
