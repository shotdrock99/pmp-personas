using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.DAL.Entities.SISEEntities
{
    public class AuthorizationValidationMessage
    {
        public int CodigoGrupoAsegurado { get; set; }
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }
}
