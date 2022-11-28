using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.BLL.Entities
{
    public class GenerarFichaTecnicaResponse : ActionResponseBase
    {
        public FichaTecnica Data { get; set; }
    }
}
