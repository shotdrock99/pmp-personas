import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, Form, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Amparo, GrupoAseguradoExtended, IRango, RangoPerfilEdad, ValoresAseguradosEvents, OpcionesUsadas } from 'src/app/models';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { GrupoAseguradoWizardService } from 'src/app/services/grupo-asegurado-wizard.service';
import { PersonasReaderService } from 'src/app/services/personas-reader.service';
import { UploadFileService } from 'src/app/services/upload-file.service';
import { ConfirmDialogComponent, ConfirmDialogModel } from 'src/app/shared/components/confirm-dialog';
import { environment } from 'src/environments/environment';

import { ProfilesComponent } from './components/profiles/profiles.component';
import { GrupoAseguradoMapperService } from 'src/app/services/grupo-asegurado-mapper.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-asegurados',
  templateUrl: './asegurados.component.html',
  styleUrls: ['./asegurados.component.scss']
})
export class AseguradosComponent implements OnInit {
  totalRegistrosValidos: any;
  constructor(
    private uploadService: UploadFileService,
    private personasReaderService: PersonasReaderService,
    private cotizacionDataService: CotizacionPersistenceService,
    private grupoAseguradoMapperService: GrupoAseguradoMapperService,
    private wizardService: GrupoAseguradoWizardService,
    private formBuilder: FormBuilder,
    public dialog: MatDialog) { }

  URL_BASE = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  private fileData: File = null;
  private uploadResponse = { status: '', message: '', filePath: '' };

  @ViewChild('rangosedadescomponent')
  rangosEdadesComponent: ProfilesComponent;

  @Input() readonly: boolean;
  @Input() opcionesUsadas$: Observable<OpcionesUsadas>;
  opcionesUsadas: OpcionesUsadas;
  cantidadOpcionesUsadas: number;
  ErrorDistriCargue: boolean = true;
  showError: boolean;
  errorNumAseg: boolean;
  grupoAsegurado: GrupoAseguradoExtended;
  aseguradosForm: FormGroup;
  aseguradosFormClean: FormGroup;
  esTipoSumaFija: boolean;
  capturaAseguradosPorOpcion = false;
  capturaValorAsegurado = false;
  capturaNumeroAsegurados = true;
  capturaPorcentaje: boolean;
  capturaNumeroPotencialAsegurados: boolean;
  capturaEdadPromedio: boolean;
  showCargarAsegurados = false;
  uploadMessage: string;
  uploadCompleted: boolean;
  uploadEnabled = false;
  uploadFileEnabled = false;
  showLoading = false;
  uploadFail = false;
  uploadError: string;
  uploadErrorSummary: IUploadAseguradosErrorSummary;
  totalRegistros: number;
  rangos: IRango[];
  submitted = false;
  capturaRangos = false;
  hasAsegurados = false;
  showPorcentajeAsegurados = false;
  fileName: string;
  showRemoveAseguradosMessage: boolean;
  rangosPerfilEdad: RangoPerfilEdad[];
  rangosPerfilEdadLoaded = false;
  valorAseguradoText = 'Valor Asegurado';
  numeroAseguradosProcesados: number;

  get form() {
    return this.aseguradosForm.controls;
  }

  get tipoTasa(): number {
    return this.cotizacionDataService.cotizacion.informacionNegocio.tipoTasa1;
  }

  get numeroAsegurados() {
    return this.aseguradosForm.get('numeroAsegurados').value;
  }

  set numeroAsegurados(value: number) {
    this.aseguradosForm.get('numeroAsegurados').setValue(value);
  }

  get edadPromedio() {
    return this.aseguradosForm.get('edadPromedio').value;
  }

  set edadPromedio(value: number) {
    this.aseguradosForm.get('edadPromedio').setValue(value);
  }

  get valorAsegurado() {
    return this.aseguradosForm.get('valorAsegurado').value;
  }

  set valorAsegurado(value: number) {
    this.aseguradosForm.get('valorAsegurado').setValue(value);
  }

  get porcentajeAsegurados() {
    return this.aseguradosForm.get('porcentajeAsegurados').value;
  }

  set porcentajeAsegurados(value: number) {
    this.aseguradosForm.get('porcentajeAsegurados').setValue(value);
  }

  get rangosArray() {
    return this.aseguradosForm.get('rangos') as FormArray;
  }

