import { Sector } from './../../../models/informacion-cliente';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { PerfilEdad, PerfilValor, PeriodoFacturacion } from 'src/app/models';
import { NavegacionService } from 'src/app/services/navegacion.service';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { InformacionnegocioWriterService } from 'src/app/services/informacionnegocio-writer.service';
import { PersonasReaderService } from 'src/app/services/personas-reader.service';
import { RequireMatch } from 'src/app/shared/functions/requireMatch';
import { SoligesproUser } from 'src/app/models/soligespro-user';
import * as moment from 'moment';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ApplicationUser } from 'src/app/models/application-user';
import { NotificationService } from 'src/app/shared/services/notification.service';

export const DATE_FORMATS = {
  parse: {
    dateInput: 'DD/MM/YYYY',
  },
  display: {
    dateInput: 'DD/MM/YYYY',
    monthYearLabel: 'MM/YYYY',
    dateA11yLabel: 'DD/MM/YYYY',
    monthYearA11yLabel: 'MM/YYYY',
  },
};

export const DEFAULT_DATETIME = '0001-01-01T00:00:00';

@Component({
  selector: 'app-informacion-aseguramiento',
  templateUrl: './informacion-aseguramiento.component.html',
  styleUrls: ['./informacion-aseguramiento.component.scss'],
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'es' },
    { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
    { provide: MAT_DATE_FORMATS, useValue: DATE_FORMATS },
  ]
})
export class InformacionAseguramientoComponent implements OnInit {
  indexView = 2;
  informacionNegocioForm: FormGroup;
  submitted = false;
  showTasa2 = false;
  showOtrosGastos = false;
  showPorcentajeComision = true;
  showCapturaPerfilEdades = false;
  showCapturaPerfilValores = false;
  conListaAsegurados = false;
  conAnyosSiniestralidad = false;
  currentUser: ApplicationUser = this.authenticationService.currentUserValue;
  minDate: any;
  periodosFacturacion: any[] = [];
  tiposRiesgo: any[] = [];
  tiposNegocio: any[] = [];
  tiposContratacion: any[] = [];
  sectores: Sector[] = [];
  tiposTasa: any[] = [];
  tiposTasa1: any[] = [];
  tiposTasa2: any[] = [];
  perfilesEdad: PerfilEdad[] = [];
  perfilesValor: PerfilValor[] = [];
  filteredPeriodosFacturacion: Observable<PeriodoFacturacion[]>;
  parametrizacionAnyosSiniestralidad: number;

  public mask = {
    guide: true,
    showMask: true,
    mask: [/\d/, /\d/, '/', /\d/, /\d/, '/', /\d/, /\d/, /\d/, /\d/]
  };

  @Input() model: any;
  @Input() readonly: boolean;
  @Input() version: number;
  @Output() OnEsNegocioDirectoEmitter = new EventEmitter();
  @Output() OnApliqueSiniestralidadEmitter = new EventEmitter();
  @Output() OnAnyosSiniestralidad: EventEmitter<number> = new EventEmitter();
  directoresComerciales: SoligesproUser[];

  constructor(
    private formBuilder: FormBuilder,
    private navigationService: NavegacionService,
    private notificationService: NotificationService,
    private cotizacionDataService: CotizacionPersistenceService,
    private informacionNegocioWriter: InformacionnegocioWriterService,
    private personasReaderService: PersonasReaderService,
    private dateAdapter: DateAdapter<any>,
    private authenticationService: AuthenticationService) { }

  // convenience getter for easy access to form fields
  get form() { return this.informacionNegocioForm.controls; }

