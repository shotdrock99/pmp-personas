import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators,FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { CotizacionState, Departamento, Municipio } from 'src/app/models';
import { CotizacionValidationResponse, CotizacionTransaction, CotizacionTasa } from 'src/app/models/cotizacion-authorization';
import { SlipClausula, SlipClausulaVariable } from 'src/app/models/slip-clausula';
import { GenerarSlipConfiguracionResponse, SlipConfiguracion } from 'src/app/models/slip-configuracion';
import {
  TasaSelectorComponent,
  TasaSelectorModel
} from 'src/app/pages/slip-cotizacion/table-templates/tasa-selector/tasa-selector.component';
import { ConfiguracionSlipReaderService } from 'src/app/services/configuracion-slip-reader.service';
import { ConfiguracionSlipWriterService } from 'src/app/services/configuracion-slip-writer.service';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { CotizacionReaderService } from 'src/app/services/cotizacion-reader.service';
import { CotizacionWriterService } from 'src/app/services/cotizacion-writer.service';
import { ParametrizacionReaderService } from 'src/app/services/parametrizacion-reader.service';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { PageToolbarBuilder } from 'src/app/shared/services/page-toolbar-builder';
import { PageToolbarConfig } from 'src/app/models/page-toolbar-item';
import { TransactionsReaderService } from 'src/app/services/transactions-reader.service';
import {
  AuthorizationTransactionsModalComponent
} from '../components/authorization-transactions-modal/authorization-transactions-modal.component';
import {
  InformacionAutorizacionesCotizacionComponent
} from '../components/informacion-autorizaciones-cotizacion/informacion-autorizaciones-cotizacion.component';
import { ConfiguracionSlipClausulasComponent } from './components/configuracion-slip-clausulas/configuracion-slip-clausulas.component';

