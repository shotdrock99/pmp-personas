import { Component, OnInit } from '@angular/core';

@Component({
  selector: "app-parametrizacion",
  templateUrl: "./parametrizacion.component.html",
  styleUrls: ["./parametrizacion.component.scss"],
})
export class ParametrizacionComponent implements OnInit {

  cards = [
    {
      icon: "people_alt",
      title: "Usuarios",
      related: "Roles",
      description:
        "Esta parametrización permite consultar, editar, deshablitar y crear usuarios teniendo en cuenta la relación que tienen con los roles del negocio y políticas internas de creación.",
      route: "usuarios",
    },
    {
      icon: "how_to_reg",
      title: "Roles",
      related: "Permisos",
      description:
        "Esta parametrización permite consultar, editar, deshabilitar y crear roles teniendo en cuenta su relación con los permisos que puede o no ejecutar este sobre la aplicación.",
      route: "roles",
    },
    {
      icon: "error_outline",
      title: "Causales",
      related: "",
      description:
        "Esta parametrización permite consultar, editar y crear causales teniendo en cuenta sus tipos y sus relaciones externas o internas.",
      route: "causales",
    },
    {
      icon: "label",
      title: "Secciones Slip",
      related: "Textos - Variables (Slip)",
      description:
        "Esta parametrización permite consultar, editar y crear secciones para un Slip teniendo en cuenta al grupo al que puede pertenecer.",
      route: ['slip', 'secciones'],
    },
    {
      icon: "extension",
      title: "Variables Slip",
      related: "Secciones - Textos (Slip)",
      description:
        "Esta parametrización permite consultar, editar y crearvariables para un Slip teniendo en cuenta el tipo sobre  el cual es usada.",
      route: ['slip', 'variables'],
    },
    {
      icon: "notes",
      title: "Textos Slip",
      related: "Secciones - Variables (Slip)",
      description:
        "Esta parametrización permite consultar, editar y crear textos para un Slip teniendo en cuenta secciones y variables que lo complementan.",
      route: ['slip', 'textos'],
    },
    {
      icon: "all_inbox",
      title: "Emails",
      related: "",
      description:
        "Esta parametrización permite consultar y editar los cuerpos de los correos de las plantillas correspondientes a cada operación de notificación vía email.",
      route: "emails",
    },
    {
      icon: "public",
      title: "Variables Globales",
      related: "",
      description:
        "Esta parametrización permite consultar y editar todas aquellas variables globales que afectan el comportamiento general del app.",
      route: "variablesGlobales",
    }
  ];

  constructor() { }

  ngOnInit() { }
}