  @Input() amparos: Amparo[];
  @Input() tipoEstructura: number;

  ngOnInit() {
    this.grupoAsegurado = this.wizardService.grupoAsegurado;
    this.aseguradosForm = this.wizardService.formGroup.get('asegurados') as FormGroup;
    this.hasAsegurados = this.grupoAsegurado.numeroAsegurados > 0;
    this.esTipoSumaFija = this.grupoAsegurado.tipoSumaAsegurada.codigoTipoSumaAsegurada === 1;
    this.registerConListaAseguradosChange();
    this.registerConDistribucionAseguradosChange();
    this.registerValorAseguradosChange();
    this.registerNumeroAseguradosChange();
    this.registerOpcionAseguradosChange();
    this.registerEdadPromedioChange();
    this.updateFormGroupSchema();
    this.updateFormGroupValues();
    // register upload file success
    this.registerUploadSuccessEvent();
    // register upload file fail
    this.registerUploadFailEvent();
    this.loadRangosPerfilEdades();
    this.updateForm();
    this.setValorAseguradoText();
    // register tipoEstructura value changes
    this.onTipoEstructuraValueChanges();
    this.validate();


    if (this.readonly) {
      this.aseguradosForm.disable();
      this.uploadFileEnabled = false;
    }
    this.numeroAseguradosProcesados = this.aseguradosForm.get('numeroAsegurados').value;
    this.opcionesUsadas$.subscribe((r: OpcionesUsadas) => {
      this.opcionesUsadas = r;
      this.cantidadOpcionesUsadas = Object.keys(Object.fromEntries(Object.entries(r).filter(([key, value]) => {
        return value === true;
      }))).length;
      this.updateControlOpciones();
    });
  }

  private setValorAseguradoText() {
    const esMultiploSueldos = this.grupoAsegurado.tipoSumaAsegurada.codigoTipoSumaAsegurada === 2;
    this.valorAseguradoText = esMultiploSueldos ? 'Valor Nómina' : 'Valor Asegurado';
  }

  private updateControlOpciones() {
    var ctrlAseguradosOpcion1 = this.aseguradosForm.get('aseguradosOpcion1');
    var ctrlAseguradosOpcion2 = this.aseguradosForm.get('aseguradosOpcion2');
    var ctrlAseguradosOpcion3 = this.aseguradosForm.get('aseguradosOpcion3');
    if (this.capturaAseguradosPorOpcion) {
      ctrlAseguradosOpcion1.setValidators([Validators.required, Validators.min(1)]);
      ctrlAseguradosOpcion1.updateValueAndValidity();
      ctrlAseguradosOpcion2.setValidators([Validators.required, Validators.min(1)]);
      ctrlAseguradosOpcion2.updateValueAndValidity();
      if (this.cantidadOpcionesUsadas > 2) {
        ctrlAseguradosOpcion3.setValidators([Validators.required, Validators.min(1)]);
        ctrlAseguradosOpcion3.updateValueAndValidity();
      }
    } else {
      this.aseguradosForm.get('aseguradosOpcion1').setValue(0);
      this.aseguradosForm.get('aseguradosOpcion2').setValue(0);
      this.aseguradosForm.get('aseguradosOpcion3').setValue(0);
      ctrlAseguradosOpcion1.setValidators(null);
      ctrlAseguradosOpcion1.updateValueAndValidity();
      ctrlAseguradosOpcion2.setValidators(null);
      ctrlAseguradosOpcion2.updateValueAndValidity();
      ctrlAseguradosOpcion3.setValidators(null);
      ctrlAseguradosOpcion3.updateValueAndValidity();
      this.aseguradosForm.get('numeroAsegurados').setValue(this.grupoAsegurado.numeroAsegurados);
    }
  }

