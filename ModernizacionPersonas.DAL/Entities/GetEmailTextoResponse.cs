using ModernizacionPersonas.Common;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.DAL.Entities
{
    public class GetEmailTextoResponse : DbActionResponse
    {
        public IEnumerable<EmailParametrizacion> EmailTextos { get; set; }

        public GetEmailTextoResponse()
        {
            EmailTextos = new List<EmailParametrizacion>();
        }
    }
}
