import { CurrencyPipe } from '@angular/common';
import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Amparo, OpcionValorAsegurado, TipoSumaAsegurada, ValorAseguradoAmparo, ValoresAseguradosEvents } from 'src/app/models';
import { GrupoAseguradoWizardService } from 'src/app/services/grupo-asegurado-wizard.service';
import { ValoresAseguradosValidator } from 'src/app/services/grupos-asegurados/valores-asegurados.validator';
import { ValoresAseguradosService } from 'src/app/services/valores-asegurados.service';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';

@Component({
  selector: 'app-option-value',
  template: `<div class="currency-container">
          <input #input type="text" [disabled]="disabled" [mask]="mask" prefix="$" thousandSeparator=","
            (change)="onChange($event)" value="{{ value }}" [readonly]="readonly" /></div>`,
  styleUrls: ['./styles.scss']
})
export class OptionValueComponent implements OnInit {

  model: any;
  disabled: boolean;
  // readonly: boolean;
  value = '';
  rawValue: number;
  amparo: Amparo;
  tipoSumaAsegurada: TipoSumaAsegurada;
  option: OpcionValorAsegurado;
  mask = 'separator.2';
  maxValue: number;
  showComponent = true;
  baseCalculo: any;
  esBasicoNoAdicional: boolean;
  currencyPipe = new CurrencyPipe('en-US');
  basicoNoAdicionalRequired: boolean;
  optionName = 'valorOption';
  modalidadAmparo: number;
  SMMLV: number;

  constructor(
    private wizardService: GrupoAseguradoWizardService,
    private valoresAseguradosService: ValoresAseguradosService,
    private cotizacionDataService: CotizacionPersistenceService,
    private valoresAseguradosValidator: ValoresAseguradosValidator,
    private notificationService: NotificationService) { }

  @Output() enable: EventEmitter<any> = new EventEmitter();
  @Input() dataSource: any[];
  @Input() readonly: boolean;
  @Input() element: ValorAseguradoAmparo;
  @Input() optionIndex: number;
  @ViewChild('input', { static: true })
  input: ElementRef;

  ngOnInit() {
    
    this.option = this.element.opciones[this.optionIndex - 1];
    this.model = this.option.valorOption;
    this.amparo = this.element.amparo;
    this.tipoSumaAsegurada = this.valoresAseguradosService.tipoSumaAsegurada;
    this.maxValue =
      this.element.amparo.modalidad.valores.length !== 0
        ? this.element.amparo.modalidad.valores[0].tope
        : undefined;
    this.modalidadAmparo = this.element.amparo.modalidad.codigo;
    this.esBasicoNoAdicional = this.amparo.siNoBasico && !this.amparo.siNoAdicional;
    this.basicoNoAdicionalRequired = this.wizardService.basicoNoAdicionalRequired;
    this.SMMLV = this.cotizacionDataService.SMMLV;
    this.baseCalculo = this.valoresAseguradosService.calcularAmparoBase(this.dataSource, this.optionIndex);

    this.rawValue = this.model.rawValue;
    this.value = this.rawValue > 0 ? this.currencyPipe.transform(this.rawValue, 'USD', 'symbol', '0.0-0') : '';

    // this.readonly = false;
    this.showComponent = !this.disabled && this.amparo.codigoGrupoAmparo !== 3;

    this.updateControl();
    this.setDisabledState();

    // suscribe changes
    this.subscribeOnBaseCalculoChange();
    this.subscribeOnValueChange();
    this.subscribeOnPercentValueChange();
  }