  private validateDistriAseguradosEdades() {
    const ctrlConDistribucionAsegurados = this.aseguradosForm.get('conDistribucionAsegurados').value;
    if (ctrlConDistribucionAsegurados) {
      this.errorNumAseg = false;
      var AseguradosOpcion1 = this.aseguradosForm.get('aseguradosOpcion1').value;
      var AseguradosOpcion2 = this.aseguradosForm.get('aseguradosOpcion2').value;
      var AseguradosOpcion3 = this.aseguradosForm.get('aseguradosOpcion3').value;
      var totalAsegDis = AseguradosOpcion1 + AseguradosOpcion2 + AseguradosOpcion3;
      var totalAsegRangos = 0;
      this.rangosArray.value.forEach(element => {
        totalAsegRangos += element.numeroAsegurados;
      });
      if (totalAsegRangos == 0 && !this.capturaRangos && this.esTipoSumaFija) {
        this.errorNumAseg = false;
        this.wizardService.isAseguradosValid = true;
      } else {
        if (totalAsegDis != totalAsegRangos && this.esTipoSumaFija) {
          this.errorNumAseg = true;
          this.wizardService.isAseguradosValid = false
        }
      }
    } else {
      this.errorNumAseg = false;
    }
  }
  private updateFormGroupValues() {
    const ctrlAseguradosOpcion1 = this.aseguradosForm.get('aseguradosOpcion1');
    const ctrlAseguradosOpcion2 = this.aseguradosForm.get('aseguradosOpcion2');
    const ctrlAseguradosOpcion3 = this.aseguradosForm.get('aseguradosOpcion3');
    const ctrlConDistribucionAsegurados = this.aseguradosForm.get('conDistribucionAsegurados');
    const valorAseguradoCtrl = this.aseguradosForm.get('valorAsegurado');
    const numeroAseguradosCtrl = this.aseguradosForm.get('numeroAsegurados');
    const edadPromedioCtrl = this.aseguradosForm.get('edadPromedio');
    const porcentajeAseguradosCtrl = this.aseguradosForm.get('porcentajeAsegurados');
    ctrlAseguradosOpcion1.setValue(this.grupoAsegurado.aseguradosOpcion1);
    ctrlAseguradosOpcion2.setValue(this.grupoAsegurado.aseguradosOpcion2);
    ctrlAseguradosOpcion3.setValue(this.grupoAsegurado.aseguradosOpcion3);
    ctrlConDistribucionAsegurados.setValue(this.grupoAsegurado.conDistribucionAsegurados);
    valorAseguradoCtrl.setValue(this.grupoAsegurado.valorAsegurado);
    numeroAseguradosCtrl.setValue(this.grupoAsegurado.numeroAsegurados);
    edadPromedioCtrl.setValue(this.grupoAsegurado.edadPromedioAsegurados);
    porcentajeAseguradosCtrl.setValue(this.grupoAsegurado.porcentajeAsegurados);
    // map rangos
    if (this.grupoAsegurado.rangosGrupo) {
      this.grupoAsegurado.rangosGrupo.forEach(x => {
        this.rangosArray.push(this.formBuilder.group(x));
      });
    }
  }

  private registerEdadPromedioChange() {
    this.aseguradosForm.get('edadPromedio')
      .valueChanges
      .subscribe(() => {
        this.submitted = true;
        setTimeout(() => { this.validate(); }, 200);
      });
  }

  private registerValorAseguradosChange() {
    this.aseguradosForm.get('valorAsegurado')
      .valueChanges
      .subscribe(() => {
        this.submitted = true;
        setTimeout(() => { this.validate(); }, 200);
      });
  }

  private registerOpcionAseguradosChange() {
    this.aseguradosForm.get('aseguradosOpcion1').valueChanges.subscribe((value: number) => {
      this.actualizarNumeroAseguradosSegunOpcion();
      this.submitted = true;
      setTimeout(() => { this.validate(); }, 200);
    });
    this.aseguradosForm.get('aseguradosOpcion2').valueChanges.subscribe((value: number) => {
      this.actualizarNumeroAseguradosSegunOpcion();
      this.submitted = true;
      setTimeout(() => { this.validate(); }, 200);
    });
    this.aseguradosForm.get('aseguradosOpcion3').valueChanges.subscribe((value: number) => {
      this.actualizarNumeroAseguradosSegunOpcion();
      this.submitted = true;
      setTimeout(() => { this.validate(); }, 200);
    });
  }

  private validarCargueAseguradoSegunOpcion() {
    if (this.capturaAseguradosPorOpcion && this.showCargarAsegurados) {
      let totalAseguradosCargue = this.hasAsegurados ? this.grupoAsegurado.numeroAsegurados : 0;
      totalAseguradosCargue += totalAseguradosCargue === this.numeroAseguradosProcesados ? 0 : this.numeroAseguradosProcesados;
      if (this.esTipoSumaFija && this.capturaAseguradosPorOpcion) {
        const totalAseguradosOpciones = Number(this.aseguradosForm.get('aseguradosOpcion1').value +
          this.aseguradosForm.get('aseguradosOpcion2').value + this.aseguradosForm.get('aseguradosOpcion3').value);
        this.ErrorDistriCargue = totalAseguradosCargue === totalAseguradosOpciones;
        this.wizardService.isAseguradosValid = !this.ErrorDistriCargue;
      }
    }
  }

