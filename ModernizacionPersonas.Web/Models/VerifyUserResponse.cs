using ModernizacionPersonas.Common;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Web.Models
{
    public class VerifyUserResponse
    {
        public ApplicationUser User { get; set; }
        public string AuthorizationToken { get; set; }
    }

    public class VerifyUserByTokenResponse
    {
        public string UserName { get; set; }        
    }
}
