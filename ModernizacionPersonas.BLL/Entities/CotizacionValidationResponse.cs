using ModernizacionPersonas.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.BLL.Entities
{
    public class CotizacionValidationResponse : ActionResponseBase
    {
        public bool IsValid { get; set; }
        public CotizacionValidation Validation { get; set; }
        public IEnumerable<CotizacionTasa> Tasas { get; set; }
        public bool RequireAuthorization { get; internal set; }
    }
}