  ngOnInit() {
    const currentYear = moment();
      this.minDate = currentYear;
    this.dateAdapter.setLocale('es');
    this.navigationService.subscribe({
      indexView: this.indexView,
      continuePromise: () => {
        return this.continue();
      },
      initializePromise: () => {
        this.initializeSection();
      }
    });
    const dt = new Date().toISOString();
    this.informacionNegocioForm = this.formBuilder.group({
      DirectorComercial: [''],
      NombreAseguradora: ['', [Validators.maxLength(100)]],
      FechaInicio: [''],
      FechaFin: [''],
      Actividad: ['', [Validators.required, Validators.maxLength(200), Validators.pattern(/^[A-Za-z\u00f1\u00d1\s]+$/)]],
      PeriodoFacturacion: ['', [Validators.required]],
      TipoRiesgo: ['', [Validators.required]],
      TipoNegocio: ['', [Validators.required]],
      TipoContratacion: ['', [Validators.required]],
      Sector: ['', [Validators.required]],
      TipoTasa1: ['', [Validators.required]],
      TipoTasa2: [0],
      AnyosSiniestralidad: [null],
      PorcentajeRetorno: ['', [Validators.required, Validators.max(200), Validators.min(0)]],
      PorcentajeOtrosGastos: ['', [Validators.max(200), Validators.min(0)]],
      PorcentajeComision: ['', [Validators.required, Validators.max(200), Validators.min(1)]],
      OtrosGastos: [''],
      PerfilEdad: [''],
      PerfilValor: [''],
      EsNegocioDirecto: [false],
      ConListaAsegurados: [false]
    });

    this.informacionNegocioForm.disable();
    if (this.cotizacionDataService.isEdit) {
      this.initializeSection();
      this.updateForm();
    }
    if (this.version > 1) {
      this.disableControls(['Sector', 'Actividad', 'TipoRiesgo', 'TipoNegocio', 'TipoContratacion', 'EsNegocioDirecto']);
    }
  }
  myDateFilter = (m: any | null): boolean => {
    const year = (m || moment()).year();
    return year >= moment().year() -1;
  } 
  private disableControls(controls: string[]) {
    controls.forEach((controlName) => {
      this.informacionNegocioForm.get(controlName).disable();
    });
  }
  private initializeSection() {
    this.informacionNegocioForm.enable();
    this.periodosFacturacion = this.cotizacionDataService.periodosFacturacion;
    this.tiposRiesgo = this.cotizacionDataService.tiposRiesgo;
    this.tiposNegocio = this.cotizacionDataService.tiposNegocio;
    this.sectores = this.cotizacionDataService.sectores;
    this.perfilesEdad = this.cotizacionDataService.perfilesEdad;
    this.perfilesValor = this.cotizacionDataService.perfilesValor;
    this.parametrizacionAnyosSiniestralidad = Number(this.cotizacionDataService.paramatrizacionSiniestralidad.valorVariable);
    this.definePeriodoFacturacionObservable();
    this.loadTasas();
    this.loadDirectoresComerciales();
    // this.registerPeriodoFacturacionChange();
    this.registerFechaInicioChange();
    this.registerFechaFinChange();
    this.registerEsNegocioDirectoChange();
    this.registerTasa1Change();
    this.registerTasa2Change();
    this.registerPorcentajeOtrosGastosChange();
    this.registerTipoNegocioChange();
    this.registerSectorChange();
    this.registerConListaAseguradosChange();
    this.registerPerfilEdadChange();
    this.registerAnyosSiniestralidadChange();
    this.updateCapturaPerfilEdad();
    this.updateCapturaPerfilValor();
    this.perfilesEdad = this.cotizacionDataService.perfilesEdad;
    this.perfilesValor = this.cotizacionDataService.perfilesValor;
    // Disable controls
    // this.informacionNegocioForm.get('FechaFin').disable();
    if (this.currentUser.rol.roleId === 9) {
      this.informacionNegocioForm.get('EsNegocioDirecto').disable();
    }
  }

  // convert to null
  private zeroValue2Null(val: number) {
    return val === 0 ? null : val;
  }

