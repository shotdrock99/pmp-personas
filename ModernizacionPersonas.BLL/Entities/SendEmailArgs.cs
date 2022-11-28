using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.BLL.Entities
{
    public class SendEmailArgs
    {
        public List<string> Attachments { get; set; }
        public string Subject { get; set; }
        public string[] CC { get; set; }
        public string[] CCO { get; set; }
        public string[] Recipients { get; set; }
        public string Body { get; set; }
    }
}
