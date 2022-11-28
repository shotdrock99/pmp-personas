import { AlertDialogModel, AlertDialogComponent } from 'src/app/shared/components/alert-dialog';
import { Component, OnInit, ViewChild, ElementRef, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { TipoDocumento } from 'src/app/models';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatAutocompleteTrigger } from '@angular/material/autocomplete';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { PersonasReaderService } from 'src/app/services/personas-reader.service';
import { InputRestrictionService } from 'src/app/shared/services/input-restriction.service';
import { RequireMatch } from 'src/app/shared/functions/requireMatch';
import { InputRestrictionType } from 'src/app/shared/input-restriction-type.enum';
import { startWith, map } from 'rxjs/operators';

@Component({
  selector: 'app-configurar-intermediario',
  templateUrl: './configurar-intermediario.component.html',
  styleUrls: ['./configurar-intermediario.component.scss']
})
export class ConfigurarIntermediarioComponent implements OnInit {
  submitted: boolean;
  title = !this.data.isEdit ? 'Agregar Intermediario' : 'Editar Intermediario';
  tipoDocumentos: any[] = [];
  filteredTipoDocs: Observable<TipoDocumento[]>;
  intermediarioForm: FormGroup;
  isEdit = this.data.isEdit;
  isFirstVersion = this.data.isFirstVersion;
  esPersonaNatural = true;
  sumPorcentajeParticipacion: number = this.data.sumPorcentajeParticipacion;
  allowedPorcentajeParticipacion = (100 - this.sumPorcentajeParticipacion);
  intermediario: any;
  showNoExisteClave = false;
  showNoExisteNumDoc = false;
  searchByClaveDone = false;
  numDocInputRestrictionType = '';
  loadingConfig = {
    clave: false,
    numeroDocumento: false
  };

  validarIntermediarioFn = this.data.validarIntermediario;

  @ViewChild('porcentajeParticipacion', { static: true })
  porcentajeParticipacionInput: ElementRef;

  @ViewChild('clave', { static: true })
  claveInput: ElementRef;

  @ViewChild('trigger', { read: MatAutocompleteTrigger })
  trigger: MatAutocompleteTrigger;
  formWithErrors: boolean;
  readonly: boolean;

  constructor(
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private cotizacionService: CotizacionPersistenceService,
    private personasReaderService: PersonasReaderService,
    private inputRestrictionService: InputRestrictionService,
    public dialogRef: MatDialogRef<ConfigurarIntermediarioComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.readonly = data.readonly;
  }

  get form() { return this.intermediarioForm.controls; }

  cotizacion = this.cotizacionService.cotizacion;

  ngOnInit(): void {
    this.intermediarioForm = this.formBuilder.group({
      Codigo: null,
      Clave: [''],
      TipoDocumento: ['', [Validators.required, RequireMatch]],
      NumeroDocumento: ['', [Validators.required, Validators.maxLength(11)]],
      RazonSocial: ['', [Validators.maxLength(100)]],
      PrimerNombre: ['', [Validators.required, Validators.maxLength(30)]],
      SegundoNombre: ['', [Validators.maxLength(30)]],
      PrimerApellido: ['', [Validators.required, Validators.maxLength(100)]],
      SegundoApellido: ['', [Validators.maxLength(30)]],
      Email: ['', [Validators.required, Validators.email]],
      PorcentajeParticipacion: ['', [Validators.required, Validators.max(100), Validators.min(1)]],
      TipoPersona: ['']
    });

    this.defineTipoDocumentosObservable();

    this.registerTipoDocumentoChange();
    // this.registerNumeroDocumentoChange();
    this.registerPorcentajeParticipacionChange();
    // this.registerClaveChange();
    this.registerPrimerNombreChange();
  }

  private registerPorcentajeParticipacionChange() {
    const ctrl = this.intermediarioForm.get('PorcentajeParticipacion');
    ctrl.valueChanges
      .subscribe((value: number) => {
        if (value > this.allowedPorcentajeParticipacion) {
          ctrl.setErrors({ invalid: true });
        }
      });
  }

  private registerTipoDocumentoChange() {
    this.intermediarioForm.get('TipoDocumento')
      .valueChanges
      .subscribe(selection => {
        if (!selection) { return; }
        this.esPersonaNatural = selection.tipoPersona === 'F';

        const ctrlNumeroDocumento = this.intermediarioForm.get('NumeroDocumento');
        const numeroDocumento = ctrlNumeroDocumento.value;
        // this.searchByNumeroDocumento(numeroDocumento);

        this.updateValidators();
      });
  }

