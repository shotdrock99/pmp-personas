using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.BLL.Entities
{
    public class SendEmailResponse 
    {        
        public int CodigoEstado { get; internal set; }
        public string DescripcionEstado { get; internal set; }
        public string RutaPDF { get; internal set; }
        public bool HasError { get; internal set; }
    }
}
