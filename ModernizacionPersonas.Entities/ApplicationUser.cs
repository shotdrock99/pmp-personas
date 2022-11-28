using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public  class ApplicationUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string DocumentId { get; set; }
        public ApplicationRole Rol { get; set; }
        public UserExternalInfo ExternalInfo { get; set; }
        public bool Activo { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string Movimiento { get; set; }
        public IEnumerable<string> Permissions { get; set; }        
        public bool CanApplyAction(PermissionAction action)
        {
            var actionName = Enum.GetName(typeof(PermissionAction), action);
            return this.Permissions.Contains(actionName);
        }        
    }

    public class ApplicationUserDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string Movimiento { get; set; }
    }

    public class ApplicationUserResponse
    {
        public ApplicationUser ApplicationUser { get; set; }
        public string Message { get; set; }

        public ApplicationUserResponse(ApplicationUser applicationUser, string message)
        {
            this.ApplicationUser = applicationUser;
            this.Message = message;
        }
    }
}