  private updateValidators() {
    const ctrlRazonSocial = this.intermediarioForm.get('RazonSocial');
    const ctrlPrimerNombre = this.intermediarioForm.get('PrimerNombre');
    const ctrlPrimerApellido = this.intermediarioForm.get('PrimerApellido');
    const ctrlTipoDocumento = this.intermediarioForm.get('TipoDocumento');
    const ctrlNumeroDocumento = this.intermediarioForm.get('NumeroDocumento');
    const tipoDocumento = ctrlTipoDocumento.value;

    const patternValidator = Validators.pattern('^[0-9]*$');
    if (!tipoDocumento.siNoAlfanumerico) {
      this.numDocInputRestrictionType = InputRestrictionType.NUMERIC_ONLY;
      ctrlNumeroDocumento.setValidators([Validators.required, patternValidator]);
    } else {
      this.numDocInputRestrictionType = InputRestrictionType.NO_SPECIAL_CHARACTERS;
      ctrlNumeroDocumento.setValidators(null);
    }

    ctrlNumeroDocumento.updateValueAndValidity();
    if (!this.esPersonaNatural) {
      ctrlRazonSocial.setValidators([Validators.required]);
      ctrlRazonSocial.updateValueAndValidity();
      // clear validator
      ctrlPrimerNombre.clearValidators();
      ctrlPrimerNombre.updateValueAndValidity();
      ctrlPrimerApellido.clearValidators();
      ctrlPrimerApellido.updateValueAndValidity();
    } else {
      ctrlPrimerNombre.setValidators([Validators.required]);
      ctrlPrimerNombre.updateValueAndValidity();
      ctrlPrimerApellido.setValidators([Validators.required]);
      ctrlPrimerApellido.updateValueAndValidity();

      // clear validators
      ctrlRazonSocial.clearValidators();
      ctrlRazonSocial.updateValueAndValidity();
    }
  }

  private registerClaveChange() {
    const ctrlClave = this.intermediarioForm.get('Clave');
    ctrlClave
      .valueChanges
      .subscribe(value => {
        this.searchByClave(value);
      });
  }

  private searchByClave(value: any) {
    if (this.intermediario && this.intermediario.codigoAgente.toString() === value) { return; }
    const ctrlClave = this.intermediarioForm.get('Clave');
    if (!value) {
      this.clearForm();
      return;
    }

    this.loadingConfig.clave = true;

    const codigoSucursal = this.cotizacion.informacionBasica.sucursal.codigoSucursal;
    this.personasReaderService.consultarIntermediario(codigoSucursal, value)
      .subscribe(response => {
        if(response.errorCode == undefined){
          this.loadingConfig.clave = false;
          // si existe intermediario en DB
          if (response.idPersona !== 0) {
            if (this.data.model.Email != response.correoElectronico && this.data.model.Email != "" && this.data.model.Email != null) {
              response.correoElectronico = this.data.model.Email;
            }
            this.intermediario = response;
            this.searchByClaveDone = true;
            this.updateFormGroup();

            ctrlClave.setErrors(null);

            if (response.codigoEstado > 0) {
              if (response.codigoEstado === 99) {
                ctrlClave.setErrors({ unregistered: true });
              } else {
                ctrlClave.setErrors({ inactive: true });
              }
            }

            this.showNoExisteClave = false;
          } else {
            ctrlClave.setErrors({ showNoExisteClave: true });
            this.showNoExisteClave = true;
            const controls = ['TipoDocumento', 'NumeroDocumento', 'RazonSocial', 'PrimerNombre',
              'SegundoNombre', 'PrimerApellido', 'SegundoApellido'];

            this.clearForm(controls);
            // this.claveInput.nativeElement.focus();
            // this.trigger.closePanel();
          }

          this.validarIntermediario();
        }else{
          if(response.status == 1){
            this.personasReaderService.handleWarning(response.errorMessage);
            this.loadingConfig.clave = false;
            const controls = ['TipoDocumento', 'NumeroDocumento', 'RazonSocial', 'PrimerNombre',
            'SegundoNombre', 'PrimerApellido', 'SegundoApellido'];
  
          this.clearForm(controls);
          }else{
            console.log(response.errorMessage);
          }
          
        }
      });
  }

  private registerNumeroDocumentoChange() {
    this.intermediarioForm.get('NumeroDocumento')
      .valueChanges
      .subscribe(value => {
        // this.searchByNumeroDocumento(value);
      });
  }