  private updateForm() {
    const now = new Date();
    const now2 = now.setFullYear(now.getFullYear() + 1);
    const dti = this.model.fechaInicio === DEFAULT_DATETIME ? null : this.model.fechaInicio;
    const dtf = this.model.fechaFin === DEFAULT_DATETIME ? null : this.model.fechaFin;
    const periodoFacturacion = this.periodosFacturacion.find(x => x.codigoPeriodo === this.model.codigoPeriodoFacturacion);
    this.informacionNegocioForm.patchValue({
      DirectorComercial: this.model.directorComercial,
      NombreAseguradora: this.model.nombreAseguradora,
      FechaInicio: dti,
      FechaFin: dtf,
      Actividad: this.model.actividad,
      PeriodoFacturacion: this.zeroValue2Null(periodoFacturacion),
      TipoRiesgo: this.zeroValue2Null(this.model.codigoTipoRiesgo),
      TipoNegocio: this.zeroValue2Null(this.model.codigoTipoNegocio),
      // TipoContratacion: this.model.codigoTipoContratacion,
      Sector: this.zeroValue2Null(this.model.sector),
      TipoTasa1: this.zeroValue2Null(this.model.tipoTasa1),
      TipoTasa2: this.model.tipoTasa2,
      PorcentajeRetorno: this.model.porcentajeRetorno, // this.zeroValue2Null(this.model.porcentajeRetorno),
      PorcentajeOtrosGastos: this.model.porcentajeOtrosGastos,
      PorcentajeComision: this.zeroValue2Null(this.model.porcentajeComision),
      OtrosGastos: this.model.otrosGastos,
      PerfilEdad: this.zeroValue2Null(this.model.perfilEdad),
      PerfilValor: this.zeroValue2Null(this.model.perfilValor),
      EsNegocioDirecto: this.model.esNegocioDirecto,
      ConListaAsegurados: this.model.conListaAsegurados,
      AnyosSiniestralidad: this.zeroValue2Null(this.model.anyosSiniestralidad),
    });
    if (this.model.esNegocioDirecto) {
      this.OnEsNegocioDirectoEmitter.emit({ value: true });
      this.updatePorcentajeComisionFieldValidatiors(true);
    }

    this.updateCapturaPerfilValor();
    this.updateCapturaPerfilEdad();

    this.informacionNegocioForm.enable();

    if (this.currentUser.rol.roleId === 9) {
      this.informacionNegocioForm.get('EsNegocioDirecto').disable();
    }

    if (this.model.tipoTasa1 === 2) {
      this.informacionNegocioForm.get('ConListaAsegurados').disable();
    }

    if (this.readonly) {
      this.informacionNegocioForm.disable();
    }
  }

  private definePeriodoFacturacionObservable() {
    this.periodosFacturacion = this.cotizacionDataService.periodosFacturacion.filter(x => x.codigoPeriodo < 7);
    this.filteredPeriodosFacturacion = this.informacionNegocioForm.get('PeriodoFacturacion')
      .valueChanges
      .pipe(
        startWith(''),
        map(value => this._filterPeriodoFacturacion(value))
      );
  }

  private _filterPeriodoFacturacion(value: string): PeriodoFacturacion[] {
    const result = this.periodosFacturacion.filter(option => option.nombrePeriodo.toLowerCase().includes(value));
    return result;
  }

  private updateCapturaPerfilEdad() {
    const ctrlPerfilEdad = this.informacionNegocioForm.get('PerfilEdad');
    const ctrlTipoTasa1 = this.informacionNegocioForm.get('TipoTasa1');
    const ctrlTipoTasa2 = this.informacionNegocioForm.get('TipoTasa2');
    // require perfil edades si se define por subramo
    // y tiene lista de asegurados
    const esTasaRangoEdades = ctrlTipoTasa1.value === 3 || ctrlTipoTasa2.value === 3;
    this.showCapturaPerfilEdades =
      (this.cotizacionDataService.cotizacion.informacionBasica.subramo.siNoPerfilEdades && this.conListaAsegurados)
      || (esTasaRangoEdades && this.conListaAsegurados);
    if (this.showCapturaPerfilEdades) {
      ctrlPerfilEdad.setValidators([Validators.required]);
      ctrlPerfilEdad.updateValueAndValidity();
    } else {
      ctrlPerfilEdad.clearValidators();
      ctrlPerfilEdad.updateValueAndValidity();
    }
  }

  private updateCapturaPerfilValor() {
    const ctrlPerfilValor = this.informacionNegocioForm.get('PerfilValor');
    // require perfil valores si se define por subramo
    // y tiene lista de asegurados
    const ctrlTipoTasa1 = this.informacionNegocioForm.get('TipoTasa1');
    const ctrlTipoTasa2 = this.informacionNegocioForm.get('TipoTasa2');
    const esTasaRangoEdades = ctrlTipoTasa1.value === 3 || ctrlTipoTasa2.value === 3;
    this.showCapturaPerfilValores =
      (this.cotizacionDataService.cotizacion.informacionBasica.subramo.siNoPerfilValores && this.conListaAsegurados)
      || (esTasaRangoEdades && this.conListaAsegurados);
    if (this.showCapturaPerfilValores) {
      ctrlPerfilValor.setValidators(Validators.required);
      ctrlPerfilValor.updateValueAndValidity();
    } else {
      ctrlPerfilValor.clearValidators();
      ctrlPerfilValor.updateValueAndValidity();
    }
  }

