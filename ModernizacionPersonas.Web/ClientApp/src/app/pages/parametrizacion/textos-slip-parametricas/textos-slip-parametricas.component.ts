import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { PageToolbarConfig, PageToolbarItem } from 'src/app/models/page-toolbar-item';
import { ParametrizacionSlipReaderService } from 'src/app/services/parametrizacion-slip-reader.service';
import { ParametrizacionSlipWriterService } from 'src/app/services/parametrizacion-slip-writer.service';
import { PageToolbarBuilder } from 'src/app/shared/services/page-toolbar-builder';
import { TextoSlip } from './../../../models/texto-slip';
import { CreateTextoSlipComponent } from './components/create-texto-slip/create-texto-slip.component';
import { TextoSlipEditorComponent } from './components/texto-slip-editor/texto-slip-editor.component';
import { FormBuilder, AbstractControl } from '@angular/forms';

@Component({
  selector: 'app-textos-slip-parametricas',
  templateUrl: './textos-slip-parametricas.component.html',
  styleUrls: ['./textos-slip-parametricas.component.scss'],
})
export class TextosSlipParametricasComponent implements OnInit {
  data: TextoSlip[];
  dataSource: MatTableDataSource<TextoSlip>;
  displayedColumns: string[] = [
    'nombreRamo',
    'subramo',
    'sector',
    'amparo',
    'seccion',
    'usuarioModifica',
    'fechaModifica',
    'opciones',
  ];
  itemsCount: number;
  toolbarConfig: PageToolbarConfig;
  pageLoaded = false;
  processing = true;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  readonly formControl: AbstractControl;

  constructor(
    private router: Router,
    private dialog: MatDialog,
    private toolbarBuilder: PageToolbarBuilder,
    private parametrizacionSlipReader: ParametrizacionSlipReaderService,
    private parametrizacionSlipWriter: ParametrizacionSlipWriterService,
    private formBuilder: FormBuilder
  ) {
    this.formControl = formBuilder.group({
      ramo: '',
      subramo: '',
      sector: '',
      amparo: '',
      seccion: ''
    });
  }

  ngOnInit() {
    this.loadTextosSlip();
    this.initializeToolbar();
  }

  initializeToolbar() {
    const items: PageToolbarItem[] = [
      {
        name: 'back',
        icon_path: 'home',
        label: '',
        tooltip: 'Atras',
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
        label: 'Nuevo Texto',
        tooltip: 'Nuevo Texto',
        isEnabled: true,
        fixed: true,
        onClick: () => {
          this.createTextoSlip();
        },
      },
    ];
    this.toolbarConfig = this.toolbarBuilder.build(items);
  }

  refresh() {
    this.pageLoaded = false;
    this.processing = true;

    this.loadTextosSlip();
    this.toolbarConfig.reset();
  }

  private loadTextosSlip() {
    this.parametrizacionSlipReader.getTextosSlip().subscribe((response) => {
      this.data = response;
      this.data.forEach(element => {
        if (element.codigoAmparo == "0") {
          element.nombreAmparo = "NO APLICA";
        }
      });
      ;
      this.dataSource = new MatTableDataSource(this.data);
      this.dataSource.sort = this.sort;
      this.customFilter(this.dataSource);
      this.itemsCount = this.dataSource.data.length;
      this.pageLoaded = true;
      this.processing = false;
    });
  }

  public editTextoSlip(item: TextoSlip) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;

    dialogConfig.data = {
      title: 'Editar Texto Slip',
      model: item,
    };

    const dialogref = this.dialog.open(TextoSlipEditorComponent, dialogConfig);

    dialogref.afterClosed().subscribe((data) => {
      if (data) {
        this.parametrizacionSlipWriter
          .editTextoSlip(data)
          .subscribe((response) => response);
      }
    });
  }

  private createTextoSlip() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.data = {
      title: 'Nueva Sección Slip',
    };

    const dialogref = this.dialog.open(CreateTextoSlipComponent, dialogConfig);

    dialogref
      .afterClosed()
      .subscribe((data) =>
        this.parametrizacionSlipWriter
          .createTextoSlip(data)
          .subscribe((response) => response)
      );
  }

  customFilter(dataSource: any) {
    this.dataSource.filterPredicate = ((data: TextoSlip, filter) => {
      const a = !filter.ramo || data.nombreRamo.toLowerCase().includes(filter.ramo);
      const b = !filter.subramo || data.nombreSubramo.toLowerCase().includes(filter.subramo);
      const c = !filter.amparo || data.nombreAmparo.toLowerCase().includes(filter.amparo);
      const d = !filter.seccion || data.nombreSeccion.toLowerCase().includes(filter.seccion);
      const e = !filter.sector || data.nombreSector.toLowerCase().includes(filter.sector);
      return a && b && c && d && e;
    }) as (data: TextoSlip, filter: any) => boolean;

    this.formControl.valueChanges.subscribe(value => {
      const filter = {
        ...value,
        ramo: value.ramo.trim().toLowerCase(),
        subramo: value.subramo.trim().toLowerCase(),
        sector: value.sector.trim().toLowerCase(),
        amparo: value.amparo.trim().toLowerCase(),
        seccion: value.seccion.trim().toLowerCase(),
      } as string;
      this.dataSource.filter = filter;
    });
  }
}