  private searchByNumeroDocumento(value: any) {
    if (value === '') {
      // this.clearForm();
      return;
    }

    const ctrlNumDoc = this.intermediarioForm.get('NumeroDocumento');
    const ctrlTipoDoc = this.intermediarioForm.get('TipoDocumento');
    const tipoDocumento = ctrlTipoDoc.value;

    if (this.searchByClaveDone && this.intermediario) {
      return;
    }

    const codigoSucursal = this.cotizacion.informacionBasica.sucursal.codigoSucursal;
    const codigoTipoDocumento = ctrlTipoDoc.value.codigoTipoDocumento;
    if (!codigoTipoDocumento) { return; }
    this.loadingConfig.numeroDocumento = true;
    this.personasReaderService.consultarIntermediarioPorDocumento(codigoSucursal, codigoTipoDocumento, value)
      .subscribe(response => {
        this.loadingConfig.numeroDocumento = false;
        // si existe un intermediario en DB
        if (response.idPersona !== 0) {
          this.formWithErrors = false;
          this.intermediario = response;
          this.updateFormGroup();
          if (response.codigoEstado > 0) {
            if (response.codigoEstado === 99) {
              ctrlNumDoc.setErrors({ unregistered: true });
            } else {
              ctrlNumDoc.setErrors({ inactive: true });
            }

            this.formWithErrors = true;
          }
        } else {
          this.intermediario = {
            codigoTipoDocumento: tipoDocumento.codigoTipoDocumento,
            numeroDocumento: value
          };

          this.showNoExisteNumDoc = true;
          this.showNoExisteClave = false;
          this.resetClave();
        }

        this.validarIntermediario();
      });
  }

  private resetClave() {
    const ctrlClave = this.intermediarioForm.get('Clave');
    ctrlClave.reset();
    ctrlClave.disable();
    ctrlClave.setValidators([]);
    ctrlClave.updateValueAndValidity();
  }

  private validarIntermediario() {
    if (this.intermediario) {
      const existeIntermediario = this.validarIntermediarioFn(this.intermediario.codigoTipoDocumento, this.intermediario.numeroDocumento);
      if (existeIntermediario) {
        this.showIntermediarioAgregadoMessage();
      }
    }
  }

  private updateForm() {
    const tipoDocumento = this.getTipoDocumentoByName(this.data.model.TipoDocumento);
    this.data.model.TipoDocumento = tipoDocumento;
    if (this.data.model.Nombre != undefined && this.data.model.PrimerApellido == undefined) {
      this.data.model.PrimerApellido = this.data.model.Nombre;
    }
    this.intermediarioForm.patchValue({
      Codigo: this.data.model.Codigo,
      Clave: this.data.model.Clave,
      TipoDocumento: tipoDocumento,
      NumeroDocumento: this.data.model.NumeroDocumento,
      RazonSocial: this.data.model.PrimerApellido,
      PrimerNombre: this.data.model.PrimerNombre,
      SegundoNombre: this.data.model.SegundoNombre,
      PrimerApellido: this.data.model.PrimerApellido,
      SegundoApellido: this.data.model.SegundoApellido,
      Email: this.data.model.Email,
      PorcentajeParticipacion: this.data.model.PorcentajeParticipacion,
      TipoPersona: ''
    });

    const controls = ['Clave', 'TipoDocumento', 'NumeroDocumento', 'RazonSocial', 'PrimerNombre',
      'SegundoNombre', 'PrimerApellido', 'SegundoApellido'];
    this.disableControls(controls);

    if (this.readonly) {
      this.intermediarioForm.disable();
    } else {
      this.intermediarioForm.enable();
    }
    if (this.isFirstVersion) {
      this.intermediarioForm.disable();
      this.enableControls(['Email']);
    }
  }

  private getTipoDocumentoByCodigo(codigoTipoDocumento: any): any {
    if (codigoTipoDocumento) {
      return this.tipoDocumentos.find(x => x.codigoTipoDocumento === codigoTipoDocumento);
    }

    return '';
  }

  private getTipoDocumentoByName(tipoDocumento: any) {
    if (tipoDocumento) {
      return this.tipoDocumentos.find(x => x.codigoTipoDocumento === tipoDocumento.codigoTipoDocumento);
    }

    return '';
  }

  private defineTipoDocumentosObservable() {
    this.personasReaderService.getTiposDocumento()
      .subscribe((response: TipoDocumento[]) => {
        this.tipoDocumentos = response;
        this.filteredTipoDocs = this.intermediarioForm.get('TipoDocumento')
          .valueChanges
          .pipe(
            startWith(''),
            map(value => this._filterTipoDocs(value))
          );

        if (this.isEdit) {
          this.updateForm();
        }
      });
  }

