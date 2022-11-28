import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { MatAutocompleteTrigger } from '@angular/material/autocomplete';
import { Observable } from 'rxjs';
import { map, startWith, switchAll, tap, debounceTime, switchMap, distinctUntilChanged } from 'rxjs/operators';
import { Departamento, Municipio, Pais, Persona } from 'src/app/models';
import { Tomador } from 'src/app/models/tomador';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { DatostomadorWriterService } from 'src/app/services/datostomador-writer.service';
import { NavegacionService } from 'src/app/services/navegacion.service';
import { ParametrizacionReaderService } from 'src/app/services/parametrizacion-reader.service';
import { PersonasReaderService } from 'src/app/services/personas-reader.service';
import { RequireMatch } from 'src/app/shared/functions/requireMatch';
import { InputRestrictionService } from 'src/app/shared/services/input-restriction.service';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { AlertDialogPreventCloseComponent } from 'src/app/shared/components/alert-dialog/alert-dialog.preventClose.component';
import { MatDialog } from '@angular/material/dialog';
import { AlertDialogComponent, AlertDialogModel } from 'src/app/shared/components/alert-dialog';

@Component({
  selector: 'app-informacion-tomador',
  templateUrl: './informacion-tomador.component.html',
  styleUrls: ['./informacion-tomador.component.scss']
})
export class InformacionTomadorComponent implements OnInit {
  Departamento = new FormControl('', Validators.required);
  Municipio = new FormControl('', Validators.required);
  constructor(
    private formBuilder: FormBuilder,
    private notificationService: NotificationService,
    private navigationService: NavegacionService,
    private cotizacionDataService: CotizacionPersistenceService,
    private datosTomadorWriterService: DatostomadorWriterService,
    private parametrizacionReaderService: ParametrizacionReaderService,
    private personasReaderService: PersonasReaderService,
    private inputRestrictionService: InputRestrictionService,
    private dialog: MatDialog,

  ) { }

  @Input() model: Tomador;
  @Input() readonly: boolean;
  @Input() version: number;

  @ViewChild('departamento', { static: true })
  departamentoInput: ElementRef;

  @ViewChild(MatAutocompleteTrigger)
  departamentoInputTrigger: MatAutocompleteTrigger;

  @ViewChild('nombre')
  nombreInput: ElementRef;

  @ViewChild('razonsocial')
  razonInput: ElementRef;

  private defaultPais = {
    codigoPais: 1,
    nombrePais: 'COLOMBIA',
    codigoRunt: 90
  };

  indexView = 1;
  datosBasicosTomadorForm: FormGroup;
  submitted = false;
  completed = false;
  esPersonaNatural: boolean;
  originalModel: Tomador;

  tipoDocumentos: any[] = [];
  paises: any[] = [];
  departamentos: Departamento[] = [];
  municipios: any[] = [];
  actividadEconomicaList: any[] = [];

  filteredPaises: Observable<Pais[]>;
  filteredDepartamentos: Observable<Departamento[]>;
  filteredMunicipios: Observable<Municipio[]>;

  // convenience getter for easy access to form fields
  get form() { return this.datosBasicosTomadorForm.controls; }

  ngOnInit() {
    this.navigationService.subscribe({
      indexView: this.indexView,
      continuePromise: () => {
        return this.continue();
      },
      initializePromise: () => {
        this.initializeSection();
      }
    });

    // copy original model to later comparison
    this.originalModel = Tomador.copy(this.model);

    this.defineForm();

    if (this.cotizacionDataService.isEdit) {
      this.initializeSection();
      this.updateForm(this.model);
      if (this.version > 1) {
        this.datosBasicosTomadorForm.disable({ emitEvent: false });
        this.Departamento.disable();
        this.Municipio.disable();
        this.enableControls(['NombreContacto', 'TelefonoContacto2']);
      }
    }
  }
  /*private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();

    return this.options.filter(option => option.toLowerCase().includes(filterValue));
  }*/
  
