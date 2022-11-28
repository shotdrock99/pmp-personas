import { Component, Input, OnInit, Output, Renderer2, ViewChild, EventEmitter } from '@angular/core';
import { FormArray, FormGroup, FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { GrupoAsegurado } from 'src/app/models';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { GruposAseguradosReaderService } from 'src/app/services/grupos-asegurados-reader.service';
import { NavegacionService } from 'src/app/services/navegacion.service';
import { ConfirmDialogComponent, ConfirmDialogModel } from 'src/app/shared/components/confirm-dialog';

import { InformacionGruposAseguradosViewData } from './../../../models/cotizacion';
import {
  AgregarGrupoAseguradoComponent,
} from './components/modal-components/agregar-grupo-asegurado/agregar-grupo-asegurado.component';
import {
  ConfigurarGrupoAseguradoComponent,
} from './components/modal-components/configurar-grupo-asegurado/configurar-grupo-asegurado.component';
import {
  EditarNombreGrupoAseguradoComponent,
} from './components/modal-components/editar-nombre-grupo-asegurado/editar-nombre-grupo-asegurado.component';
import { GruposAseguradosWriterService } from 'src/app/services/gruposasegurados-writer.service';
import { NotificationService } from 'src/app/shared/services/notification.service';


@Component({
  selector: 'app-informacion-grupos-asegurados',
  templateUrl: './informacion-grupos-asegurados.component.html',
  styleUrls: ['./informacion-grupos-asegurados.component.scss']
})
export class InformacionGruposAseguradosComponent implements OnInit {

  constructor(
    private formBuilder: FormBuilder,
    private renderer: Renderer2,
    private notificationService: NotificationService,
    private cotizacionDataService: CotizacionPersistenceService,
    private gruposAseguradosReader: GruposAseguradosReaderService,
    private gruposAseguradosWriterService: GruposAseguradosWriterService,
    public dialog: MatDialog,
    private navigationService: NavegacionService) { }


  @Input() model: InformacionGruposAseguradosViewData;
  @Input() readonly: boolean;
  @Output() sumaFijaUpdate = new EventEmitter();

  @ViewChild(MatPaginator)
  paginator: MatPaginator;

  @ViewChild(MatSort)
  sort: MatSort;

  private indexView = 4;
  showErrors: boolean;
  errors: string[] = [];
  formErrors: string[] = [];
  submitted = false;
  isEditMode = false;
  itemsCount = 0;
  changes: MutationObserver;
  gruposAseguradosForm: FormGroup;
  dataSource: MatTableDataSource<any>;
  displayedColumns1: string[] = [
    'nombre',
    'tiposumaasegurada',
    'sumaminimaasegurada',
    'sumamaximaasegurada',
    'opciones'
  ];
  dialogResult: any;

  allowCreateGrupoAsegurados = true;

  @ViewChild('tableGruposAsegurados', { static: true })
  tableHtml: any;

  get gruposAseguradosArray() {
    return this.gruposAseguradosForm.get('gruposAsegurados') as FormArray;
  }

  ngOnInit() {

    this.gruposAseguradosForm = this.formBuilder.group({
      gruposAsegurados: this.formBuilder.array([])
    });

    this.dataSource = new MatTableDataSource<any>([]);
    this.navigationService.subscribe({
      indexView: this.indexView,
      continuePromise: () => {
        return this.continue();
      },
      initializePromise: () => {
        this.initializeSection();
      }
    });

    this.gruposAseguradosForm.disable();
    if (this.cotizacionDataService.isEdit) {
      this.updateForm();
      this.gruposAseguradosForm.enable();
    }
  }

  private updateForm() {
    this.allowCreateGrupoAsegurados = true;
    this.dataSource = new MatTableDataSource<any>(this.model.gruposAsegurados);
    this.model.gruposAsegurados.forEach((item, idx) => {
      // item.configured = true;
      (this.gruposAseguradosForm.controls.gruposAsegurados as FormArray)
        .push(this.formBuilder.group(item));
    });

    this.itemsCount = this.dataSource.data.length;

    if (this.readonly) {
      this.gruposAseguradosForm.disable();
      this.allowCreateGrupoAsegurados = false;
    }
  }

  private initializeSection() {
    this.gruposAseguradosForm.enable();
    this.allowCreateGrupoAsegurados = true;
  }

  private clearErrors() {
    this.errors = [];
  }

  private validate(): boolean {
    this.clearErrors();
    const gruposAseguradosRawValue = this.gruposAseguradosArray.value;
    const hasGroups = gruposAseguradosRawValue.length > 0;
    const areValidGroups = gruposAseguradosRawValue.every((x: any) => x.configured);
    const isValidForm = hasGroups && areValidGroups;
    if (!isValidForm) {
      if (!hasGroups) {
        this.errors.push('No hay grupos configurados. Revise la configuración de los grupos antes de continuar.');
      } else if (!areValidGroups) {
        this.errors.push('Existen grupos que no han sido configurados. Revise la configuración de los grupos antes de continuar.');
      }
    }

    return isValidForm;
  }

  private continue() {
    const isValidForm = this.validate();
    if (!isValidForm) {
      this.showErrors = true;
      this.formErrors = this.errors;
    }
    return isValidForm;
  }

  private async addGrupoAseguradoAsync(args: GrupoAsegurado) {
    return new Promise(async (resolve, reject) => {
      let key: string;
      const response = await this.gruposAseguradosWriterService.crearGrupoAsegurados(args);
      if (response[key = 'status'] !== 1) {
        reject('Hubo un error agregando el grupo.');
      }
      args.codigoGrupoAsegurado = response[key = 'codigo'];
      resolve(args);
      (this.gruposAseguradosForm.controls.gruposAsegurados as FormArray)
        .push(this.formBuilder.group(args));
    });
  }

  private updateGrupoAsegurado(groupIndex: number, model: any) {
    const toast = this.notificationService.showToast('Guardando Grupo', 0);
    let control = (this.gruposAseguradosForm.controls.gruposAsegurados as FormArray).at(groupIndex);
    control.get('configured').setValue(true);
    this.dataSource.data[groupIndex].configured = true;
    this.gruposAseguradosWriterService.updateGrupoAsegurado(model)
      .subscribe(res => {
        if (!res) { return; }
        if (groupIndex >= 0) {
          const codigoGrupoAsegurado = model.codigoGrupoAsegurado;
          control = (this.gruposAseguradosForm.controls.gruposAsegurados as FormArray).at(groupIndex);
          control.get('configured').setValue(true);

          this.dataSource.data[groupIndex].configured = true;
          this.validate();

          toast.dismiss();

          if (this.dialogResult.codigoTipoSuma === 1) {
            this.sumaFijaUpdate.emit(this.dialogResult);
          }
        }
      });
  }

  openAgregarGrupoAsegurados() {
    const codigoCotizacion = this.cotizacionDataService.cotizacion.codigoCotizacion;
    const nombresGrupos = this.dataSource.data.map(x => x.nombre);
    const dialogRef = this.dialog.open(AgregarGrupoAseguradoComponent, {
      width: '500px',
      data: {
        nombresGrupos
      }
    });

    const conListaAsegurados = this.cotizacionDataService.cotizacion.informacionNegocio.conListaAsegurados;

    dialogRef.afterClosed().subscribe(async result => {
      if (result === undefined) { return; }
      const grupo = await this.addGrupoAseguradoAsync({
        codigoCotizacion,
        nombreGrupoAsegurado: result.nombre,
        tipoSumaAsegurada: result.tipoSumaAsegurada,
        valorMinAsegurado: result.sumaAseguradaMinima,
        valorMaxAsegurado: result.sumaAseguradaMaxima,
        conListaAsegurados,
        valorAsegurado: 0,
        configured: false
      });

      if (grupo) {
        const ds = this.dataSource.data;
        ds.push(grupo);
        this.dataSource.data = ds;
        await this.openConfigureGrupo(null, grupo as GrupoAsegurado, { dirty: false });
      }

      this.itemsCount = this.dataSource.data.length;
    });
  }

  openEditGroupName(grupo: GrupoAsegurado) {
    const codigoCotizacion = this.cotizacionDataService.cotizacion.codigoCotizacion;
    const nombresGrupos = this.dataSource.data.map(x => x.nombre);
    const dialogRef = this.dialog.open(EditarNombreGrupoAseguradoComponent, {
      width: '500px',
      data: {
        nombreGrupo: grupo.nombreGrupoAsegurado,
        nombresGrupos
      }
    });

    dialogRef.afterClosed().subscribe(async result => {
      if (result === undefined) {  return; }
      const group = this.dataSource.data.find(x => x.codigoGrupoAsegurado === grupo.codigoGrupoAsegurado);
      const idx = this.dataSource.data.indexOf(group);
      const grupoAseguradoModel = await this.gruposAseguradosReader.consultarGrupoAsync(grupo.codigoGrupoAsegurado) as any;
      grupoAseguradoModel.nombreGrupoAsegurado = result.nombre;
      this.updateGrupoAsegurado(idx, grupoAseguradoModel);

      const g = this.dataSource.data.find(x => x.codigoGrupoAsegurado === grupo.codigoGrupoAsegurado);
      g.nombreGrupoAsegurado = result.nombre;
    });
  }

  async openConfigureGrupo(e, grupoAsegurado: GrupoAsegurado, options?: any) {
    const dirty = options ? options.dirty : true;
    const grupoAseguradoModel = await this.gruposAseguradosReader.consultarGrupoAsync(grupoAsegurado.codigoGrupoAsegurado);

    const conListaAsegurados = this.cotizacionDataService.cotizacion.informacionNegocio.conListaAsegurados;
    grupoAsegurado.conListaAsegurados = conListaAsegurados;

    const dialogRef = this.dialog.open(ConfigurarGrupoAseguradoComponent, {
      disableClose: true,
      width: '1200px',
      data: {
        grupoAsegurado,
        grupoAseguradoInfo: grupoAseguradoModel,
        dirty,
        readonly: this.readonly
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      this.__removeWeirdHeaders();
      if (!result) { return; }
      const group = this.dataSource.data.find(x => x.codigoGrupoAsegurado === grupoAsegurado.codigoGrupoAsegurado);
      const idx = this.dataSource.data.indexOf(group);
      if (result.codigoTipoSuma === 1) {
        this.dialogResult = result;
      }
      this.updateGrupoAsegurado(idx, result);
    });
  }

  private async removeGrupoAsegurado(codigo: number) {
    const result: any = await this.gruposAseguradosWriterService.removeGrupoAseguradoAsync(codigo);
    return result;
  }

  // TODO must identify headers addition
  private __removeWeirdHeaders() {
    const nativeEl = this.tableHtml._elementRef.nativeElement;
    const rows: any[] = nativeEl.querySelectorAll('tr.mat-row');
    rows.forEach(r => {
      const ths: any[] = r.querySelectorAll('th');
      ths.forEach(el => {
        this.renderer.removeChild(nativeEl, el);
      });
    });
  }

  async removeGrupo(e: any, args: GrupoAsegurado) {
    const message = `¿Está seguro de que desea eliminar este grupo de asegurados?`;
    const dialogData = new ConfirmDialogModel('Eliminar Grupo de Asegurados', message);
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: dialogData
    });

    dialogRef.afterClosed().subscribe(async (dialogResult: boolean) => {
      if (dialogResult) {

        const response = await this.removeGrupoAsegurado(args.codigoGrupoAsegurado);
        // remove item from datasource
        const idx = this.dataSource.data.findIndex(x => x.codigoGrupoAsegurado === args.codigoGrupoAsegurado);
        this.dataSource.data.splice(idx, 1);

        const ds = this.dataSource.data;
        this.dataSource.data = ds;

        // remove from fromGroup
        if (idx >= 0) {
          const gruposAsegurados = (this.gruposAseguradosForm.get('gruposAsegurados') as FormArray);
          if (gruposAsegurados) {
            gruposAsegurados.removeAt(idx);
          }
        }

        this.itemsCount = this.dataSource.data.length;
      }
    });
  }
}