  private actualizarNumeroAseguradosSegunOpcion() {
    const totalAsegurados = Number(this.aseguradosForm.get('aseguradosOpcion1').value + this.aseguradosForm.get('aseguradosOpcion2').value +
      this.aseguradosForm.get('aseguradosOpcion3').value);
    if (totalAsegurados > 0) {
      this.aseguradosForm.get('numeroAsegurados').setValue(totalAsegurados);
    }
  }

  private registerNumeroAseguradosChange() {
    this.aseguradosForm.get('numeroAsegurados')
      .valueChanges
      .subscribe(() => {
        this.submitted = true;
        setTimeout(() => { this.validate(); }, 200);
      });
  }

  private registerUploadSuccessEvent() {
    this.uploadService.onUploadFinished.subscribe(res => {
      this.onUploadFileSuccess(res);
    });
  }

  private registerUploadFailEvent() {
    this.uploadService.onUploadFail.subscribe(res => {
      this.onUploadFileFail(res);
      this.showLoading = false;
    });
  }

  private updateFormGroupSchema() {
    const codigoTipoSumaAsegurada = this.grupoAsegurado.tipoSumaAsegurada.codigoTipoSumaAsegurada;
    const conListaAsegurados = this.grupoAsegurado.conListaAsegurados || false;
    // Mostar controls de subir archivo de aseegurados si se requiere
    this.showCargarAsegurados = conListaAsegurados;
    // Captura Suma Asegurada Total si el tipo de suma asegurada es diferente de Suma Fija (1) y SMMLV (10)
    this.capturaValorAsegurado = codigoTipoSumaAsegurada > 1 && codigoTipoSumaAsegurada < 10 && !conListaAsegurados;
    // Captura numero de asegurados, Edad promedio si es sin lista de asegurados
    this.capturaNumeroAsegurados = !conListaAsegurados;
    this.capturaEdadPromedio = !conListaAsegurados;
    this.capturaPorcentaje = false;
    this.capturaNumeroPotencialAsegurados = false;
    this.capturaRangos = false;

    switch (this.tipoTasa) {
      // si es tasa por ramo amparo
      case 1:
        break;
      // si es tipo Tasa por edad de cada asegurado
      case 2:
        // Configure campos requeridos al solicitar archivo de asegurados
        this.configureControlsOnConListaAsegurados();
        this.showCargarAsegurados = true;
        // No requiere habilitar campos, este caso solo funciona con archivo de asegurados
        this.capturaNumeroAsegurados = false;
        this.capturaEdadPromedio = false;
        break;
      // tipo tasa por rango de edades
      case 3:
        // this.capturaRangos = !conListaAsegurados;
        this.capturaNumeroAsegurados = false;
        this.capturaEdadPromedio = false;
        this.capturaRangos = (this.cotizacionDataService.cotizacion.requiereConfigurarPerfilEdades && !conListaAsegurados) ? true : false;
        break;
      // tipo tasa por edad promedio
      case 4:
        this.capturaPorcentaje = !conListaAsegurados && true;
        this.capturaNumeroPotencialAsegurados = !conListaAsegurados && true;
        break;
      // tasa por siniestralidad
      case 5:
        this.capturaPorcentaje = !conListaAsegurados && true;
        this.capturaNumeroPotencialAsegurados = !conListaAsegurados && true;
        break;
      default:
        break;
    }
    const disable = false;
    // this.updateValorAseguradoValidators();
    this.updateFormGroupValidators();
  }

  private loadRangosPerfilEdades() {
    const requiereConfigurarPerfilEdades = this.cotizacionDataService.cotizacion.requiereConfigurarPerfilEdades;
    if (requiereConfigurarPerfilEdades) {
      const codigoPerfil =
        this.cotizacionDataService.cotizacion.informacionNegocio.perfilEdad ==
          null
          ? 0
          : this.cotizacionDataService.cotizacion.informacionNegocio.perfilEdad;
      this.personasReaderService.getRangosPorPerfilEdad(codigoPerfil)
        .subscribe((response: RangoPerfilEdad[]) => {
          this.rangosPerfilEdad = response;
          this.rangosPerfilEdadLoaded = true;
        });
    }
  }