  getOptionText(option) {
    debugger;
    return option.nombreMunicipio;
  }
  private defineForm() {
    // definicion del formulario
    this.datosBasicosTomadorForm = this.formBuilder.group({
      TipoDocumento: ['', [Validators.required, RequireMatch]],
      NumeroDocumento: ['', [Validators.required, Validators.maxLength(11), Validators.pattern('^[0-9]*$')]],
      RazonSocial: ['', [Validators.required, Validators.maxLength(100)]],
      PrimerNombre: ['', [Validators.required, Validators.maxLength(30)]],
      SegundoNombre: ['', [Validators.maxLength(30)]],
      PrimerApellido: ['', [Validators.required, Validators.maxLength(100)]],
      SegundoApellido: ['', [Validators.maxLength(30)]],
      ActividadEconomica: [''],
      Pais: [{
        value: this.defaultPais,
        disabled: true
      }],
      Direccion: ['', [Validators.maxLength(60)]],
      Email: ['', [Validators.required, Validators.email, Validators.maxLength(60)]],
      //Telefono: ['',[Validators.maxLength(60)] ],
      NombreContacto: ['', [Validators.maxLength(100)]],
      TelefonoContacto1: ['',  [Validators.maxLength(60)]],
      TelefonoContacto2: ['']
    });

    this.datosBasicosTomadorForm.get('Pais').disable();
  }

  private initializeSection() {
    this.defineTiposDocumento();
    // this.definePaisesObservable();
    this.defineDepartamentosObservable();

    this.registerDepartamentoChange();
    this.registerTipoDocumentoChange();
    this.configureForm();
  }

  private defineTiposDocumento() {
    this.tipoDocumentos = this.cotizacionDataService.tiposDocumento;
  }

  private registerTipoDocumentoChange() {
    this.datosBasicosTomadorForm.get('TipoDocumento')
      .valueChanges
      .subscribe(selection => {
        if (!selection) { return; }
        this.esPersonaNatural = selection.tipoPersona === 'F';
        // update validators
        this.datosBasicosTomadorForm.get('NumeroDocumento').reset();
        this.updateNombresValidators();
        this.fetchTomador();
      });
  }

  private updateNombresValidators() {
    const ctrlRazonSocial = this.datosBasicosTomadorForm.get('RazonSocial');
    const ctrlPrimerNombre = this.datosBasicosTomadorForm.get('PrimerNombre');
    const ctrlPrimerApellido = this.datosBasicosTomadorForm.get('PrimerApellido');
    if (!this.esPersonaNatural) {
      ctrlRazonSocial.setValidators([Validators.required, Validators.maxLength(100)]);
      ctrlRazonSocial.updateValueAndValidity();
      // clear validator
      ctrlPrimerNombre.clearValidators();
      ctrlPrimerNombre.updateValueAndValidity();
      ctrlPrimerApellido.clearValidators();
      ctrlPrimerApellido.updateValueAndValidity();
    } else {
      ctrlPrimerNombre.setValidators([Validators.required, Validators.maxLength(30)]);
      ctrlPrimerNombre.updateValueAndValidity();
      ctrlPrimerApellido.setValidators([Validators.required, Validators.maxLength(100)]);
      ctrlPrimerApellido.updateValueAndValidity();
      // clear validators
      ctrlRazonSocial.clearValidators();
      ctrlRazonSocial.updateValueAndValidity();
    }
  }