  private setDisabledState(change?: any) {
    this.baseCalculo = this.valoresAseguradosService.calcularAmparoBase(this.dataSource, this.optionIndex);
    this.disabled = this.model.disabled;
    // si el cambio se emitio por edición
    if (change) {
      // si el basico del calculo es mayor a 0
      // y el amparo es un basico adicional
      // y el amparo no captura porcentaje
      if (this.baseCalculo > 0 && !this.esBasicoNoAdicional && !this.amparo.siNoPorcentajeBasico) {
        this.disabled = false;
      }
      // deshabilite el control si el tipo de suma asegurada captura salarios
      // y se definio la captura por salarios
      // !this.valoresAseguradosService.calculoBasePorValorAsegurado
      // y el valor base de calculo es mayor a 0
      if (this.wizardService.capturaSalarios && this.baseCalculo > 0) {
        if (this.esBasicoNoAdicional) {
          if (this.tipoSumaAsegurada.codigoTipoSumaAsegurada !== 5) {
            this.disabled = true;
          }
        }
      }
      if (this.amparo.codigoAmparo === change.amparo.codigoAmparo && change.opcion.rawValue > 0) {
        if (change.event === ValoresAseguradosEvents.SALARIO_VALUE_CHANGE) {
          // this.disabled = true;
          // if (this.tipoSumaAsegurada.codigoTipoSumaAsegurada !== 10) {
          //   this.value = '';
          //   this.rawValue = 0;
          //   this.updateElement();
          // }
          this.value = '';
          this.rawValue = 0;
          this.updateElement();
        }
      }
    }
    // si es SMMLV y es ABNA
    if (this.tipoSumaAsegurada.codigoTipoSumaAsegurada === 10) {
      if (this.esBasicoNoAdicional) {
        this.disabled = true;
        // this.value = '';
      }
      if (this.amparo.siNoPorcentajeBasico) {
        if (this.tipoSumaAsegurada.codigoTipoSumaAsegurada !== 10) {
          this.value = '';
        }
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

    // si la captura se realiza por salarios y la base de calculo ya fue configurada
    if (!this.valoresAseguradosService.calculoBasePorValorAsegurado && this.baseCalculo > 0) {
      // this.disabled = true;
    }
  }

  private updateControl() {
    const multiplier = this.option.porcentajeOption.rawValue;
    if (this.amparo.siNoPorcentajeBasico && this.baseCalculo > 0 && multiplier > 0) {
      this.rawValue = this.baseCalculo * (multiplier / 100);
      if (this.tipoSumaAsegurada.codigoTipoSumaAsegurada === 10) {
        this.rawValue = (this.baseCalculo * this.tipoSumaAsegurada.valorSalarioMinimo) * (multiplier / 100);
      }
      this.value = this.currencyPipe.transform(this.rawValue, 'USD', 'symbol', '0.0-0');
    }
    this.validate();
  }

  private subscribeOnPercentValueChange() {
    if (!this.esBasicoNoAdicional) {
      this.valoresAseguradosService
        .subscribe({
          event: ValoresAseguradosEvents.PERCENT_VALUE_CHANGE,
          optionIndex: this.optionIndex
        }, (change) => {
          this.updateOnChange(change);
          // this.validate()
        });
    }
  }

  private updateOnChange(change: any) {
    if (this.amparo.codigoAmparo !== change.amparo.codigoAmparo) { return; }
    this.baseCalculo = this.valoresAseguradosService.calcularAmparoBase(this.dataSource, this.optionIndex);
    if (this.tipoSumaAsegurada.codigoTipoSumaAsegurada === 5) {
      this.rawValue = 0;
      this.value = this.currencyPipe.transform(this.rawValue, 'USD', 'symbol', '0.0-0');

    } else {
      const percent = this.element.opciones[change.optionIndex - 1].porcentajeOption.rawValue;
      // let valorAmparoBNA = change.opcion.rawValue;
      let valorAmparoBNA = this.baseCalculo;

      if (this.tipoSumaAsegurada.codigoTipoSumaAsegurada === 10) {
        valorAmparoBNA = this.tipoSumaAsegurada.valorSalarioMinimo * this.baseCalculo;
      }

      this.rawValue = valorAmparoBNA * (percent / 100);
      this.value = this.currencyPipe.transform(this.rawValue, 'USD', 'symbol', '0.0-0');

    }
    // si aplica calculo de amparo basico
    if (this.valoresAseguradosService.calculoBasePorValorAsegurado && this.baseCalculo > 0) {
      if (this.amparo.siNoPorcentajeBasico) {
        const multiplier = change.opcion.rawValue;
        this.rawValue = this.baseCalculo * (multiplier / 100);
        this.value = this.currencyPipe.transform(this.rawValue, 'USD', 'symbol', '0.0-0');
      }
    }
  }

  private subscribeOnBaseCalculoChange() {
    if (!this.esBasicoNoAdicional || this.wizardService.calculaValorAseguradoONumeroSalarios) {
      this.valoresAseguradosService
        .subscribe({
          event: ValoresAseguradosEvents.BASE_CALCULO_CHANGE,
          optionIndex: this.optionIndex
        }, (change) => {
          this.setDisabledState(change);
          if (this.tipoSumaAsegurada.codigoTipoSumaAsegurada !== 10) {
            if (this.tipoSumaAsegurada.codigoTipoSumaAsegurada !== 5) {
              this.rawValue = 0;
              this.value = this.currencyPipe.transform(this.rawValue, 'USD', 'symbol', '0.0-0');
            }
          }
          if (this.tipoSumaAsegurada.codigoTipoSumaAsegurada !== 5) {
            let percent = this.element.opciones[change.optionIndex - 1].porcentajeOption.rawValue;
            let valorAmparoBNA = change.opcion.rawValue;
            if (this.tipoSumaAsegurada.codigoTipoSumaAsegurada === 10) {
              valorAmparoBNA = this.tipoSumaAsegurada.valorSalarioMinimo * change.opcion.rawValue;
            }
            if (this.tipoSumaAsegurada.codigoTipoSumaAsegurada === 10 || this.tipoSumaAsegurada.codigoTipoSumaAsegurada === 2) {
              if (this.esBasicoNoAdicional) {
                percent = 100;
              }
            }
            this.rawValue = valorAmparoBNA * (percent / 100);
            if (this.rawValue !== 0) {
              this.value = this.currencyPipe.transform(this.rawValue, 'USD', 'symbol', '0.0-0');
            } else {
              this.value = '';
            }
          }
          // this.validate();
        });
    }
  }

  private subscribeOnValueChange() {
    if (!this.esBasicoNoAdicional || this.wizardService.calculaValorAseguradoONumeroSalarios) {
      this.valoresAseguradosService
        .subscribe({
          event: ValoresAseguradosEvents.SALARIO_VALUE_CHANGE,
          optionIndex: this.optionIndex
        }, (change) => {
          this.setDisabledState(change);
          // this.validate();
        });
    }
  }

  private validate() {
    if (!this.amparo.siNoPorcentajeBasico) {
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
          calculoBasePorValorAsegurado: x.calculoBasePorValorAsegurado
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
    const args = {
      amparo: this.amparo,
      optionIndex: this.optionIndex,
      opcion: this.model
    };
    if (this.esBasicoNoAdicional) {
      this.valoresAseguradosService.emit(ValoresAseguradosEvents.BASE_CALCULO_CHANGE, args);
    }

    this.valoresAseguradosService.emit(ValoresAseguradosEvents.CURRENCY_VALUE_CHANGE, args);
  }

  private updateElement() {
    // update element
    this.model.value = this.value;
    this.model.rawValue = this.rawValue;
    this.model.disabled = this.disabled;
    this.option.valorOption = this.model;
  }

  private focusNextControl() {
    if (this.esBasicoNoAdicional && this.rawValue > 0) {
      setTimeout(() => {
        const elIndex = this.optionIndex;
        const table = document.getElementById('tableValoresAsegurados');
        const inputs = table.querySelectorAll('input:not([disabled])');
        (inputs[elIndex] as HTMLElement).focus();
      });
    }
  }

  onChange(e) {
    this.value = e.target.value;
    this.rawValue = Number(this.value.replace(/\D+/g, ''));
    const maxValue = (this.modalidadAmparo === 5) ? this.SMMLV * this.maxValue : this.maxValue;
    if (this.maxValue && this.rawValue > maxValue) {
      // this.rawValue = this.maxValue;
      // this.value = this.currencyPipe.transform(this.rawValue, 'USD', 'symbol', '0.0-0');
      this.input.nativeElement.style.color = 'red';
      const mensajeError = (this.modalidadAmparo === 5) ? `${Math.round(this.maxValue)} SMMLV` :
      `${this.currencyPipe.transform(this.maxValue, 'USD', 'symbol', '0.0-0')}`;
      this.notificationService.showAlert(`NO se puede superar el valor máximo de ${mensajeError}`);
    } else {
      this.input.nativeElement.style.color = '#000000';
    }

    // if (this.rawValue > 0 || this.wizardService.calculaValorAseguradoONumeroSalarios) {
    this.updateElement();
    e.target.value = this.value;
    // if (this.wizardService.calculaValorAseguradoONumeroSalarios) {
    this.valoresAseguradosService.calculoBasePorValorAsegurado = true;
    this.wizardService.calculoBasePorValorAsegurado = true;
    // }
    this.emitValueChange();
    this.validate();
    this.focusNextControl();
  }
}