  private updateCapturaAnyonSiniestralidad(): void {
    const ctrlAnyosSiniestralidad = this.informacionNegocioForm.get('AnyosSiniestralidad');
    if (this.conAnyosSiniestralidad) {
      ctrlAnyosSiniestralidad.setValidators([Validators.required]);
      ctrlAnyosSiniestralidad.updateValueAndValidity();
    } else {
      ctrlAnyosSiniestralidad.reset();
      ctrlAnyosSiniestralidad.clearValidators();
      ctrlAnyosSiniestralidad.updateValueAndValidity();
    }
  }

  private registerTipoNegocioChange() {
    this.informacionNegocioForm.get('TipoNegocio')
      .valueChanges
      .subscribe(selection => {
        if (!selection || selection === '' || selection === 0) { return; }
        const codigoTipoNegocio = selection;
        this.loadTiposContratacion(codigoTipoNegocio);
      });
  }

  private registerSectorChange() {
    this.informacionNegocioForm.get('Sector')
      .valueChanges
      .subscribe(selection => {
        if (selection) {
          this.cotizacionDataService.loadAmparos(selection);
          const codigoRamo = this.cotizacionDataService.cotizacion.informacionBasica.codigoRamo;
          const codigoSubramo = this.cotizacionDataService.cotizacion.informacionBasica.codigoSubramo;
          const codigoSector = selection;
          this.loadTasasSector(codigoRamo, codigoSubramo, codigoSector);
        }
      });
  }

  private registerAnyosSiniestralidadChange(): void {
    this.informacionNegocioForm.get('AnyosSiniestralidad').valueChanges.subscribe(anyos => {
      this.OnAnyosSiniestralidad.emit(Number(anyos));
    });
  }

  private loadTiposContratacion(codigoTipoNegocio: number) {
    this.personasReaderService.getTiposContratacionxNegocio(codigoTipoNegocio)
      .subscribe((response) => {
        
        this.tiposContratacion = response;
        if (this.cotizacionDataService.isEdit) {
          this.informacionNegocioForm.get('TipoContratacion').setValue(this.model.codigoTipoContratacion);
        }
      });
  }

  private loadTasasSector(codigoRamo: number, codigoSubramo: number, codigoSector: number) {
    this.personasReaderService.getTasas(codigoRamo, codigoSubramo, codigoSector)
      .subscribe((response) => {
        
        this.tiposTasa1 = response;
        if (this.cotizacionDataService.isEdit) {
          this.tiposTasa1 = Object.assign([], response);
        }
      });
  }

  private registerConListaAseguradosChange() {
    this.informacionNegocioForm.get('ConListaAsegurados')
      .valueChanges
      .subscribe((value) => {
        this.conListaAsegurados = value;
        this.updateCapturaPerfilEdad();
        this.updateCapturaPerfilValor();
      });
  }

  private registerPerfilEdadChange() {
    this.informacionNegocioForm.get('PerfilEdad')
      .valueChanges
      .subscribe((value) => {

      });
  }

  private loadDirectoresComerciales() {
    const cotizacion = this.cotizacionDataService.cotizacion;
    const ctrl = this.informacionNegocioForm.get('DirectorComercial');
    this.personasReaderService.getDirectoresComerciales(cotizacion.informacionBasica.codigoSucursal)
      .subscribe(res => {
        this.directoresComerciales = res;
        // ctrl.setValue(res[0]);
        if (this.model.usuarioDirectorComercial) {
          const selected = res.find(x => x.loginUsuario === this.model.usuarioDirectorComercial);
          if (selected) {
            ctrl.setValue(selected);
          }
        }
        if (res.length === 0) {
          ctrl.setValidators([]);
          ctrl.updateValueAndValidity();
          this.directoresComerciales.push({ loginUsuario: 'SIN DIRECTOR', nombreUsuario: 'SIN DIRECTOR', emailUsuario: '' });
          const m = this.directoresComerciales[0];
          ctrl.setValue(m);
          ctrl.disable();
        }
        // if (res.length === 1) {
        //   ctrl.disable();
        // }
      });
  }