  private _filterTipoDocs(value: string): TipoDocumento[] {
    const filterValue = value;
    return this.tipoDocumentos.filter(option => option.nombreReducido.toLowerCase().includes(filterValue));
  }

  private updateFormGroup() {
    const controls = ['TipoDocumento', 'NumeroDocumento', 'RazonSocial', 'PrimerNombre',
      'SegundoNombre', 'PrimerApellido', 'SegundoApellido'];
    const tipoDoc = this.getTipoDocumentoByCodigo(this.intermediario.codigoTipoDocumento);
    this.esPersonaNatural = tipoDoc.tipoPersona === 'F';
    this.intermediarioForm.patchValue({
      TipoDocumento: tipoDoc,
      NumeroDocumento: this.intermediario.numeroDocumento,
      RazonSocial: this.intermediario.primerApellido,
      PrimerNombre: this.intermediario.primerNombre,
      SegundoNombre: this.intermediario.segundoNombre,
      PrimerApellido: this.intermediario.primerApellido,
      SegundoApellido: this.intermediario.segundoApellido,
      Email: this.intermediario.email || this.intermediario.correoElectronico,
      TipoPersona: tipoDoc.tipoPersona
    });

    this.disableControls(controls);
    this.porcentajeParticipacionInput.nativeElement.focus();
  }

  private showIntermediarioAgregadoMessage() {
    const message = `El registro ya fue agregado a la lista de intermediarios`;
    const dialogData = new AlertDialogModel('Información', message);
    const dialogRef = this.dialog.open(AlertDialogComponent, {
      maxWidth: '400px',
      data: dialogData
    });

    dialogRef.afterClosed().subscribe(result => {
      this.esPersonaNatural = true;
      this.updateValidators();
      this.clearForm();
    });
  }

  private registerPrimerNombreChange() {
    const ctrlPrimerNombre = this.intermediarioForm.get('PrimerNombre');
    ctrlPrimerNombre
      .valueChanges
      .subscribe(value => {
        this.showNoExisteNumDoc = false;
      });
  }

  private enableControls(controls: any[]) {
    controls.forEach(ctrlName => {
      this.intermediarioForm.controls[ctrlName].enable();
    });
  }

  private disableControls(controls: any[]) {
    controls.forEach(ctrlName => {
      this.intermediarioForm.controls[ctrlName].disable();
    });
  }

  onNumeroDocumentoKeyDown(e) {
    const ctrlTipoDocumento = this.intermediarioForm.get('TipoDocumento');
    const tipoDocumento = ctrlTipoDocumento.value;
    if (!tipoDocumento.siNoAlfanumerico) {
      this.inputRestrictionService.validateIntegerOnly(e);
    } else {
      this.inputRestrictionService.validateNoSpecialChars(e);
    }
  }

  onClaveBlur(e) {
    const value = this.intermediarioForm.get('Clave').value;
    if (value != null && value != 0) {
      this.searchByClave(value);
    }

  }

  onNumeroDocumentoBlur(e) {
    const value = this.intermediarioForm.get('NumeroDocumento').value;
    this.searchByNumeroDocumento(value);
  }

  clearForm(controls?: any): void {
    this.submitted = false;
    this.intermediarioForm.reset();
    controls = !controls ? ['Clave', 'TipoDocumento', 'NumeroDocumento', 'RazonSocial', 'PrimerNombre',
      'SegundoNombre', 'PrimerApellido', 'SegundoApellido', 'PorcentajeParticipacion'] : controls;
    controls.forEach(controlName => {
      this.intermediarioForm.get(controlName).setValue('');
    });

    if (false) {
      this.intermediarioForm.reset({
        TipoDocumento: '',
        NumeroDocumento: '',
        RazonSocial: '',
        PrimerNombre: '',
        SegundoNombre: '',
        PrimerApellido: '',
        SegundoApellido: ''
      });
    }

    this.intermediario = null;
    this.searchByClaveDone = false;
    this.showNoExisteClave = false;
    this.showNoExisteNumDoc = false;

    this.enableControls(controls);
  }

  onAcceptClick(): void {
    this.submitted = true;
    const isValidForm: boolean = !this.intermediarioForm.invalid;
    if (isValidForm && !this.formWithErrors) {
      const args = this.intermediarioForm.getRawValue();
      if (args.TipoDocumento.tipoPersona === 'J') {
        args.PrimerApellido = args.RazonSocial;
      }
      this.data.model.PorcentajeParticipacion = this.intermediarioForm.get('PorcentajeParticipacion').value;
      this.dialogRef.close(args);
    }
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  displayFn(field, item): string {
    if (item === null) { return; }
    return item[field];
  }
}
