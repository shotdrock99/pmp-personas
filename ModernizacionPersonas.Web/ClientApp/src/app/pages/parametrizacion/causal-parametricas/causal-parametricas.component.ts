import { PageToolbarBuilder } from './../../../shared/services/page-toolbar-builder';
import { Router } from '@angular/router';
import { PageToolbarItem, PageToolbarConfig } from 'src/app/models/page-toolbar-item';
import { CausalesWriterService } from './../../../services/causales-writer.services';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { CausalesReaderService } from './../../../services/causales-reader.service';
import { Component, OnInit } from '@angular/core';
import { Causal } from 'src/app/models/causal';
import { CausalEditComponent } from './components/causal-edit/causal-edit.component';
import { CausalCrearComponent } from './components/causal-crear/causal-crear.component';

@Component({
  selector: "app-causal-parametricas",
  templateUrl: "./causal-parametricas.component.html",
  styleUrls: ["./causal-parametricas.component.scss"],
})
export class CausalParametricasComponent implements OnInit {
  isLoading: boolean = true;
  data: Causal[];
  dataSource: MatTableDataSource<Causal>;
  displayedColumns: string[] = [
    "descripcionCausal",
    "activo",
    "externo",
    "solidaria",
    "tipoCausal",
    "usuarioModifica",
    "fechaModifica",
    "acciones",
  ];
  itemsCount: number;
  activoCheck: false;
  toolbarConfig: PageToolbarConfig;

  constructor(
    private causalesReaderService: CausalesReaderService,
    private causalesWriterService: CausalesWriterService,
    private dialog: MatDialog,
    private router: Router,
    private toolbarBuilder: PageToolbarBuilder
  ) {}

  ngOnInit() {
    this.loadCausales();
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
        label: "Nueva causal",
        tooltip: "Nueva causal",
        isEnabled: true,
        fixed: true,
        onClick: () => {
          this.createCausal();
        },
      },
    ];

    this.toolbarConfig = this.toolbarBuilder.build(items);
  }

  private loadCausales() {
    this.causalesReaderService.getCausales().subscribe((response) => {
      this.data = response;
      this.dataSource = new MatTableDataSource(this.data);
      this.itemsCount = this.dataSource.data.length;
      this.isLoading = false;
    });
  }

  refresh() {
    this.loadCausales();
    this.toolbarConfig.reset();
  }

  public disableCausal(causal: Causal) {
    this.causalesWriterService
      .disableCausal(causal.codigoCausal)
      .subscribe((response) => this.refresh());
  }

  public editCausal(causal: Causal) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.direction = "ltr";
    dialogConfig.height = "auto";
    dialogConfig.width = "400px";

    dialogConfig.data = {
      id: causal.codigoCausal,
      title: "Editar Causal",
      causalTexto: causal.causalTexto,
      activo: causal.activo,
      externo: causal.externo,
      solidaria: causal.solidaria,
      tipoCausal: causal.tipoCausal,
    };

    const dialogRef = this.dialog.open(CausalEditComponent, dialogConfig);

    dialogRef
      .afterClosed()
      .subscribe((data) =>
        this.causalesWriterService
          .updateCausal(data)
          .subscribe((response) => this.refresh())
      );
  }

  public createCausal() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.direction = "ltr";
    dialogConfig.height = "auto";
    dialogConfig.width = "400px";

    dialogConfig.data = {
      title: "Crear Causal",
    };

    const dialogRef = this.dialog.open(CausalCrearComponent, dialogConfig);

    dialogRef
      .afterClosed()
      .subscribe((data) =>
        this.causalesWriterService
          .createCausal(data)
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
