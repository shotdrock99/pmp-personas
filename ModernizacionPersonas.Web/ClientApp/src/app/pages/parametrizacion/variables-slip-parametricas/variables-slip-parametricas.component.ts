import { Component, OnInit, ViewChild } from '@angular/core';
import { VariableSlip } from 'src/app/models/variable-slip';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { PageToolbarConfig, PageToolbarItem } from 'src/app/models/page-toolbar-item';
import { PageToolbarBuilder } from 'src/app/shared/services/page-toolbar-builder';
import { VariablesSlipReaderService } from 'src/app/services/variables-slip-reader.service';
import { VariableSlipEditarComponent } from './components/variable-slip-editar/variable-slip-editar.component';
import { VariablesSlipWriterService } from 'src/app/services/variables-slip-writer.service';
import { VariableSlipCrearComponent } from './components/variable-slip-crear/variable-slip-crear.component';
import { Router } from '@angular/router';

@Component({
  selector: "app-variables-slip-parametricas",
  templateUrl: "./variables-slip-parametricas.component.html",
  styleUrls: ["./variables-slip-parametricas.component.scss"],
})
export class VariablesSlipParametricasComponent implements OnInit {
  isLoading: boolean = true;
  data: VariableSlip[];
  dataSource: MatTableDataSource<VariableSlip>;
  displayedColumns: string[] = [
    "nombre",
    "descripcion",
    "tipo",
    "valor",
    "valorTope",
    "usuarioModifica",
    "fechaModifica",
    "opciones",
  ];
  itemsCount: number;
  toolbarConfig: PageToolbarConfig;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(
    private dialog: MatDialog,
    private toolbarBuilder: PageToolbarBuilder,
    private variablesSlipReader: VariablesSlipReaderService,
    private variablesSlipWriter: VariablesSlipWriterService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadVariablesSlip();
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
          this.router.navigate(["/parametrizacion"]);
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
      {
        name: "add",
        icon_path: "add",
        label: "Nueva Variable",
        tooltip: "Nueva Variable",
        isEnabled: true,
        fixed: true,
        onClick: () => {
          this.createVariableSlip();
        },
      },
    ];
    this.toolbarConfig = this.toolbarBuilder.build(items);
  }

  refresh() {
    this.loadVariablesSlip();
    this.toolbarConfig.reset();
  }

  private loadVariablesSlip() {
    this.variablesSlipReader.getVariablesSlip().subscribe((response) => {
      this.data = response;
      this.dataSource = new MatTableDataSource(this.data);
      this.itemsCount = this.dataSource.data.length;
      this.isLoading = false;
    });
  }

  public editVariableSlip(variable: VariableSlip) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.direction = "ltr";
    dialogConfig.height = "auto";
    dialogConfig.width = "600px";

    dialogConfig.data = {
      title: "Editar Variable Slip",
      codigoVariable: variable.codigoVariable,
      nombreVariable: variable.nombreVariable,
      descripcionVariable: variable.descripcionVariable,
      tipoDato: variable.tipoDato,
      valorVariable: variable.valorVariable,
      valorTope: variable.valorTope,
    };

    const dialogRef = this.dialog.open(
      VariableSlipEditarComponent,
      dialogConfig
    );

    dialogRef
      .afterClosed()
      .subscribe((data) =>
        this.variablesSlipWriter
          .editVariableSlip(data)
          .subscribe((response) => this.refresh())
      );
  }

  private createVariableSlip() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.direction = "ltr";
    dialogConfig.height = "auto";
    dialogConfig.width = "600px";

    dialogConfig.data = {
      title: "Crear Variable Slip",
    };

    const dialogRef = this.dialog.open(
      VariableSlipCrearComponent,
      dialogConfig
    );

    dialogRef
      .afterClosed()
      .subscribe((data) =>
        this.variablesSlipWriter
          .createVariableSlip(data)
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
