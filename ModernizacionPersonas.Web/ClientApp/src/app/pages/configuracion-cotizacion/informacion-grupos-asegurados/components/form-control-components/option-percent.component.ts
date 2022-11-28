import { Component, ElementRef, Input, OnInit, Renderer2, ViewChild } from '@angular/core';
import { FormArray, FormControl } from '@angular/forms';
import { Amparo, TipoSumaAsegurada, OpcionValorAsegurado, ValorAseguradoAmparo, ValoresAseguradosEvents } from 'src/app/models';
import { GrupoAseguradoWizardService } from 'src/app/services/grupo-asegurado-wizard.service';
import { ValoresAseguradosService } from 'src/app/services/valores-asegurados.service';
import { ValoresAseguradosValidator } from 'src/app/services/grupos-asegurados/valores-asegurados.validator';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';


@Component({
  selector: 'app-option-percent',
  template: `
  <div class="percent-container">
      <input #input type="text" [mask]="mask" suffix="%" (change)="onChange($event)"
        value="{{ value }}" [readonly]="readonly" [disabled]="disabled" /></div>`,
  styleUrls: ['./styles.scss']
})
export class OptionPercentComponent implements OnInit {

  baseCalculo = 0;
  esBasicoNoAdicional: boolean;
  basicoNoAdicionalRequired: boolean;
  optionName = 'porcentajeOption';

  model: any;
  disabled = false;
  // readonly: boolean = false;
  value = '';
  rawValue: number;
  // maxValue = 200;
  amparo: Amparo;
  tipoSumaAsegurada: TipoSumaAsegurada;
  mask = 'percent';
  patterns: { '0': { pattern: RegExp; }; };
  option: OpcionValorAsegurado;
  control: FormControl;
  maxValue: number;
  modalidadAmparo: number;
  SMMLV: number;

  constructor(
    private wizardService: GrupoAseguradoWizardService,
    private valoresAseguradosService: ValoresAseguradosService,
    private valoresAseguradosValidator: ValoresAseguradosValidator,
    private cotizacionDataService: CotizacionPersistenceService,
    private htmlEl: ElementRef,
    private rederer: Renderer2) { }

  @Input()
  dataSource: any[];

  @Input()
  index: number;

  @Input()
  readonly: boolean;

  @Input()
  element: ValorAseguradoAmparo;

  @Input()
  optionIndex: number;

  @ViewChild('input', { static: true })
  input: ElementRef;

  get controlForm() {
    const row = this.wizardService.valoresAseguradosArray.at(this.index);
    const arr = row.get('opciones') as FormArray;
    return arr.at(this.optionIndex - 1);
  }

  ngOnInit() {
    // this.control = <FormControl>this.controlForm.get('prima');

    this.option = this.element.opciones[this.optionIndex - 1];
    this.model = this.option.porcentajeOption;
    this.amparo = this.element.amparo;
    this.tipoSumaAsegurada = this.valoresAseguradosService.tipoSumaAsegurada;
    this.maxValue =
      this.element.amparo.modalidad.valores.length !== 0
        ? this.element.amparo.modalidad.valores[0].tope
        : undefined;
    this.modalidadAmparo = this.element.amparo.modalidad.codigo;
    this.SMMLV = this.cotizacionDataService.SMMLV;
    this.basicoNoAdicionalRequired = this.wizardService.basicoNoAdicionalRequired;
    this.esBasicoNoAdicional = this.amparo.siNoBasico && !this.amparo.siNoAdicional;
    this.baseCalculo = this.valoresAseguradosService.calcularAmparoBase(this.dataSource, this.optionIndex);

    this.rawValue = this.model.rawValue;
    this.value = this.rawValue > 0 ? `${this.rawValue}%` : '';

    // set default value for disabled property
    this.disabled = this.model.disabled;
    // this.readonly = false;

    // actualiza los valores del control al inicializar
    this.updateControl();
    this.setDisabledState();

    // suscribe el control a cambios del entorno
    this.subscribeOnBaseCalculoChange();
  }

