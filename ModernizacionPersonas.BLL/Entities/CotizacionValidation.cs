using ModernizacionPersonas.DAL.Entities.SISEEntities;
using ModernizacionPersonas.Entities;
using System.Collections.Generic;

namespace ModernizacionPersonas.BLL.Entities
{
    public class CotizacionValidation
    {
        public AuthorizationValidationMessage ValidationMessage { get; set; }
        public IEnumerable<CotizacionAuthorization> Authorizations { get; set; }
        public IEnumerable<AuthorizationUser> Users { get; set; }
    }
}
