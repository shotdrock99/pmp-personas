import { ParametrizacionSlipWriterService } from 'src/app/services/parametrizacion-slip-writer.service';
import { ParametrizacionSlipReaderService } from 'src/app/services/parametrizacion-slip-reader.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { SeccionSlip } from 'src/app/models/seccion-slip';
import { MatDialogConfig, MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { PageToolbarConfig, PageToolbarItem } from 'src/app/models/page-toolbar-item';
import { PageToolbarBuilder } from 'src/app/shared/services/page-toolbar-builder';
import { SeccionSlipEditarComponent } from './components/seccion-slip-editar/seccion-slip-editar.component';
import { SeccionSlipCrearComponent } from './components/seccion-slip-crear/seccion-slip-crear.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-secciones-slip-parametricas',
  templateUrl: './secciones-slip-parametricas.component.html',
  styleUrls: ['./secciones-slip-parametricas.component.scss'],
})
export class SeccionesSlipParametricasComponent implements OnInit {
  isLoading = true;
  data: SeccionSlip[];
  dataSource: MatTableDataSource<SeccionSlip>;
  displayedColumns: string[] = ['seccion', 'grupo', 'especial', 'usuarioModifica', 'fechaModifica', 'opciones'];
  itemsCount: number;
  toolbarConfig: PageToolbarConfig;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(
    private dialog: MatDialog,
    private toolbarBuilder: PageToolbarBuilder,
    private seccionesSlipReader: ParametrizacionSlipReaderService,
    private seccionesSlipWriter: ParametrizacionSlipWriterService,
    private router: Router
  ) { }

  ngOnInit() {
    this.loadSeccionesSlip();
    this.initializeToolbar();
  }

  initializeToolbar() {
    const items: PageToolbarItem[] = [
      {
        name: 'home',
        icon_path: 'home',
        label: '',
        tooltip: 'Parametrización',
        isEnabled: true,
        fixed: true,
        onClick: () => {
          this.router.navigate(['/parametrizacion']);
        },
      },
      {
        name: 'refresh',
        icon_path: 'refresh',
        label: '',
        tooltip: 'Refrescar',
        isEnabled: true,
        fixed: true,
        onClick: () => {
          this.refresh();
        },
      },
      {
        name: 'add',
        icon_path: 'add',
        label: 'Nueva Sección',
        tooltip: 'Nueva Sección',
        isEnabled: true,
        fixed: true,
        onClick: () => {
          this.createSeccionSlip();
        },
      },
    ];
    this.toolbarConfig = this.toolbarBuilder.build(items);
  }

  refresh() {
    this.loadSeccionesSlip();
    this.toolbarConfig.reset();
  }

  private loadSeccionesSlip() {
    this.seccionesSlipReader.getSeccionesSlip().subscribe((response) => {
      this.data = response;
      this.dataSource = new MatTableDataSource(this.data);
      this.itemsCount = this.dataSource.data.length;
      this.isLoading = false;
    });
  }

  public editSeccionSlip(seccion: SeccionSlip) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.direction = 'ltr';
    dialogConfig.height = 'auto';
    dialogConfig.width = '400px';

    dialogConfig.data = {
      title: 'Editar Sección Slip',
      codigo: seccion.codigo,
      seccion: seccion.seccion,
      grupo: seccion.grupo,
      especial: seccion.especial,
    };

    const dialogref = this.dialog.open(
      SeccionSlipEditarComponent,
      dialogConfig
    );

    dialogref
      .afterClosed()
      .subscribe((data) =>
        this.seccionesSlipWriter
          .editSeccionSlip(data)
          .subscribe((response) => this.refresh())
      );
  }

  private createSeccionSlip() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.direction = 'ltr';
    dialogConfig.height = 'auto';
    dialogConfig.width = '400px';

    dialogConfig.data = {
      title: 'Nueva Sección Slip',
    };

    const dialogref = this.dialog.open(SeccionSlipCrearComponent, dialogConfig);

    dialogref
      .afterClosed()
      .subscribe((data) =>
        this.seccionesSlipWriter
          .createSeccionSlip(data)
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