@Component({
  selector: 'app-configuracion-slip-cotizacion',
  templateUrl: './configuracion-slip-cotizacion.component.html',
  styleUrls: ['./configuracion-slip-cotizacion.component.scss']
})
export class ConfiguracionSlipCotizacionComponent implements OnInit {
  transactions: CotizacionTransaction[];
  lastTransaction: CotizacionTransaction;
  readonly: boolean;
  lastTransactionCommentsMessage = '';
  disabledTabSubject = new BehaviorSubject<any>(null);
  Departamento = new FormControl('', Validators.required);
  DepartamentoTomador = new FormControl('', Validators.required);
  Municipio = new FormControl('', Validators.required);
  MunicipioTomador= new FormControl('', Validators.required);
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private toolbarBuilder: PageToolbarBuilder,
    private transactionsReader: TransactionsReaderService,
    private cotizacionDataService: CotizacionPersistenceService,
    private notificationService: NotificationService,
    private parametrizacionReaderService: ParametrizacionReaderService,
    private configuracionSlipReaderService: ConfiguracionSlipReaderService,
    private configuracionSlipWriterService: ConfiguracionSlipWriterService,
    private cotizacionReaderService: CotizacionReaderService,
    private cotizacionWriterService: CotizacionWriterService) { }

  @ViewChild('informacionAutorizacionesCotizacion')
  informacionAutorizacionesComponent: InformacionAutorizacionesCotizacionComponent;

  @ViewChild('clausulasForm')
  clausulasForm: ConfiguracionSlipClausulasComponent;

  codigoCotizacion: number;
  version: number;
  numeroCotizacion: string;
  cotizacionState: CotizacionState;
  submitted = false;
  pageLoaded = false;
  form: FormGroup;
  toolbarConfig: PageToolbarConfig;
  disabledContinue = true;

  departamentos: any[] = [];
  filteredDepartamentos: Observable<Departamento[]>;
  filteredDepartamentsPolicyholder: Observable<Departamento[]>;
  municipios: any[] = [];
  filteredMunicipios: Observable<Municipio[]>;
  filteredMunicipalitiesPolicyholder: Observable<Municipio[]>;
  mostrarNotificacionAutorizaciones = false;
  cotizacionApproved: boolean;
  approverUserName: string;
  showConfiguracionSlip = true;
  showButtonContinueToSlip: boolean;
  tomadorInfo: any;
  model: SlipConfiguracion = null;

  get cotizacion() { return this.cotizacionDataService.cotizacion; }
  get formControls() { return this.form.controls; }
  get tomadorForm() { return this.form.get('tomador') as FormGroup; }
  get tomadorControls() { return this.tomadorForm.controls; }

  showConfiguracionAmparos = false;
  alertConfig: any = {
    className: 'alert-success',
    icon: 'done',
    message: '',
    visible: false
  };

  private currentClauses: string;

  ngOnInit() {
    this.route.data.subscribe(res => {
      this.codigoCotizacion = res.data.codigoCotizacion;
      this.tomadorInfo = res.data.informacionBasicaTomador;
      this.version = res.data.version;
      this.numeroCotizacion = res.data.numero;
      this.cotizacionState = res.data.estado;
      this.readonly = res.data.readonly;
      this.cotizacionDataService.setCotizacionState(res.data.estado);
      this.updateAuthorizationSectionView(res.data.estado);
      this.loadAuthControls();
      this.loadTransactions();
      this.consultarConfiguracion();
    });
    this.defineForm();
    this.defineDepartamentosObservable();
    this.defineDepartamentsPolicyholderObservable();
    this.registerDepartamentoChange();
    this.registerDepartamentoTomadorChange();
    this.registerformOnChange();
    this.initializeToolbar();
    this.showButtonContinueToSlip = (this.cotizacion.estado >= CotizacionState.Sent
      || this.cotizacion.estado === CotizacionState.PendingAuthorization
      || this.cotizacion.estado === CotizacionState.RefusedAuthorization) ? false : true;
    setTimeout(() => {
      if (this.model != null) {
        debugger;
        //this.currentClauses = JSON.stringify(this.model.clausulas.filter(x => x.activo));
      }
      
      //this.disabledContinue = false;
    }, 5000);
  }

  private updateAlertConfig() {
    this.alertConfig.visible = true;
    if (this.cotizacionState === CotizacionState.Lookover) {
      this.alertConfig.className = 'alert-warning';
      this.alertConfig.icon = 'swap_horiz';
      this.alertConfig.message = `La cotización se encuentra en estado <b>Devuelto para corrección</b> por el usuario <b>` +
        `${this.lastTransaction.codigoUsuario}</b>, se requiere corregir la información de la cotización.` +
        `A continuación encontrará mas detalles.`;
    } else if (this.cotizacionState === CotizacionState.ApprovedAuthorization) {
      if (this.transactions) {
        this.alertConfig.className = 'alert-success';
        this.alertConfig.icon = 'done';
        this.alertConfig.message = `Los controles de autorización fueron autorizados por <b>${this.lastTransaction.codigoUsuario}</b>.` +
          ` A continuación encontrará mas detalles.`;
      } else {
        this.alertConfig.visible = false;
      }
    } else if (this.cotizacionState === CotizacionState.RefusedAuthorization) {
      this.alertConfig.className = 'alert-danger';
      this.alertConfig.icon = 'clear';
      this.alertConfig.message = `Los controles de autorización fueron rechazados por <b>${this.lastTransaction.codigoUsuario}</b>.` +
        ` A continuación encontrará mas detalles.`;
    } else {
      this.alertConfig.visible = false;
    }
  }

  private loadTransactions() {
    const codigoCotizacion = this.cotizacionDataService.cotizacion.codigoCotizacion;
    const version = this.cotizacionDataService.cotizacion.version;
    this.transactionsReader.getAuthorizationTransactions(codigoCotizacion, version)
      .subscribe(response => {
        if (response.length > 0) {
          this.transactions = response;
          this.lastTransaction = this.transactions[this.transactions.length - 1];
          this.lastTransactionCommentsMessage = this.lastTransaction.comments.length > 0 ? this.lastTransaction.comments[0].message : '';
          this.updateAlertConfig();
        }
      });
  }

  private defineForm() {
    this.form = this.formBuilder.group({
      tomador: this.formBuilder.group({
        codigoTomador: [''],
        nombre: ['', [Validators.required, Validators.pattern]],
        direccion: [''],
        telefono: [''],
        email: ['', [Validators.required, Validators.email]],
      }),
      actividad: ['', [Validators.required]],
      diasValidezCotizacion: [30, [Validators.required, Validators.min(1), Validators.max(180)]],
      condiciones: ['']
    });

    if (this.readonly) {
      this.form.disable();
    }
    this.form.get('tomador').get('nombre').disable();
    this.form.get('actividad').disable();
  }

  private initializeToolbar() {
    const items = [{
      name: 'back',
      icon_path: 'home',
      label: '',
      tooltip: 'Volver a cotizaciones',
      isEnabled: true,
      fixed: true,
      onClick: () => {
        this.navigateToCotizaciones();
      }
    }, {
      name: 'save',
      icon_path: 'save',
      label: 'Guardar',
      tooltip: 'Guardar',
      isEnabled: true,
      fixed: true,
      onClick: () => {
        this.save();
      }
    }, {
      name: 'refresh',
      icon_path: 'refresh',
      label: 'Refrescar',
      tooltip: 'Refrescar',
      isEnabled: true,
      fixed: true,
      onClick: () => {
        this.refresh();
      }
    }];

    this.toolbarConfig = this.toolbarBuilder.build(items);
  }

  private navigateToCotizaciones() {
    this.router.navigate(['cotizaciones']);
  }

  private updateAuthorizationSectionView(estado: CotizacionState) {
    this.mostrarNotificacionAutorizaciones = (estado >= CotizacionState.Validated && estado <= CotizacionState.Lookover);
    this.cotizacionApproved = estado === CotizacionState.ApprovedAuthorization;
    this.approverUserName = '';
    if (!this.mostrarNotificacionAutorizaciones) {
      setTimeout(() => {
        this.disabledTabSubject.next({
          tabId: 4,
          disabled: false,
          state: estado
        });
      }, 6500);
    }
  }

  private defineConfiguracionAmparosVisibility() {
    this.showConfiguracionAmparos = this.model.amparos.filter(x => x.variables.length > 0).length > 0;
  }

  private consultarConfiguracion() {
    this.configuracionSlipReaderService.obtenerConfiguracion(this.codigoCotizacion)
      .subscribe((response: GenerarSlipConfiguracionResponse) => {
      
        this.model = response.data;
        this.currentClauses = JSON.stringify(this.model.clausulas.filter(x => x.activo));
        /*this.model.tomador.codigoTomador = this.tomadorInfo.codigoTomador;
        this.model.tomador.nombreTomadorSlip = this.tomadorInfo.tomadorSlip;
        this.model.tomador.nombre = this.tomadorInfo.nombreTomadorSlip;
        this.model.tomador.codigoCiudad = this.tomadorInfo.codigoMunicipio;
        this.model.tomador.codigoDepartamento = this.tomadorInfo.codigoDepartamento;
        this.model.tomador.direccion = this.tomadorInfo.direccion;
        this.model.tomador.telefono = this.tomadorInfo.telefono;*/
        this.patchForm();
        this.defineConfiguracionAmparosVisibility();
        this.pageLoaded = true;
        this.updateAuthorizationSectionView(response.cotizacionState);
        this.updateAlertConfig();
        this.disabledContinue = false;
      },
      (error) => {                  //Error callback
        /*const message = error.message;
        
        this.notificationService.dialog.closeAll();
        this.notificationService.showAlert(message);
        this.informacionAutorizacionesComponent.refresh();*/
        //resolve(false);
  
        //throw error;   //You can also throw the error to a global error handler
      });
  }

  private obtenerDepartamento(codigo: number) {
    if (codigo > 0) {
      return this.departamentos.find(x => x.codigoDepartamento === codigo);
    }
  }

  private obtenerCiudad(codigo: number) {
    if (codigo > 0) {
      return this.municipios.find(x => x.codigoMunicipio === codigo);
    }
  }

  private patchForm() {
    /*if (this.model.codigoDepartamento) {
      const departamento = this.departamentos.find(x => x.codigoDepartamento === this.model.codigoDepartamento);
      this.Departamento.setValue(departamento, { emitEvent: true });
    }*/
    
    this.form.patchValue({
      
      tomador: {
        codigoTomador: this.model.tomador.codigoTomador,
        nombre: this.model.tomador.nombre,
        direccion: this.model.tomador.direccion,
        telefono: this.model.tomador.telefono,
        email: this.model.tomador.email
      },
      actividad: this.model.actividad,
      diasValidezCotizacion: this.model.diasValidezCotizacion,
      condiciones: this.model.condiciones
    });
    this.form.get('actividad').disable();
    this.actualizarDepartamentoCiudad();
    setTimeout(() => {
      this.actualizarDepartamentoCiudadTomador();
    }, 1000);
  }

  validateDVC() {
    const ctrlDVC = this.form.get('diasValidezCotizacion').value;
    if (ctrlDVC > 180) {
      this.form.patchValue({ diasValidezCotizacion: 180 });
    } else if (ctrlDVC < 1) {
      this.form.patchValue({ diasValidezCotizacion: 1 });
    }
  }

  private actualizarDepartamentoCiudadTomador() {
    const departamentoTomador = this.obtenerDepartamento(this.model.tomador.codigoDepartamento);
    if (departamentoTomador) {
      this.DepartamentoTomador.setValue(departamentoTomador);
    }
    setTimeout(() => {
      const ciudadTomador = this.obtenerCiudad(this.model.tomador.codigoCiudad);
      if (ciudadTomador) {
        this.MunicipioTomador.setValue(ciudadTomador);
      }
    }, 1000);
  }

  private actualizarDepartamentoCiudad() {
    const departamento = this.obtenerDepartamento(this.model.codigoDepartamento);
    if (departamento) {
      this.Departamento.setValue(departamento);
    }
    setTimeout(() => {
      const ciudad = this.obtenerCiudad(this.model.codigoCiudad);
      if (ciudad) {
        this.Municipio.setValue(ciudad);
      }
    }, 1000);
  }

  private registerDepartamentoChange() {

    this.Departamento
      .valueChanges
      .subscribe((selection) => {
        if (selection) {
          this.Municipio.setValue("");
          this.loadMunicipios(selection.codigoDepartamento);
          //this.form.get('ciudad').setValue('');
        }
      });
  }

  private registerDepartamentoTomadorChange() {
    this.DepartamentoTomador
      .valueChanges
      .subscribe((selection) => {
        if (selection) {
          this.MunicipioTomador.setValue("");
          this.loadMunicipalitiesPolicyholder(selection.codigoDepartamento);
          //this.tomadorForm.get('ciudad').setValue('');
        }
      });
  }

  private registerformOnChange() {
    this.form.valueChanges.subscribe(() => {
      if (this.form.dirty || this.form.touched) {
        this.disabledTabSubject.next({
          tabId: 4,
          disabled: true,
          state: CotizacionState.Validated
        });
      }
    });
  }

  private loadMunicipios(codigoDepartamento: any) {
    if (!codigoDepartamento) { return; }
    this.parametrizacionReaderService.getMunicipiosPorDepartamento(codigoDepartamento)
      .subscribe((response: Municipio[]) => {
        if (response.length === 0) {
          const municipio = Municipio.create('No hay subramos disponibles.');
          this.municipios.push(municipio);
        } else {
          this.municipios = response;
          this.filteredMunicipios = this.Municipio
          .valueChanges
          .pipe(
            startWith(''),
            map(value => (typeof value === 'string' ? value : value.nombreMunicipio)),
            map(name => (name ? this._filterMunicipios(name) : this.municipios.slice())),
          );
          if (this.model.codigoCiudad) {
            const ciudad = this.municipios.find(x => x.codigoMunicipio === this.model.codigoCiudad);
            this.Municipio.setValue(ciudad, { emitEvent: true });
          }
        }
      });
  }

  private loadMunicipalitiesPolicyholder(codigoDepartamento: any) {
    if (!codigoDepartamento) { return; }
    this.parametrizacionReaderService.getMunicipiosPorDepartamento(codigoDepartamento)
      .subscribe((response: Municipio[]) => {
        if (response.length === 0) {
          const municipio = Municipio.create('No hay subramos disponibles.');
          this.municipios.push(municipio);
        } else {
          this.municipios = response;
          this.filteredMunicipalitiesPolicyholder = this.MunicipioTomador
          .valueChanges
          .pipe(
            startWith(''),
            map(value => (typeof value === 'string' ? value : value.nombreMunicipio)),
            map(name => (name ? this._filterMunicipios(name) : this.municipios.slice())),
          );
          if (this.model.tomador.codigoCiudad) {
            const ciudad = this.municipios.find(x => x.codigoMunicipio === this.model.tomador.codigoCiudad);
            this.MunicipioTomador.setValue(ciudad, { emitEvent: true });
          }
        }
      });
  }

  private defineDepartamentosObservable() {
    this.parametrizacionReaderService.getDepartamentos()
      .subscribe((response: Departamento[]) => {
        this.departamentos = response;
        this.filteredDepartamentos = this.Departamento
        .valueChanges
        .pipe(
          startWith(''),
          map(value => (typeof value === 'string' ? value : value.nombreDepartamento)),
          map(name => (name ? this._filterDepartamento(name) : this.departamentos.slice())),
        );
      });
  }

  private defineDepartamentsPolicyholderObservable() {
    this.parametrizacionReaderService.getDepartamentos()
      .subscribe((response: Departamento[]) => {
        this.departamentos = response;
        this.filteredDepartamentsPolicyholder = this.DepartamentoTomador
          .valueChanges
          .pipe(
            startWith(''),
            map(value => (typeof value === 'string' ? value : value.nombreDepartamento)),
            map(name => (name ? this._filterDepartamento(name) : this.departamentos.slice())),
          );
      });
  }

  private _filterDepartamento(value: string): Departamento[] {
    var filterValue = value != null ? value.toLowerCase() : "";
    if (value) {
      var temp =this.departamentos.filter(option => option.nombreDepartamento.toLowerCase().includes(filterValue));
      return temp;
    }
    return this.departamentos;
  }

  private _filterMunicipios(value: string): Municipio[] {
    var filterValue = value != null ? value.toLowerCase() : "";
    if (value) {
      var temp =this.municipios.filter(option => option.nombreMunicipio.toLowerCase().includes(filterValue));
      return temp;
    }
    return this.municipios;
    
  }

  private navigateToSlip() {
    this.router.navigate(['/cotizaciones', this.codigoCotizacion, 'slip', 'preview']);
  }

  private openTasaSelector(tasas: CotizacionTasa[]) {
    const dialogData = new TasaSelectorModel();
    dialogData.tasas = tasas;

    const dialogRef = this.dialog.open(TasaSelectorComponent, {
      maxWidth: '500px',
      data: dialogData
    });
    return dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const cotizacion = this.cotizacionDataService.cotizacion;
        const tasa = tasas.find((x: any) => x.key === result.value);
        this.updateSelectedTasa({
          codigoCotizacion: cotizacion.codigoCotizacion,
          tasaInfo: tasa
        });
      }
    });
  }

  private updateSelectedTasa(args: any) {
    this.configuracionSlipWriterService.updateTasaCotizacion(args)
      .subscribe(res => {
        this.router.navigate(['/cotizaciones', this.codigoCotizacion, 'slip', 'preview']);
      });

    // TODO update tasa seleccionada
    this.router.navigate(['/cotizaciones', this.codigoCotizacion, 'slip', 'preview']);
  }

  private validateCotizacion(flag: number) {
    return new Promise((resolve, reject) => {
      // si la cotizacion esta aprovada no realiza la validacion de la cotizacion
      // if (!this.cotizacionApproved) {
	 
		this.save();
	
	setTimeout(() => {
    ;
      this.cotizacionReaderService.validateCotizacion(this.codigoCotizacion, this.version, flag)
        .subscribe((res: CotizacionValidationResponse) => {
          const hasControls = res.validation && res.validation.authorizations.length > 0;
          this.mostrarNotificacionAutorizaciones = hasControls && res.cotizacionState < CotizacionState.ApprovedAuthorization;
          if (res.isValid) {
            // this.cotizacionDataService.setCotizacionState(CotizacionState.Validated);
            this.cotizacionApproved = false;
            this.pageLoaded = true;
            if (hasControls && res.cotizacionState < CotizacionState.ApprovedAuthorization) {
              const cotizacion = this.cotizacionDataService.cotizacion;
              cotizacion.authorizationInfo = res.validation;
              this.notificationService.showAlert('Existen controles de autorización que deben ser revisados antes de continuar.');
              this.disabledTabSubject.next({
                tabId: 4,
                disabled: true,
                state: res.cotizacionState
              });
              this.informacionAutorizacionesComponent.refresh();
              resolve(false);
              return;
            }

            // si no hay controles de validacion...
            if (res.cotizacionState < CotizacionState.Sent) {
              // si hay multiples tasas configuradas...
              // if (res.tasas && res.tasas.length > 1) {
              //   this.openTasaSelector(res.tasas);
              //   resolve(false);
              //   return;
              // } else {
              //   // si no hay controles de validacion ni multiples tasa seleccionadas continue a Slip
              //   resolve(true);
              // }
              resolve(true);
            }
          } else {
            const message = res.validation.validationMessage.message;
            this.notificationService.showAlert(message);
            resolve(false);
          }
        },
        (error) => {    
          
          //throw error;   //You can also throw the error to a global error handler
        });
		}, 8000);
    });
  }

  reloadValidation() {
    this.validateCotizacion(1);
  }

  openHistory() {
    const dialogRef = this.dialog.open(AuthorizationTransactionsModalComponent, {
      width: '400px',
      data: {
        codigoCotizacion: this.codigoCotizacion,
        version: 0
      }
    });

    return dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // TODO
      }
    });
  }

  onFormValuesChange(args) {
    if (args.target != undefined) {
      if (args.target.id == "TxtCondicionesEspeciales") {
        /*this.save();
        this.informacionAutorizacionesComponent.refresh();*/
        args.dirty = true;
      }
    }
    // this.save();
    if (args.dirty) {
      this.disabledTabSubject.next({
        tabId: 4,
        disabled: true,
        state: CotizacionState.Validated
      });
    }
  }

  triggerUnlockCotizacion() {
    this.cotizacionWriterService.unlockCotizacion(this.codigoCotizacion, this.version)
      .subscribe(res => res);
  }

  displayFn(field: any, item: any): string {
    return item[field];
  }

  save() {
    debugger;
    const f = this.form.getRawValue();
    // let m = this.model;
    const amparos = this.model.amparos.filter(x => x);
    var cla = this.model.clausulas.filter(x => x.activo);
    var temp = JSON.parse(this.currentClauses);
    cla.forEach(element => {
      temp.forEach(ele => {
          if(ele.nombre == element.nombre){
            ele.variables.forEach(ele2 => {
              element.variables.forEach(ele3 => {
                if(ele2.codigoVariable == ele3.codigoVariable){
                  ele3.errors = ele2.errors;
                }
              })
            });
            
          }
      })
    });
    f.ciudad = this.Municipio.value;
    f.departamento = this.Departamento.value;
    f.tomador.ciudad = this.MunicipioTomador.value;
    f.tomador.departamento = this.DepartamentoTomador.value;
    f.tomador.esIntermediario = this.model.tomador.esIntermediario;
    console.log("Nuevo" + JSON.stringify(cla));
    console.log("Original" + JSON.stringify(temp));
    const merged = {
      ...f, ...{
        amparos,
        clausulas: cla,
        snCambioClausulas: !(JSON.stringify(temp) === JSON.stringify(cla))
      }
    };
    merged.idCotizacion = this.codigoCotizacion;
    merged.condiciones = !merged.condiciones ? '' : merged.condiciones;
	;
	this.configuracionSlipWriterService.guardarConfiguracion(merged);	
    
  }

  continue() {
	  
      
	
    const isClausulasValid = this.clausulasForm.valid;
    if (this.form.invalid || !isClausulasValid) {
      this.submitted = true;
      this.notificationService.showAlert(
        'Existen valores en el formulario que no son validos, verifique los valores de las variables configuradas antes de continuar.');
      return;
    }
    if (this.model.clausulas.filter(x => x.activo).length === 0) {
      this.notificationService.showAlert('NO se puede continuar, no se ha registrado ninguna cláusula');
      return;
    }
    if (this.cotizacionState === CotizacionState.RefusedAuthorization) {
      this.notificationService.showAlert(
        'La cotización no ha sido autorizada, no se puede proceder al envío de Slip.');
      return;
    }
    this.pageLoaded = false;
    //const toast = this.notificationService.showToast('Validando información');
    // if (this.cotizacionState === CotizacionState.ApprovedAuthorization && !this.cotizacionDataService.hasMultiplesTasas) {
    //   this.navigateToSlip();
    //   return;
    // } else {
    //   this.save();
    // }
	
    //setTimeout(() => {
		
	//}, 3000);
    // validate cotizacion data
	
	
	
	
	this.validateCotizacion(2)
	  .then((res: boolean) => {
		if (res) {
		  // redirect if...
		  this.navigateToSlip();
		}

		//toast.dismiss();
	});
	
	
  }

  refresh() {
    this.pageLoaded = false;
    this.loadAuthControls();
    this.consultarConfiguracion();
  }

  loadAuthControls() {
    this.cotizacionReaderService.validateCotizacion(this.codigoCotizacion, this.version, 1)
      .subscribe((res: CotizacionValidationResponse) => {
        var hasControls = false;
        if(res.validation.authorizations != null){
         hasControls = res.validation && res.validation.authorizations.length > 0;
        }
         
        this.mostrarNotificacionAutorizaciones = hasControls && res.cotizacionState < CotizacionState.ApprovedAuthorization;
        if (res.isValid) {
          this.cotizacionApproved = false;
          if (hasControls && res.cotizacionState < CotizacionState.ApprovedAuthorization && this.informacionAutorizacionesComponent != undefined) {
            this.informacionAutorizacionesComponent.refresh();
          }
        }
      },
      (error) => {    
        /*;                          //Error callback
        const message = error.message;
        this.cotizacionApproved = false;
        this.pageLoaded=true;  
        this.notificationService.dialog.closeAll();
        this.notificationService.showAlert(message);
        this.informacionAutorizacionesComponent.refresh();*/
        
        //this.resolve(false);
  
        //throw error;   //You can also throw the error to a global error handler
      });
  }

  refreshClause(args) {
    this.consultarConfiguracion();
  }
}