  private updateFormGroupValidators() {
    const valorAseguradoCtrl = this.aseguradosForm.get('valorAsegurado');
    const numeroAseguradosCtrl = this.aseguradosForm.get('numeroAsegurados');
    const edadPromedioCtrl = this.aseguradosForm.get('edadPromedio');
    const porcentajeAseguradosCtrl = this.aseguradosForm.get('porcentajeAsegurados');
    const numeroPotencialAseguradosCtrl = this.aseguradosForm.get('numeroPotencialAsegurados');
    const rangosCtrl = this.aseguradosForm.get('rangos');

    if (!this.capturaValorAsegurado) {
      valorAseguradoCtrl.setValidators(null);
    } else {
      valorAseguradoCtrl.setValidators([Validators.required, Validators.min(1)]);
    }
    if (!this.capturaNumeroAsegurados) {
      numeroAseguradosCtrl.setValidators(null);
    } else {
      numeroAseguradosCtrl.setValidators([Validators.required, Validators.min(1)]);
    }
    if (!this.capturaEdadPromedio) {
      edadPromedioCtrl.setValidators(null);
    } else {
      edadPromedioCtrl.setValidators([Validators.required, Validators.min(1)]);
    }
    if (!this.capturaPorcentaje) {
      porcentajeAseguradosCtrl.setValidators(null);
    }
    if (!this.capturaNumeroPotencialAsegurados) {
      numeroPotencialAseguradosCtrl.setValidators(null);
    }
    if (!this.capturaRangos) {
      rangosCtrl.setValidators(null);
    }
    valorAseguradoCtrl.updateValueAndValidity();
    numeroAseguradosCtrl.updateValueAndValidity();
    edadPromedioCtrl.updateValueAndValidity();
    porcentajeAseguradosCtrl.updateValueAndValidity();
    numeroPotencialAseguradosCtrl.updateValueAndValidity();
  }

  private updateForm() {
    const conListaAseguradosCtrl = this.aseguradosForm.get('conListaAsegurados');
    const tipoEstructuraCtrl = this.aseguradosForm.get('tipoEstructura');
    const tpEstructura =
      this.tipoEstructura === 1
        ? 'tipoUno'
        : this.tipoEstructura === 2
          ? 'tipoDos'
          : this.tipoEstructura === 3
            ? 'tipoTres'
            : this.tipoEstructura === 4
              ? 'tipoCuatro'
              : '';
    tipoEstructuraCtrl.setValue(tpEstructura);
    this.uploadFileEnabled = tipoEstructuraCtrl.value !== '' ? true : false;
    const tieneListaAsegurados = this.cotizacionDataService.cotizacion.informacionNegocio.conListaAsegurados;
    const esTasaEdadAsegurado = this.cotizacionDataService.cotizacion.informacionNegocio.tipoTasa1 === 2 ||
      this.cotizacionDataService.cotizacion.informacionNegocio.tipoTasa2 === 2;
    if (tieneListaAsegurados || esTasaEdadAsegurado) {
      conListaAseguradosCtrl.setValue(true);
      // tipoEstructuraCtrl.setValue(this.cotizacionDataService.cotizacion.informacionGruposAsegurados.gruposAsegurados.)
    }
    if (this.hasAsegurados) {
      this.aseguradosForm.patchValue({
        // conListaAsegurados: [false],
        // valorAsegurado: [''],
        // numeroAsegurados: ['', numeroAseguradosValidators],
        // porcentajeAsegurados: [''],
        // numeroPotencialAsegurados: [''],
        // edadPromedio: ['', edadPromedioValidators],
        archivoCargado: [true],
        // rangos: this.formBuilder.array([])
      });
    }
    conListaAseguradosCtrl.disable();
    // tipoEstructuraCtrl.disable();
  }

  private onTipoEstructuraValueChanges() {
    this.aseguradosForm
      .get('tipoEstructura')
      .valueChanges.subscribe(
        (te) => (this.uploadFileEnabled = te !== '' ? true : false)
      );
  }