  private fetchTomador() {
    if (this.readonly) { return; }

    const controls = ['RazonSocial', 'PrimerNombre', 'SegundoNombre', 'PrimerApellido',
      'SegundoApellido', 'Direccion', 'TelefonoContacto1'];
    const controlsWithDocument = ['RazonSocial', 'PrimerNombre', 'SegundoNombre', 'PrimerApellido',
      'SegundoApellido', 'Direccion', 'TelefonoContacto1', 'TipoDocumento', 'NumeroDocumento'];
    const ctrlTipoDocumento = this.datosBasicosTomadorForm.get('TipoDocumento');
    const ctrlNumeroDocumento = this.datosBasicosTomadorForm.get('NumeroDocumento');
    const tipoDocumento = ctrlTipoDocumento.value;
    const numeroDocumento = ctrlNumeroDocumento.value;

    if (!tipoDocumento || !numeroDocumento) {
      this.enableControls(controls);
      this.clearForm(controls);
    } else {
      this.personasReaderService.consultarTomador(tipoDocumento.codigoTipoDocumento, numeroDocumento)
        .subscribe((response: any) => {
          if (response != null) {
            if (response.errorCode == undefined) {
              if (response) {
                this.model.numeroDocumento = response.NumeroDocumento;
                this.updateForm(response);
                this.disableControls(controls);
                this.departamentoInput.nativeElement.focus();
              } else {
                controls.push('Departamento', 'Municipio');
                this.clearForm(controls);
                this.enableControls(controls);
              }
            } else {
              if (response.status == 1) {
                this.personasReaderService.handleWarning(response.errorMessage);
                this.clearForm(controlsWithDocument);
                this.enableControls(controlsWithDocument);
              } else {
                console.log(response.errorMessage);
              }

            }
          }
        });
    }
  }

  private configureForm() {

  }

  private registerDepartamentoChange() {
    this.Departamento
      .valueChanges
      .subscribe((selection) => {
        if (selection !== '' && selection != undefined && selection != undefined) {
          this.loadMunicipios(selection.codigoDepartamento);
        }
      });
  }

