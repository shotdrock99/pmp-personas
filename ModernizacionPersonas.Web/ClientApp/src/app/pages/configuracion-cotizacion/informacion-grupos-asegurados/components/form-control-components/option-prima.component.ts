import { CurrencyPipe } from '@angular/common';
import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormControl } from '@angular/forms';
import { Amparo, TipoSumaAsegurada, OpcionValorAsegurado, ValorAseguradoAmparo, ValoresAseguradosEvents } from 'src/app/models';
import { GrupoAseguradoWizardService } from 'src/app/services/grupo-asegurado-wizard.service';
import { ValoresAseguradosService } from 'src/app/services/valores-asegurados.service';
import { ValoresAseguradosValidator } from 'src/app/services/grupos-asegurados/valores-asegurados.validator';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { formatNumber } from '@angular/common';

@Component({
  selector: 'app-option-prima',
  template: `
  <div class="asistencia-container">
      <input #input type="text" (blur)="onChange($event)" mask="separator.2" prefix="$" thousandSeparator=","
        value="{{ value }}" [readonly]="readonly" [disabled]="disabled"/></div>`,
  styleUrls: ['./styles.scss']
})
export class OptionPrimaComponent implements OnInit {

  model: any;
  disabled = false;
  // readonly: boolean = false;
  value = '';
  rawValue: number;
  amparo: Amparo;
  tipoSumaAsegurada: TipoSumaAsegurada;
  option: OpcionValorAsegurado;
  control: FormControl;
  maxValue: number;

  baseCalculo = 0;
  basicoNoAdicionalRequired: boolean;
  optionName = 'primaOption';

  currencyPipe = new CurrencyPipe('en-US');
  modalidadAmparo: number;
  SMMLV: number;

  constructor(
    private wizardService: GrupoAseguradoWizardService,
    private valoresAseguradosService: ValoresAseguradosService,
    private valoresAseguradosValidator: ValoresAseguradosValidator,
    private cotizacionDataService: CotizacionPersistenceService,) { }

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
    this.model = this.option.primaOption;
    this.amparo = this.element.amparo;
    this.tipoSumaAsegurada = this.valoresAseguradosService.tipoSumaAsegurada;
    this.maxValue =
      this.element.amparo.modalidad.valores.length !== 0
        ? this.element.amparo.modalidad.valores[0].tope
        : undefined;
    this.modalidadAmparo = this.element.amparo.modalidad.codigo;
    this.SMMLV = this.cotizacionDataService.SMMLV;
    this.basicoNoAdicionalRequired = this.wizardService.basicoNoAdicionalRequired;
    this.baseCalculo = this.valoresAseguradosService.calcularAmparoBase(this.dataSource, this.optionIndex);

    this.rawValue = this.model.rawValue;
    this.value = this.rawValue > 0 ? this.currencyPipe.transform(this.rawValue, 'USD', 'symbol', '0.0-2') : '';
    // this.readonly = false;

    this.updateControl();
    this.setDisabledState();
    this.subscribeOnBaseCalculoChange();
  }

  private updateControl() {
    this.validate();
  }

  private setDisabledState(change?: any) {
    this.baseCalculo = this.valoresAseguradosService.calcularAmparoBase(this.dataSource, this.optionIndex);
    this.disabled = this.model.disabled;
    // si el cambio se emitio por edición
    if (change) {
      // si el basico del calculo es mayor a 0 y el amparo es un basico adicional
      if (this.baseCalculo > 0) {
        this.disabled = false;
      }
    }

    // si capturar valor de amparo base para calculo es requerido
    if (this.basicoNoAdicionalRequired) {
      // si el basico del calculo es 0 y el amparo es un basico adicional
      if (this.baseCalculo === 0) {
        this.disabled = true;
        this.reset();
      }
    }
  }

  private subscribeOnBaseCalculoChange() {
    this.valoresAseguradosService
      .subscribe({
        event: ValoresAseguradosEvents.BASE_CALCULO_CHANGE,
        optionIndex: this.optionIndex
      }, (change) => {
        this.setDisabledState(change);
        // this.validate();
      });
  }

  private validate() {
    // valide si es Asistencia
    if (this.amparo.codigoGrupoAmparo === 3) {
      const opcion = this.dataSource.map(x => {
        const maxValue: any[] = [undefined, undefined];
        maxValue[0] = x.amparo.modalidad.valores.length !== 0 ?
          ((x.amparo.modalidad.codigo === 5) ? this.SMMLV * x.amparo.modalidad.valores[0].tope :
            x.amparo.modalidad.valores[0].tope) : undefined;
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

    this.updateElement();
  }

  private emitValueChange() {
    this.valoresAseguradosService
      .emit(ValoresAseguradosEvents.PRIMA_VALUE_CHANGE, {
        amparo: this.amparo,
        optionIndex: this.optionIndex,
        opcion: this.model
      });
  }

  onChange(e) {
    
    if (e.target.value !== '') {
      this.value = e.target.value;
      this.rawValue = Number(this.value.replace('$', '')/*.replace(/\D+/g, '')*/);
      if (Number.isNaN(this.rawValue)) {
        var temp = this.value.replace('$', '').replace(',','');
        this.rawValue = +parseFloat(temp).toFixed(2);
      }
      if (this.maxValue && this.rawValue > this.maxValue) {
        this.rawValue = this.maxValue;
        this.value = this.currencyPipe.transform(this.rawValue, 'USD', 'symbol', '0.0-2');
      }

      e.target.value = this.value;

      this.updateElement();

      this.emitValueChange();
    }

    this.validate();
  }

  private updateElement() {
    // update element
    this.model.value = this.value;
    this.model.rawValue = this.rawValue;
    this.model.disabled = this.disabled;

    this.option.primaOption = this.model;
  }
}