  // private updateValorAseguradoValidators() {
  //   const valorAseguradoCtrl = this.aseguradosForm.get('valorAsegurado');
  //   valorAseguradoCtrl.setValidators(null);
  //   if (this.capturaValorAsegurado) {
  //     valorAseguradoCtrl.setValidators([Validators.required]);
  //     valorAseguradoCtrl.updateValueAndValidity();
  //   }
  // }

  private configureControlsOnConListaAsegurados() {
    const archivoCargadoCtrl = this.aseguradosForm.get('archivoCargado');
    const tipoArchivoCtrl = this.aseguradosForm.get('tipoEstructura');
    const rangosCtrl = this.aseguradosForm.get('rangos');
    // si requiere lista de asegurados
    if (this.showCargarAsegurados) {
      archivoCargadoCtrl.setValidators([Validators.required]);
      archivoCargadoCtrl.updateValueAndValidity();
      tipoArchivoCtrl.setValidators([Validators.required]);
      tipoArchivoCtrl.updateValueAndValidity();
      // eliminar validadores de numero asegurados
      const numeroAseguradosCtrl = this.aseguradosForm.get('numeroAsegurados');
      numeroAseguradosCtrl.setValidators(null);
      numeroAseguradosCtrl.updateValueAndValidity();
      // eliminar validadores de edad promedio
      const edadPromedioCtrl = this.aseguradosForm.get('edadPromedio');
      edadPromedioCtrl.setValidators(null);
      edadPromedioCtrl.updateValueAndValidity();
      rangosCtrl.setValidators(null);
      rangosCtrl.updateValueAndValidity();
    } else {
      archivoCargadoCtrl.setValidators(null);
      archivoCargadoCtrl.updateValueAndValidity();
      if (this.capturaRangos) {
        rangosCtrl.setValidators([Validators.required]);
        rangosCtrl.updateValueAndValidity();
      }
    }
  }

  private registerConListaAseguradosChange() {
    this.aseguradosForm.get('conListaAsegurados')
      .valueChanges
      .subscribe(conListaAsegurados => {
        // update grupo asegurado
        this.grupoAsegurado.conListaAsegurados = conListaAsegurados;
        this.updateFormGroupSchema();
        this.configureControlsOnConListaAsegurados();
      });
  }

  private registerConDistribucionAseguradosChange() {
    this.aseguradosForm.get('conDistribucionAsegurados')
      .valueChanges
      .subscribe(conDistribucionAsegurados => {
        
        this.capturaAseguradosPorOpcion = conDistribucionAsegurados;
        this.updateControlOpciones();
      });
  }

  private fileProgress(fileInput: any) {
    this.fileData = fileInput.target.files[0] as File;
  }

  private onUploadFileSuccess(res: UploadAseguradosResponse) {
    this.uploadCompleted = true;
    this.uploadFail = false;
    // update form values
    this.numeroAsegurados = res.totalAsegurados;
    this.numeroAseguradosProcesados = res.registrosProcesados;
    this.edadPromedio = res.edadPromedio;
    this.valorAsegurado = res.valorAsegurado;
    this.porcentajeAsegurados = res.porcentajeAsegurados;
    this.totalRegistros = res.registrosTotales;
    this.totalRegistrosValidos = res.totalRegistrosValidos;
    this.uploadErrorSummary = {
      registrosFallidos: res.registrosFallidos,
      errorSummary: res.errorSummary,
      withErrors: res.withErrors
    };
    this.showLoading = false;
    // update step validity
    this.wizardService.isAseguradosValid = true;
    this.validarCargueAseguradoSegunOpcion();
  }

  private onUploadFileFail(res: any) {
    this.uploadFail = true;
    this.uploadError = res.error;
    this.wizardService.isAseguradosValid = false;
  }

  private createFile(data: any) {
    // Create a virtual Anchor tag
    const link = document.createElement('a');
    link.setAttribute('href', data);
    link.setAttribute('download', 'asegurados.csv');

    // Append the Anchor tag in the actual web page or application
    document.body.appendChild(link);

    // Trigger the click event of the Anchor link
    link.click();

    // Remove the Anchor link form the web page or application
    document.body.removeChild(link);
  }

  private validate() {
    const isValid = this.aseguradosForm.valid;
    this.wizardService.isAseguradosValid = isValid;
    this.validarCargueAseguradoSegunOpcion();
    this.validateDistriAseguradosEdades();
  }