  private updateControl() {
    if (this.amparo.codigoAmparo === 2 || this.amparo.codigoAmparo === 4 || this.amparo.codigoAmparo === 51) {
      this.mask = '999';
      this.patterns = { 0: { pattern: new RegExp('\([0-9]|[1-8][0-9]|9[0-9]|1[0-9]{2}|200)') } };
    }

    this.validate();
  }

  private setDisabledState(change?: any) {
    this.baseCalculo = this.valoresAseguradosService.calcularAmparoBase(this.dataSource, this.optionIndex);
    this.disabled = this.model.disabled;
    // si el cambio se emitio por edición
    if (change) {
      // si el basico del calculo es mayor a 0
      // y el amparo es un basico adicional
      // y el amparo captura porcentaje
      if (this.baseCalculo > 0 && !this.esBasicoNoAdicional && this.amparo.siNoPorcentajeBasico) {
        this.disabled = false;
      }
    }

    // si capturar valor de amparo base para calculo es requerido
    if (this.basicoNoAdicionalRequired) {
      // si el basico del calculo es 0 y el amparo es un basico adicional
      if (this.baseCalculo === 0 && !this.esBasicoNoAdicional) {
        this.disabled = true;
        this.reset();
      }
    }
  }

  private subscribeOnBaseCalculoChange() {
    if (!this.esBasicoNoAdicional) {
      this.valoresAseguradosService
        .subscribe({
          event: ValoresAseguradosEvents.BASE_CALCULO_CHANGE,
          optionIndex: this.optionIndex
        }, (change) => {
          this.setDisabledState(change);
          // this.validate();
        });
    }
  }

  private validate() {
    // valide si captura porcentaje basico
    if (this.amparo.siNoPorcentajeBasico) {
      const opcion = this.dataSource.map(x => {
        const maxValue: any[] = [undefined, undefined];
        maxValue[0] = x.amparo.modalidad.valores.length !== 0 ?
          ((x.amparo.modalidad.codigo === 5) ? this.SMMLV * x.amparo.modalidad.valores[0].tope :
            x.amparo.modalidad.valores[0].tope)  : undefined;
        if (this.element.amparo.codigoAmparo === x.amparo.codigoAmparo) {
          x.calculoBasePorValorAsegurado = true;
        }
        if (x.amparo.modalidad.codigo === 4) {
          maxValue[1] = x.amparo.modalidad.valores[1].tope;
        }
        return {
          amparo: x.amparo,
          valor: x.opciones[this.optionIndex - 1],
          maxValue,
          calculoBasePorValorAsegurado: x.calculoBasePorValorAsegurado,
        };
      });
      const isValidOption = this.valoresAseguradosValidator.validateOption({
        tipoSumaAsegurada: this.tipoSumaAsegurada,
        optionIndex: this.optionIndex,
        opcion,
        element: this.element,
      });

    }
  }

  private reset() {
    this.value = '';
    this.rawValue = 0;
    // this.disabled = true;

    this.updateControl();
  }

  private emitValueChange() {
    this.valoresAseguradosService
      .emit(ValoresAseguradosEvents.PERCENT_VALUE_CHANGE, {
        amparo: this.amparo,
        optionIndex: this.optionIndex,
        opcion: this.model
      });
  }

  private updateElement() {
    // update element
    this.model.value = this.value;
    this.model.rawValue = this.rawValue;
    this.model.disabled = this.disabled;

    this.option.porcentajeOption = this.model;
  }

  private focusNextControl() {
    if (this.esBasicoNoAdicional && this.rawValue > 0) {
      setTimeout(() => {
        const table = document.getElementById('tableValoresAsegurados');
        const inputs = table.querySelectorAll('input:not([disabled])');
        (inputs[1] as HTMLElement).focus();
      });
    }
  }

  onChange(e) {
    
    this.value = e.target.value;
    const rawValue = Number(this.value.replace(/\D+/g, ''));
    this.rawValue = rawValue;
    if (this.maxValue && rawValue > this.maxValue) {
      this.rawValue = this.maxValue;
      this.value = `${this.rawValue}%`;
    }

    this.updateElement();
    e.target.value = this.value;

    this.emitValueChange();
    this.validate();
    this.focusNextControl();
  }
}