  private loadMunicipios(codigoDepartamento: any) {
    if (codigoDepartamento != undefined) {
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
                map(value => this._filterMunicipios(value))
              );

            if (this.model.codigoMunicipio) {
              const municipio = response.find(x => x.codigoMunicipio === this.model.codigoMunicipio);
              const ctrl = this.Municipio;
              ctrl.setValue(municipio, { emitEvent: false });
            }
          }
        });
    }
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
        if (this.model.codigoDepartamento) {
          const departamento = response.find(x => x.codigoDepartamento === this.model.codigoDepartamento);
          this.Departamento.setValue(departamento, { emitEvent: true });
        }
      });
  }

  private definePaisesObservable() {
    this.parametrizacionReaderService.getPaises()
      .subscribe((response: Pais[]) => {
        this.paises = response;
        this.filteredPaises = this.datosBasicosTomadorForm.get('Pais')
          .valueChanges
          .pipe(
            startWith(''),
            map(value => this._filterPais(value))
          );
        const pais = this.paises.find(x => x.nombrePais === 'COLOMBIA');
        const ctrlPais = this.datosBasicosTomadorForm.get('Pais');
        ctrlPais.setValue(pais);
        ctrlPais.disable();
      });
  }

  private _filterPais(value: string): Pais[] {
    const filterValue = value;
    return this.paises.filter(option => option.nombrePais.toLowerCase().includes(filterValue));
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

  private updateForm(tomador) {
    const controls = ['RazonSocial', 'PrimerNombre', 'SegundoNombre', 'PrimerApellido',
      'SegundoApellido', 'Direccion', 'TelefonoContacto1'];
    if (tomador === null) {
      this.enableControls(controls);
      this.clearForm(controls);
      if (this.esPersonaNatural) {
        this.nombreInput.nativeElement.focus();
      } else {
        this.razonInput.nativeElement.focus();
      }
      this.departamentoInputTrigger.closePanel();
    } else {
      this.fillForm(tomador);
      this.disableControls(controls);
      this.updateNombresValidators();
      this.datosBasicosTomadorForm.get('RazonSocial').enable();
    }
    if (this.readonly) {
      this.datosBasicosTomadorForm.disable({ emitEvent: false });
    }
  }

  private fillForm(tomador: any) {
    debugger;
    const codigoTipoDocumento = tomador.tipoDocumento || tomador.codigoTipoDocumento;
    const numeroDocumento = tomador.numeroDocumento || tomador.documento;
    const tipoDocumento = this.tipoDocumentos.find(x => x.codigoTipoDocumento === codigoTipoDocumento);
    const correo = this.getCorreoTomador(tomador);
    this.datosBasicosTomadorForm.patchValue({
      TipoDocumento: tipoDocumento,
      NumeroDocumento: numeroDocumento,
      RazonSocial: tomador.primerApellido,
      PrimerNombre: tomador.primerNombre,
      SegundoNombre: tomador.segundoNombre,
      PrimerApellido: tomador.primerApellido,
      SegundoApellido: tomador.segundoApellido,
      Direccion: Persona.getDireccion(tomador),
      Email: correo,
      //Telefono: Persona.getTelefono(tomador),
      NombreContacto: tomador.nombreContacto,
      TelefonoContacto1: Persona.getTelefono(tomador),
      TelefonoContacto2: tomador.telefonoContacto2
    }, { emitEvent: false });
    if (tipoDocumento) {
      this.esPersonaNatural = tipoDocumento.tipoPersona === 'F';
    }
    if (tomador.direccion[0].codigoDepartamento != undefined) {
      this.model.codigoMunicipio = tomador.direccion[0].codigoMunicipio;
      this.model.codigoDepartamento = tomador.direccion[0].codigoDepartamento;
      const departamento = this.departamentos.find(x => x.codigoDepartamento === tomador.direccion[0].codigoDepartamento);
      const ctrl = this.datosBasicosTomadorForm.get('Departamento');
      ctrl.setValue(departamento, { emitEvent: true });
      this.loadMunicipios(tomador.direccion[0].codigoDepartamento);
      
      
    }
  }

  private getCorreoTomador(tomador: any) {
    if (tomador.correo) {
      if (tomador.correo.length > 0) {
        return tomador.correo[0].correoElectronico;
      }
    }

    return tomador.email;
  }

  private clearForm(controls: string[]) {
    controls.forEach((controlName) => {
      const input = this.datosBasicosTomadorForm.get(controlName);
      input.setValue('');
    });
  }

  private disableControls(controls: string[]) {

    controls.forEach((controlName) => {
      this.datosBasicosTomadorForm.get(controlName).disable();
    });
  }

  private enableControls(controls: string[]) {
    controls.forEach((controlName) => {
      this.datosBasicosTomadorForm.get(controlName).enable();
    });
  }

  onNumeroDocumentoBlur(event) {
    this.fetchTomador();
  }

  onNumeroDocumentoKeyDown(e) {
    const ctrlTipoDocumento = this.datosBasicosTomadorForm.get('TipoDocumento');
    const tipoDocumento = ctrlTipoDocumento.value;
    if (!tipoDocumento.siNoAlfanumerico) {
      this.inputRestrictionService.validateIntegerOnly(e);
    } else {

      this.inputRestrictionService.validateNoSpecialChars(e);
    }
  }

  async continue() {
    // if (this.submitted) return;
    this.submitted = true;
    const promise = new Promise((resolve, reject) => {
      const isValidForm: boolean = this.datosBasicosTomadorForm.valid;
      if (!this.originalModel.isEqual(this.model)) {
        if (isValidForm) {
          const toast = this.notificationService.showToast('Guardando Tomador', 0);
          let model = this.datosBasicosTomadorForm.getRawValue();
          model.Departamento = this.Departamento.value;
          model.Municipio = this.Municipio.value;
          debugger;
          this.datosTomadorWriterService.SaveTomador(model)
            .subscribe(res => {
              if (res) {
                toast.dismiss();
                this.submitted = false;
                resolve(true);
              }
            });
        }
      }
    });
    const result = await promise;
    return result;
    // return isValidForm && this.completed;
  }

  displayFn(field: any, item: any): string {
    if (item != undefined) {
      return item[field];
    }

  }

}
