import { CurrencyPipe } from '@angular/common';
import { Component, Input, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Amparo, OpcionValorAsegurado, TipoSumaAsegurada, ValorAseguradoAmparo, ValoresAseguradosEvents } from 'src/app/models';
import { ValoresAseguradosValidator } from 'src/app/services/grupos-asegurados/valores-asegurados.validator';
import { ValoresAseguradosService } from 'src/app/services/valores-asegurados.service';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { GrupoAseguradoWizardService } from 'src/app/services/grupo-asegurado-wizard.service';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';

@Component({
  selector: 'app-option-daily-value',
  template: `<div class="currency-container">
            <input #input type="text" [disabled]="disabled" [mask]="mask" prefix="$" thousandSeparator=","
            (change)="onChange($event)" value="{{ value }}"
            [readonly]="readonly"/> </div>`,
  styleUrls: ['./styles.scss'],
})
export class OptionDailyValueComponent implements OnInit {

  model: any;
  disabled: boolean;
  value = '';
  rawValue: number;
  option: OpcionValorAsegurado;
  mask = 'separator.2';
  amparo: Amparo;
  maxValue: number;
  tipoSumaAsegurada: TipoSumaAsegurada;
  currencyPipe = new CurrencyPipe('en-US');
  baseCalculo: any;
  esBasicoNoAdicional: boolean;
  basicoNoAdicionalRequired: boolean;
  modalidadAmparo: number;
  SMMLV: number;

  constructor(
    private wizardService: GrupoAseguradoWizardService,
    private valoresAseguradosService: ValoresAseguradosService,
    private valoresAseguradosValidator: ValoresAseguradosValidator,
    private cotizacionDataService: CotizacionPersistenceService,
    private notificationService: NotificationService ) {}

  @Input() dataSource: any[];
  @Input() element: ValorAseguradoAmparo;
  @Input() optionIndex: number;
  @Input() readonly: boolean;
  @ViewChild('input', { static: true })
  input: ElementRef;

  ngOnInit() {
    this.option = this.element.opciones[this.optionIndex - 1];
    this.model = this.option.valorDiarioOption;
    this.amparo = this.element.amparo;
    this.maxValue = this.element.amparo.modalidad.valores[0].tope;
    this.disabled = this.model.disabled;
    this.tipoSumaAsegurada = this.valoresAseguradosService.tipoSumaAsegurada;
    this.rawValue = this.model.rawValue;
    this.value = this.rawValue > 0 ? this.currencyPipe.transform(this.rawValue, 'USD', 'symbol', '0.0-0') : '';
    this.esBasicoNoAdicional = this.amparo.siNoBasico && !this.amparo.siNoAdicional;
    this.basicoNoAdicionalRequired = this.wizardService.basicoNoAdicionalRequired;
    this.modalidadAmparo = this.element.amparo.modalidad.codigo;
    this.SMMLV = this.cotizacionDataService.SMMLV;
    this.setDisabledState();
    this.subscribeOnBaseCalculoChange();
  }

  private setDisabledState(change?: any) {
    this.baseCalculo = this.valoresAseguradosService.calcularAmparoBase(this.dataSource, this.optionIndex);
    this.disabled = this.model.disabled;
    if (change) {
      // si el basico del calculo es mayor a 0
      // y el amparo es un basico adicional
      // y el amparo no captura porcentaje
      if (this.baseCalculo > 0 && !this.esBasicoNoAdicional && !this.amparo.siNoPorcentajeBasico) {
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

  onChange(e) {
    this.value = e.target.value;
    this.rawValue = Number(this.value.replace(/\D+/g, ''));
    if (this.rawValue > this.maxValue) {
      // this.rawValue = this.maxValue;
      // this.value = this.currencyPipe.transform(this.rawValue, 'USD', 'symbol', '0.0-0');
      this.input.nativeElement.style.color = 'red';
      this.notificationService.showAlert(`No se puede superar el valor máximo de ` +
                                         `${this.currencyPipe.transform(this.maxValue, 'USD', 'symbol', '0.0-0')}`);
    } else {
      this.input.nativeElement.style.color = '#000000';
    }

    e.target.value =  this.value;

    this.updateElement();
    this.emitValueChange();
    this.validate();
    // this.focusNextControl();
  }

  private reset() {
    this.value = '';
    this.rawValue = 0;
    // this.disabled = true;

    this.validate();
  }

  private updateElement() {
    this.model.value = this.value;
    this.model.rawValue = this.rawValue;
    this.model.disabled = this.disabled;
    this.option.valorOption = this.model;
  }

  private emitValueChange() {
    const args = {
      amparo: this.amparo,
      optionIndex: this.optionIndex,
      opcion: this.model
    };

    this.valoresAseguradosService.emit(ValoresAseguradosEvents.VALOR_DIARIO_CHANGE, args);
  }

  private validate() {
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

  private focusNextControl() {
    if (this.rawValue > 0) {
      setTimeout(() => {
        const elIndex = this.optionIndex;
        const table = document.getElementById('tableValoresAseguradosModalidades');
        const inputs = table.querySelectorAll('input:not([disabled])');
        (inputs[elIndex] as HTMLElement).focus();
      });
    }
  }
}