  private loadTasas() {
    this.tiposTasa = this.cotizacionDataService.tasas;
    const ds = Object.assign([], this.tiposTasa);
    this.tiposTasa1 = ds;
    const ds2 = Object.assign([], this.tiposTasa);
    this.tiposTasa2 = ds2;
  }

  private registerPorcentajeOtrosGastosChange() {
    this.informacionNegocioForm.get('PorcentajeOtrosGastos')
      .valueChanges
      .subscribe(value => {
        this.showOtrosGastos = (value > 0);
        const ctrlOtrosGastos = this.informacionNegocioForm.get('OtrosGastos');
        if (this.showOtrosGastos) {
          ctrlOtrosGastos.setValidators([Validators.required]);
        } else {
          ctrlOtrosGastos.clearValidators();
        }
        ctrlOtrosGastos.updateValueAndValidity();
      });
  }

  private registerFechaInicioChange() {
    this.informacionNegocioForm.get('FechaInicio')
      .valueChanges
      .subscribe((selection) => {
        const ctrlPeriodoFacturacion = this.informacionNegocioForm.get('PeriodoFacturacion');
        const ctrlFechaInicio = this.informacionNegocioForm.get('FechaInicio');
        const ctrlFechaFin = this.informacionNegocioForm.get('FechaFin');
        if (selection) {
          const periodoFacturacion = ctrlPeriodoFacturacion.value;
          const fechaFin = moment(ctrlFechaInicio.value);
          ctrlFechaFin.setValue(fechaFin.add(1, 'year').toDate());
          ctrlFechaFin.clearValidators();
          ctrlFechaFin.enable();
        } else {
          ctrlFechaFin.disable();
          ctrlFechaFin.clearValidators();
          ctrlFechaFin.setValue(null);
        }
      });
  }

  private registerFechaFinChange() {
    this.informacionNegocioForm.get('FechaFin')
      .valueChanges
      .subscribe((selection) => {
        const ctrlFechaInicio = this.informacionNegocioForm.get('FechaInicio');
        const ctrlFechaFin = this.informacionNegocioForm.get('FechaFin');
        const fechaInicio: Date = ctrlFechaInicio.value;
        const fechaFin: Date = ctrlFechaFin.value;
        if (fechaInicio && fechaFin && fechaInicio > selection) {
          ctrlFechaFin.setErrors({ gtInitial: true });
        }
      });
  }


  private registerPeriodoFacturacionChange() {
    this.informacionNegocioForm.get('PeriodoFacturacion')
      .valueChanges
      .subscribe((selection) => {
        const ctrlFechaInicio = this.informacionNegocioForm.get('FechaInicio');
        const ctrlFechaFin = this.informacionNegocioForm.get('FechaFin');
        const fechaInicio: Date = ctrlFechaInicio.value || new Date();
        const fechaFin = new Date(fechaInicio.toDateString());
        switch (selection.nombrePeriodo) {
          case 'ANUAL':
            fechaFin.setFullYear(fechaFin.getFullYear() + 1);
            break;
          case 'SEMESTRAL':
            fechaFin.setMonth(fechaFin.getMonth() + 6);
            break;
          case 'CUATRIMESTRAL':
            fechaFin.setMonth(fechaFin.getMonth() + 4);
            break;
          case 'TRIMESTRAL':
            fechaFin.setMonth(fechaFin.getMonth() + 3);
            break;
          case 'BIMENSUAL':
            fechaFin.setMonth(fechaFin.getMonth() + 2);
            break;
          case 'MENSUAL':
            fechaFin.setMonth(fechaFin.getMonth() + 1);
            break;

          default:
            break;
        }
      });
  }