  async deleteAsegurados(e) {
    const message = `¿Está seguro de eliminar los asegurados de este grupo? \nImportante: Esta accion no se podrá deshacer.`;
    const dialogData = new ConfirmDialogModel('Eliminar Asegurados', message);
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: dialogData
    });

    dialogRef.afterClosed().subscribe(async (dialogResult: boolean) => {
      if (dialogResult) {
        const response = await this.wizardService.deleteAseguradosAsync(this.grupoAsegurado.codigoGrupoAsegurado);
        this.hasAsegurados = false;
        if (response.State === 1) {
          this.showRemoveAseguradosMessage = true;
        }
      }
    });
  }

  onFileChange(event) {
    


    if (event.target.files.length > 0) {
      let file = event.target.files[0];
      this.fileName = file.name; //+ Math.floor(Math.random() * 99).toString();
      this.aseguradosForm.get('archivoCargado').setValue(file);
      this.uploadEnabled = true;
    }
  }

  onUploadFile() {
    //this.aseguradosForm.updateOn
    this.uploadCompleted = false;
    this.showLoading = true;
    const tipoArchivo = this.aseguradosForm.get('tipoEstructura').value;
    const tipoEstructura =
      tipoArchivo === 'tipoUno'
        ? 1
        : tipoArchivo === 'tipoDos'
          ? 2
          : tipoArchivo === 'tipoTres'
            ? 3
            : 4;
    let numeroSalarios = this.grupoAsegurado.numeroSalariosAsegurado;
    let valorMin = this.grupoAsegurado.valorMinAsegurado;
    let valorMax = this.grupoAsegurado.valorMaxAsegurado;
    const amparoBNA = this.wizardService.valoresAseguradosArray.value.find(x => !x.amparo.siNoAdicional && x.amparo.siNoBasico);
    if (this.wizardService.capturaSalarios) {
      const opcion = amparoBNA.opciones[0];
      numeroSalarios = opcion.numeroSalariosOption.rawValue;
    }
    if (this.wizardService.tipoSumaAsegurada.codigoTipoSumaAsegurada === 1) {
      valorMin = Math.min.apply(Math, amparoBNA.opciones.map((o) => o.valorOption.rawValue));
      valorMax = Math.max.apply(Math, amparoBNA.opciones.map((o) => o.valorOption.rawValue));
    }
    const formJSON = this.wizardService.formGroup.getRawValue();
    const gruposAsegurados = this.grupoAseguradoMapperService.map(this.grupoAsegurado, formJSON);
    const amparoBNAEdadesGrupo = gruposAsegurados.amparosGrupo.find((x: any) => x.esAmparoBasicoNoAdicional).edadesGrupo;
    const edadMinIngreso = amparoBNAEdadesGrupo.edadMinAsegurado;
    const edadMaxIngreso = amparoBNAEdadesGrupo.edadMaxAsegurado;
    const edadPermanenciaIngreso = amparoBNAEdadesGrupo.edadMaxPermanencia;
    const cotizacion = this.cotizacionDataService.cotizacion;
    const codigoGrupoAsegurado = this.grupoAsegurado.codigoGrupoAsegurado;
    const uploadURL = `${this.URL_BASE}/${cotizacion.codigoCotizacion}/gruposasegurados/${codigoGrupoAsegurado}/asegurados/` +
      `upload?numeroSalarios=${numeroSalarios}&valorMin=${valorMin}&valorMax=${valorMax}&edadMinimaAsegurado=${edadMinIngreso}` +
      `&edadMaximaAsegurado=${edadMaxIngreso}&edadPermanenciaAsegurado=${edadPermanenciaIngreso}&tipoEstructura=${tipoEstructura}`;
    const formData = new FormData();
    const data = this.aseguradosForm.get('archivoCargado').value;
    formData.append('file', data);
    this.uploadService.upload2(uploadURL, formData);
    this.uploadEnabled = false;
  }

  downloadBaseFile(e: Event, fileType: string) {
    e.preventDefault();
    // const tipoArchivo = this.aseguradosForm.get('tipoEstructura').value;
    const arr =
      fileType === 'tipoUno'
        ? [['Documento', 'Fecha Nacimiento', 'Valor Asegurado']]
        : fileType === 'tipoDos'
          ? [['Documento', 'Edad', 'Valor Asegurado']]
          : fileType === 'tipoTres'
            ? [['Nombre', 'Fecha Nacimiento', 'Valor Asegurado']]
            : [['Nombre', 'Edad', 'Valor Asegurado']];
    const csv = arr.map(row => row.map(item => {
      return (typeof item === 'string' && item.indexOf(',') >= 0) ? `"${item}"` : String(item);
    }).join(',')).join('\n');
    // Format the CSV string
    const data = encodeURI('data:text/csv;charset=utf-8,' + csv);
    this.createFile(data);
  }

  downloadFailedFile(e) {
    e.preventDefault();
    const tipoArchivo = this.aseguradosForm.get('tipoEstructura').value;
    const arr =
      tipoArchivo === 'tipoUno'
        ? [['Documento', 'Fecha Nacimiento', 'Valor Asegurado', 'Error']]
        : tipoArchivo === 'tipoDos'
          ? [['Documento', 'Edad', 'Valor Asegurado', 'Error']]
          : tipoArchivo === 'tipoTres'
            ? [['Nombre', 'Fecha Nacimiento', 'Valor Asegurado', 'Error']]
            : [['Nombre', 'Edad', 'Valor Asegurado', 'Error']];
    // let arr = [['NumeroDocumento', 'FechaNacimiento', 'ValorAsegurado', 'Error']];
    const summary: IErrorSummary[] = this.uploadErrorSummary.errorSummary;
    summary.forEach((item: IErrorSummary, idx: number) => {
      const numDoc = item.asegurado.numeroDocumento;
      const fechaNacimiento = item.asegurado.fechaNacimiento;
      const edad = item.asegurado.edad;
      const valorAsegurado = item.asegurado.valorAsegurado;
      const nombre = item.asegurado.nombre;
      const fechaOriginal = item.asegurado.fechaOriginal;
      // let message = item.errors.map(x => x.split(':')[1].trim()).join(',');
      const message = item.errors.join(',');
      // push line
      if (tipoArchivo === 'tipoUno') {
        arr.push([numDoc.toString(), fechaOriginal, valorAsegurado.toString(), message]);
      } else if (tipoArchivo === 'tipoDos') {
        arr.push([numDoc.toString(), fechaOriginal.toString(), valorAsegurado.toString(), message]);
      } else if (tipoArchivo === 'tipoTres') {
        arr.push([nombre, fechaOriginal, valorAsegurado.toString(), message]);
      } else {
        arr.push([nombre, fechaOriginal.toString(), valorAsegurado.toString(), message]);
      }
    });
    const csv = arr.map(row => row.map(item => {
      return (typeof item === 'string' && item.indexOf(',') >= 0) ? `"${item}"` : String(item);
    }).join(',')).join('\n');
    // Format the CSV string
    const data = encodeURI('data:text/csv;charset=utf-8,' + csv);
    this.createFile(data);
  }

  onRangosChange(args) {
    this.rangos = args;

    // clear form array
    while (this.rangosArray.controls.length !== 0) {
      this.rangosArray.removeAt(0);
    }

    if (this.rangos.length > 0) {
      this.rangos.forEach(x => {
        this.rangosArray.push(this.formBuilder.control(x));
      });
    } else {
      const rangosCtrl = this.aseguradosForm.get('rangos');
      rangosCtrl.setErrors({ required: true });
      // rangosCtrl.setValidators([minLengthArray(2)]);
      // rangosCtrl.setErrors({ 'minLengthArray(2)': true })
    }

    this.validate();
  }
}

export const minLengthArray = (min: number) => {
  return (c: AbstractControl): { [key: string]: any } => {
    if (c.value.length >= min) {
      return null;
    }
    return { MinLengthArray: true };
  };
};

export interface UploadAseguradosResponse {
  totalAsegurados: number;
  registrosTotales: number;
  totalRegistrosValidos: number;
  registrosProcesados: number;
  edadPromedio: number;
  valorAsegurado: number;
  porcentajeAsegurados: number;
  registrosFallidos: number;
  withErrors: boolean;
  errorSummary: any;
}

export interface IUploadAseguradosErrorSummary {
  registrosFallidos: number;
  withErrors: boolean;
  errorSummary: any;
}

export interface IErrorSummary {
  asegurado: IAsegurado;
  errors: any[];
}

export interface IAsegurado {
  edad: number;
  fechaNacimiento: Date;
  numeroDocumento: number;
  valorAsegurado: number;
  fechaOriginal: string;
  nombre: string;
}
