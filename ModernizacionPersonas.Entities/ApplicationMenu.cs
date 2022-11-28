using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ModernizacionPersonas.Entities
{
    public class ApplicationMenu
    {
        public List<ApplicationMenuItem> Items { get; set; }

        public ApplicationMenu()
        {
            this.Items = new List<ApplicationMenuItem>();
        }

        public static ApplicationMenu BuildMenu(IEnumerable<UserPermission> permissions)
        {
            var menu = new ApplicationMenu();
            // Add Cotizaciones item for all roles
            var cotizacionesItem = ApplicationMenuItem.Create("cotizaciones", "Cotizaciones", 1, icon: "view_list");
            foreach (var permission in permissions)
            {
                if (permission.Action == PermissionAction.COTIZAR)
                {
                    var item1 = ApplicationMenuItem.Create("newcotizacion", "Nueva Cotización", icon: "add", routerLink: "/cotizaciones/nueva");
                    cotizacionesItem.Items.Add(item1);
                    var item2 = ApplicationMenuItem.Create("listcotizaciones", "Lista de cotizaciones", icon: "view_list", routerLink: "/cotizaciones");
                    cotizacionesItem.Items.Add(item2);
                    cotizacionesItem.ItemsCount = cotizacionesItem.Items.Count();
                }

                if (permission.Action == PermissionAction.AUTORIZAR)
                {
                    menu.Items.Add(ApplicationMenuItem.Create("autorizaciones", "Autorizaciones", 2, icon: "security", routerLink: "/autorizaciones"));
                }

                if (permission.Action == PermissionAction.ADMIN)
                {
                    menu.Items.Add(ApplicationMenuItem.Create("admin", "Administración", 3, icon: "settings", routerLink: "/parametrizacion"));
                }
            }

            if (cotizacionesItem.Items.Count() == 0)
            {
                cotizacionesItem.RouterLink = "/cotizaciones";
            }

            menu.Items.Add(cotizacionesItem);
            menu.Items = menu.Items.OrderBy(x => x.Index).ToList();
            return menu;
        }

        public static ApplicationMenu BuildViewMenu(IEnumerable<UserPermission> permissions)
        {
            var menu = new ApplicationMenu();
            return menu;
        }
    }

    public class ApplicationMenuItem
    {
        public int Index { get; set; }
        public string ParentName { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Text { get; set; }
        public string RouterLink { get; set; }
        public int ItemsCount { get; set; }
        public List<ApplicationMenuItem> Items { get; set; }

        public static ApplicationMenuItem Create(string name, string text, int index = 0, string parentName = "", string icon = "", string routerLink = "")
        {
            return new ApplicationMenuItem
            {
                Index = index,
                Name = name,
                ParentName = parentName,
                Icon = icon,
                Text = text,
                RouterLink = routerLink,
                Items = new List<ApplicationMenuItem>(),
                ItemsCount = 0
            };
        }

        public static ApplicationMenuItem Create(PermissionAction action)
        {
            switch (action)
            {
                case PermissionAction.COTIZAR:
                    return Create("newcotizacion", "Nueva Cotización", parentName: "cotizaciones", icon: "add", routerLink: "/cotizaciones/nueva");
                case PermissionAction.ENVIAR_SLIP:
                    return Create("sendslip", "Enviar Slip", parentName: "more");
                case PermissionAction.VERSIONAR:
                    return Create("newversion", "Nueva versión", parentName: "more");
                //case PermissionAction.DOCUMENTAR:
                //    return Create("document", "Documentar");
                case PermissionAction.CONSULTAR:
                    return Create("viewcotizacion", "Ver cotización", parentName: "cotizaciones");
                case PermissionAction.RECUPERAR:
                    return Create("requestcotizacion", "Recuperar cotización", parentName: "more");
                default:
                    return null;
            }
        }
    }
}