  private registerTasa1Change() {
    const ctrlConListaAsegurados = this.informacionNegocioForm.get('ConListaAsegurados');
    const ctrlTipoTasa2 = this.informacionNegocioForm.get('TipoTasa2');
    this.informacionNegocioForm.get('TipoTasa1')
      .valueChanges
      .subscribe((value) => {
        if (!value) { return; }
        ctrlConListaAsegurados.enable();
        if (value === 5 || ctrlTipoTasa2.value === 5) {
          this.conAnyosSiniestralidad = true;
        } else {
          this.conAnyosSiniestralidad = false;
        }
        if (!this.model.conListaAsegurados) {
          if (value !== 2) {
            ctrlConListaAsegurados.setValue(false);
          }
        }
        const tasa1 = this.tiposTasa.find(x => x.codigoTasa === value);
        ctrlTipoTasa2.enable();
        this.tiposTasa2 = Object.assign([], this.tiposTasa.filter(x => x.codigoTasa === tasa1.codigoTasaDos));
        // let criteria = value !== 5 ? i => i.codigoTasa === 5 : i => i.codigoTasa !== 5;
        // this.tiposTasa2 = this.tiposTasa2.filter(criteria);
        // si es tipo Tasa por edad de cada asegurado
        if (value !== '' && !this.conListaAsegurados) {
          this.OnApliqueSiniestralidadEmitter.emit({ value1: value, value2: ctrlTipoTasa2.value });
          ctrlConListaAsegurados.setValue((value === 2), { emitEvent: false });
          if (value === 2) {
            ctrlConListaAsegurados.disable();
          }

          this.conListaAsegurados = ctrlConListaAsegurados.value;
        }

        if (this.model.tipoTasa1 !== value) {
          if (value === 5) {
            ctrlTipoTasa2.setValue(tasa1.codigoTasaDos, { emitEvent: false });
          }
        }

        if (tasa1.codigoTasaDos === 0 || value === 5) { // || value === 5 || value !== 1) {
          // dirty solution
          setTimeout(() => {
            ctrlTipoTasa2.disable();
          }, 300);
        }

        this.updateCapturaAnyonSiniestralidad();
        this.updateCapturaPerfilEdad();
        this.updateCapturaPerfilValor();
      });
  }

  private registerTasa2Change() {
    const ctrlConListaAsegurados = this.informacionNegocioForm.get('ConListaAsegurados');
    const ctrlTipoTasa1 = this.informacionNegocioForm.get('TipoTasa1');
    const ctrlTipoTasa2 = this.informacionNegocioForm.get('TipoTasa2');
    this.informacionNegocioForm.get('TipoTasa2')
      .valueChanges
      .subscribe((value) => {
        ctrlTipoTasa2.setValue(value, { emitEvent: false });
        if (value === 5 || ctrlTipoTasa1.value === 5) {
          this.conAnyosSiniestralidad = true;
        } else {
          this.conAnyosSiniestralidad = false;
        }
        if (value === '' && this.model.tipoTasa2 > 0) {
          ctrlTipoTasa2.setValue(this.model.tipoTasa2);
        }

        if (value !== '' && !this.conListaAsegurados) {
          this.OnApliqueSiniestralidadEmitter.emit({ value1: ctrlTipoTasa1.value, value2: value });
          ctrlConListaAsegurados.setValue((value === 2), { emitEvent: false });
        }

        this.updateCapturaAnyonSiniestralidad();
        this.updateCapturaPerfilEdad();
        this.updateCapturaPerfilValor();
      });
  }

  private registerEsNegocioDirectoChange() {
    this.informacionNegocioForm.get('EsNegocioDirecto')
      .valueChanges
      .subscribe((value) => {
        this.OnEsNegocioDirectoEmitter.emit({ value });
        this.updatePorcentajeComisionFieldValidatiors(value);
      });
  }

  private updatePorcentajeComisionFieldValidatiors(esNegocioDirecto: boolean) {
    this.showPorcentajeComision = !esNegocioDirecto;
    const ctrl = this.informacionNegocioForm.get('PorcentajeComision');
    if (!esNegocioDirecto) {
      ctrl.setValidators([Validators.required, Validators.max(100), Validators.min(1)]);
    } else {
      ctrl.clearValidators();
      ctrl.reset();
    }

    ctrl.updateValueAndValidity();
  }

  private compareInformacionNegocioForm(): boolean {
    return true;
  }

  async continue() {
    const promise = new Promise((resolve, reject) => {
      this.submitted = true;
      const isValidForm: boolean = !this.informacionNegocioForm.invalid;
      const hasChanges = this.compareInformacionNegocioForm();
      if (isValidForm && hasChanges) {
        const toast = this.notificationService.showToast('Guardando Información de Negocio', 0);
        const model = this.informacionNegocioForm.getRawValue();
        this.informacionNegocioWriter.SaveInformacionNegocio(model)
          .subscribe(res => {
            if (res) {
              this.cotizacionDataService.updateInformacionNegocio(model);
              toast.dismiss();
              resolve(true);
            }
          });
      }
    });
    const result = await promise;
    return result;
  }

  displayFn(field, item): string {
    return item[field];
  }
}
