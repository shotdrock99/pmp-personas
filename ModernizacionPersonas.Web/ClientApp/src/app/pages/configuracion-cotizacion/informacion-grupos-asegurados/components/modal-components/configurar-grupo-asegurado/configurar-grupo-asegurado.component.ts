

import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatStepper, MatStep } from '@angular/material/stepper';
import { NgSelectComponent } from '@ng-select/ng-select';
import { Observable, of, BehaviorSubject } from 'rxjs';
import { GrupoAseguradoExtended, TipoSumaAsegurada, OpcionesUsadas } from 'src/app/models';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { GrupoAseguradoMapperService } from 'src/app/services/grupo-asegurado-mapper.service';
import { GrupoAseguradoWizardService } from 'src/app/services/grupo-asegurado-wizard.service';
import { EdadesAmparosValidator } from 'src/app/services/grupos-asegurados/edades-amparos.validator';
import { ValoresAseguradosValidator } from 'src/app/services/grupos-asegurados/valores-asegurados.validator';

@Component({
  selector: 'app-configurar-grupo-asegurado',
  templateUrl: './configurar-grupo-asegurado.component.html',
  styleUrls: ['./configurar-grupo-asegurado.component.scss']
})
export class ConfigurarGrupoAseguradoComponent implements OnInit {
  readonly: boolean;
  constructor(
    private grupoAseguradoMapperService: GrupoAseguradoMapperService,
    public wizardService: GrupoAseguradoWizardService,
    private valoresAseguradosValidator: ValoresAseguradosValidator,
    private edadesAmparosvalidator: EdadesAmparosValidator,
    private cotizacionPersistenceService: CotizacionPersistenceService,
    public dialogRef: MatDialogRef<ConfigurarGrupoAseguradoComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.readonly = data.readonly;
  }

  @ViewChild('amparos', { static: true })
  cmbAmparos: NgSelectComponent;

  @ViewChild('stepper', { static: true })
  stepper: MatStepper;

  submitted = false;
  model: GrupoAseguradoExtended;
  nombreGrupo: string;
  tipoSumaAsegurada: TipoSumaAsegurada;
  amparosSeleccionados = [];
  amparosCollection$: Observable<any[]>;
  opcionesUsadasSubject = new BehaviorSubject<OpcionesUsadas>({
    opcion1: false,
    opcion2: false,
    opcion3: false
  });

  get form() {
    return this.wizardService.formGroup;
  }

  get amparosArray() {
    return this.wizardService.formGroup.get('amparos');
  }

  get valoresAseguradosArray() {
    return this.wizardService.formGroup.get('valoresAsegurados');
  }

  get edadesAmparosArray() {
    return this.wizardService.formGroup.get('edadesAmparos');
  }

  get aseguradosArray() {
    return this.wizardService.formGroup.get('asegurados');
  }

  get selectedAmparos() {
    return this.wizardService.amparosArray.value;
  }

  get isValoresAseguradosValid() {
    return this.wizardService.isValoresAseguradosValid;
  }

  get isEdadesAmparosValid() {
    return this.wizardService.isEdadesAmparosValid;
  }

  get isAseguradosValid() {
    return this.wizardService.isAseguradosValid;
  }

  ngOnInit(): void {
    this.tipoSumaAsegurada = this.data.grupoAsegurado.tipoSumaAsegurada;
    this.nombreGrupo = this.data.grupoAsegurado.nombreGrupoAsegurado;

    this.loadAmparos();

    this.wizardService.init(this.data.grupoAsegurado, this.data.grupoAseguradoInfo);
    this.model = this.wizardService.grupoAsegurado;

    this.selectAmparos();
    this.registerStepperSelectionChange();
  }

  private registerStepperSelectionChange() {
    this.stepper.selectionChange.subscribe((change: StepperSelectionChangeArgs) => {
      if (change.previouslySelectedIndex < change.selectedIndex) {
        // ask step
        // si el paso actual es Valores Asegurados (idx 1)
        if (change.previouslySelectedIndex === 1) {
          this.wizardService.isValoresAseguradosValid = this.valoresAseguradosValidator.validate();
        }
        if (change.previouslySelectedIndex === 2) {
          if (this.wizardService.grupoAsegurado.tipoSumaAsegurada.codigoTipoSumaAsegurada === 1) {
            const opcionesvaloresAsegurados = this.wizardService.formGroup.value.valoresAsegurados[0].opciones;
            this.opcionesUsadasSubject.next({
              opcion1: opcionesvaloresAsegurados[0].valorOption.rawValue > 0,
              opcion2: opcionesvaloresAsegurados[1].valorOption.rawValue > 0,
              opcion3: opcionesvaloresAsegurados[2].valorOption.rawValue > 0
            });
          }
        }
      }
    });
  }

  private async loadAmparos() {
    const amparos = this.cotizacionPersistenceService.amparos;
    this.amparosCollection$ = this.getAmparosObservable(amparos);
    this.cmbAmparos.focus();
  }

  private getAmparosObservable(response: any): Observable<any[]> {
    return of(response);
  }

  private selectAmparos() {
    const arr = [];
    this.model.amparos.forEach(amparo => {
      arr.push(amparo.codigoAmparo);
    });

    this.amparosSeleccionados = arr;
  }

  onRemoveAmparos(args: any) {
    this.wizardService.removeAmparo(args.value);
  }

  onAddAmparos(amparo: any) {
    this.wizardService.pushAmparo(amparo);
  }

  onNoClick(): void {
    this.valoresAseguradosValidator.removeAllErrors();
    this.dialogRef.close();
  }

  onStepChange(event) {
  }

  onCompleteClick() {
    
    const isValid = this.edadesAmparosvalidator.validate();
    if (isValid) {
      this.submitted = true;
      const isValidForm: boolean = !this.wizardService.formGroup.invalid;
      if (isValidForm) {
        const formJSON = this.wizardService.formGroup.getRawValue();
        const args = this.grupoAseguradoMapperService.map(this.model, formJSON);
        this.valoresAseguradosValidator.removeAllErrors();
        this.dialogRef.close(args);
      }
    }
  }

  dismiss() {
    this.valoresAseguradosValidator.removeAllErrors();
    this.dialogRef.close();
  }
}

export interface StepperSelectionChangeArgs {
  previouslySelectedIndex: number;
  previouslySelectedStep: MatStep;
  selectedIndex: number;
  selectedStep: MatStep;
}
