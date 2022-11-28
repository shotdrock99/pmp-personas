using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.BLL.Entities
{
    public class GetAuthorizationInfoResponse : ActionResponseBase
    {
        public string LastModifyUser { get; set; }
        public string LastRoleModifyUser { get; set; }
        public IEnumerable<CotizacionAuthorizationDTO> Authorizations { get; set; }

        public IEnumerable<AuthorizationUser> Users { get; set; }
    }
}
