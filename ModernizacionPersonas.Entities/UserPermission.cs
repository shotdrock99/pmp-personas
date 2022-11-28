using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class UserPermission
    {
        public PermissionAction Action { get; set; }
        public string ActionName { get; set; }
    }

    public enum PermissionAction
    {
        ADMIN = 1,
        COTIZAR,
        AUTORIZAR,
        ENVIAR_SLIP,
        VERSIONAR,
        DOCUMENTAR,
        CONSULTAR,
        RECUPERAR,
        INTERMEDIARIO
    }
}
