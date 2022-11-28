import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { PageToolbarConfig, PageToolbarItem } from 'src/app/models/page-toolbar-item';
import { PageToolbarBuilder } from 'src/app/shared/services/page-toolbar-builder';
import { Router } from '@angular/router';
import { EmailEditarComponent } from './components/email-editar/email-editar.component';
import { EmailParametrizacionWriterService } from 'src/app/services/email-parametrizacion-writer.service';

@Component({
  selector: "app-email-parametricas",
  templateUrl: "./email-parametricas.component.html",
  styleUrls: ["./email-parametricas.component.scss"],
})
export class EmailParametricasComponent implements OnInit {
  isLoading: boolean = true;
  data: template[] = [
    {
      templateCode: 1,
      templateName: "Aprobación de Autorización",
      subject:
        "AUTORIZACION DE COTIZACION Nro. {NumeroCotizacion} TOMADOR – {NombreTomador} RAMO – {DescripcionRamo}",
    },
    {
      templateCode: 2,
      templateName: "Aprobación de Cotización",
      subject:
        "AUTORIZACIÓN DE COTIZACIÓN Nro. {NumeroCotizacion} TOMADOR - {NombreTomador} - RAMO {DescripcionRamo}",
    },
    {
      templateCode: 3,
      templateName: "Notificación de Autorización",
      subject:
        "SOLICITUD AUTORIZACION DE COTIZACION Nro. {NumeroCotizacion} TOMADOR – {NombreTomador} RAMO – {DescripcionRamo}",
    },
    {
      templateCode: 4,
      templateName: "Rechazo de Autorización",
      subject:
        "RECHAZO DE COTIZACION Nro. {NumeroCotizacion} TOMADOR – {NombreTomador} RAMO – {DescripcionRamo}",
    },
    {
      templateCode: 5,
      templateName: "Rechazo de Cotización",
      subject:
        "NO ACEPTACION DE COTIZACIÓN Nro. {NumeroCotizacion} TOMADOR - {NombreTomador} - RAMO {DescripcionRamo}",
    },
    {
      templateCode: 6,
      templateName: "Devuelto para revisión",
      subject:
        "AJUSTE DE COTIZACION Nro. {NumeroCotizacion} TOMADOR – {NombreTomador} RAMO – {DescripcionRamo}",
    },
    {
      templateCode: 7,
      templateName: "Envío Slip Intermedieario Comercial",
      subject:
        "PROPUESTA COTIZACION TOMADOR - {NombreTomador} RAMO – {NombreRamo}",
    },
    {
      templateCode: 8,
      templateName: "Envío Slip Tomador",
      subject:
        "PROPUESTA COTIZACION TOMADOR - {NombreTomador} RAMO – {NombreRamo}",
    },
    {
      templateCode: 9,
      templateName: "Solicitud Expedición Web",
      subject:
        "SOLICITUD EXPEDICION COTIZACION Nro. {NumeroCotizacion} - Versión. {Version} - TOMADOR - {NombreTomador} - RAMO – {NombreRamo}",
    },
  ];
  dataSource: MatTableDataSource<template>;
  displayedColumns: string[] = ['nombre', 'opciones'];
  itemsCount: number;
  toolbarConfig: PageToolbarConfig;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(private dialog: MatDialog,
    private toolbarBuilder: PageToolbarBuilder,
    private router: Router,
    private emailParametrizacionWriter: EmailParametrizacionWriterService) {}

  ngOnInit() {
    this.loadTemplates();
    this.initializeToolbar();
  }

  initializeToolbar() {
    const items: PageToolbarItem[] = [
      {
        name: "home",
        icon_path: "home",
        label: "",
        tooltip: "Parametrización",
        isEnabled: true,
        fixed: true,
        onClick: () => {
          this.router.navigate(['/parametrizacion']);
        },
      },
      {
        name: "refresh",
        icon_path: "refresh",
        label: "",
        tooltip: "Refrescar",
        isEnabled: true,
        fixed: true,
        onClick: () => {
          this.refresh();
        },
      },
    ];
    this.toolbarConfig = this.toolbarBuilder.build(items);
  }

  refresh(){
    this.loadTemplates();
    this.toolbarConfig.reset();
  }

  loadTemplates(){
    this.dataSource = new MatTableDataSource(this.data);
    this.itemsCount = this.dataSource.data.length;
    this.isLoading = false;
  }

  private editTemplate(template: template){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.direction = "ltr";
    dialogConfig.height = "auto";
    dialogConfig.width = "700px";

    dialogConfig.data = {
      title: template.templateName,
      codigoTemplate: template.templateCode,
      subject: template.subject
    };

    const dialogRef = this.dialog.open(EmailEditarComponent, dialogConfig);

    dialogRef
      .afterClosed()
      .subscribe((data) =>
        this.emailParametrizacionWriter
          .editTextoEmail(data)
          .subscribe((response) => this.refresh())
      );
  }

  applyFilter(filterValue: string) {
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }

    this.itemsCount = this.dataSource.data.length;
  }

}

interface template {
  templateCode: number;
  templateName: string;
  subject: string;
}
