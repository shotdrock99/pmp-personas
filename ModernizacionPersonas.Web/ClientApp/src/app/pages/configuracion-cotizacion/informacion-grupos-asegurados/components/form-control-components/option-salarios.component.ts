import { Component, ElementRef, EventEmitter, Input, OnInit, Output, Renderer2, ViewChild } from '@angular/core';
import { Amparo, TipoSumaAsegurada, OpcionValorAsegurado, ValorAseguradoAmparo, ValoresAseguradosEvents } from 'src/app/models';
import { GrupoAseguradoWizardService } from 'src/app/services/grupo-asegurado-wizard.service';
import { ValoresAseguradosService } from 'src/app/services/valores-asegurados.service';
import { ValoresAseguradosValidator } from 'src/app/services/grupos-asegurados/valores-asegurados.validator';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';


@Component({
  selector: 'app-option-salarios',
  template: `<div class="number-container" >
          <input #input type="text" [disabled]="disabled" [mask]="mask"
            (change)="onChange($event)" value="{{ value }}"
            [readonly]="readonly" /></div>`,
  styleUrls: ['./styles.scss']
})
export class OptionSalariosComponent implements OnInit {
  model: any;
  disabled: boolean;
  // readonly: boolean;
  value = '';
  rawValue: number;
  amparo: Amparo;
  tipoSumaAsegurada: TipoSumaAsegurada;
  optionIndex = 1;
  option: OpcionValorAsegurado;
  mask = 'separator.2';
  maxValue: number;
  showComponent = true;
  baseCalculo: any;
  esBasicoNoAdicional: boolean;
  basicoNoAdicionalRequired: boolean;
  optionName = 'numeroSalariosOption';
  modalidadAmparo: number;
  SMMLV: number;

  constructor(
    private wizardService: GrupoAseguradoWizardService,
    private valoresAseguradosService: ValoresAseguradosService,
    private valoresAseguradosValidator: ValoresAseguradosValidator,
    private cotizacionDataService: CotizacionPersistenceService,
    private htmlEl: ElementRef,
    private renderer: Renderer2) { }

  @Output() enable: EventEmitter<any> = new EventEmitter();
  @Input() dataSource: any[];
  @Input() element: ValorAseguradoAmparo;
  @Input() readonly: boolean;

  @ViewChild('input', { static: true })
  input: ElementRef;

  ngOnInit() {
    
    this.option = this.element.opciones[this.optionIndex - 1];
    this.model = this.option.numeroSalariosOption;
    this.amparo = this.element.amparo;
    this.tipoSumaAsegurada = this.valoresAseguradosService.tipoSumaAsegurada;
    this.maxValue =
      this.element.amparo.modalidad.valores.length !== 0
        ? this.element.amparo.modalidad.valores[0].tope
        : undefined;
    this.modalidadAmparo = this.element.amparo.modalidad.codigo;
    this.SMMLV = this.cotizacionDataService.SMMLV;
    this.esBasicoNoAdicional = this.amparo.siNoBasico && !this.amparo.siNoAdicional;
    this.basicoNoAdicionalRequired = this.wizardService.basicoNoAdicionalRequired;
    this.baseCalculo = this.valoresAseguradosService.calcularAmparoBase(this.dataSource, this.optionIndex);

    this.rawValue = this.model.rawValue;
    this.value = this.rawValue > 0 ? this.model.value.replace(/(?!^)(?=(?:\d{3})+$)/g, ' ') : '';

    // this.readonly = false;
    this.showComponent = !this.disabled && this.amparo.codigoGrupoAmparo !== 3;

    this.updateControl();
    this.setDisabledState();

    this.subscribeOnBaseCalculoChange();
    this.subscribeOnValueChange();
    this.subscribeOnPercentValueChange();
  }

  private updateControl() {
    const multiplier = this.option.porcentajeOption.rawValue;
    if (this.amparo.siNoPorcentajeBasico && multiplier > 0 && !this.valoresAseguradosService.calculoBasePorValorAsegurado) {
      this.rawValue = (this.baseCalculo * multiplier) / 100;
      if (this.rawValue.toString().length > 4) {
        this.rawValue = +((this.baseCalculo * multiplier) / 100).toString().match(/^-?\d+(?:\.\d{0,2})?/)[0];
      }
      this.value = this.rawValue.toString().replace(/(?!^)(?=(?:\d{3})+$)/g, ' ');
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
      if (this.baseCalculo > 0 && !this.esBasicoNoAdicional) {
        this.disabled = false;
      }
      // deshabilite el control si el amparo captura pordcentaje basico
      if (this.amparo.siNoPorcentajeBasico) {
        this.disabled = true;
      }
      if (this.amparo.codigoAmparo === change.amparo.codigoAmparo && change.opcion.rawValue > 0) {
        if (change.event === ValoresAseguradosEvents.CURRENCY_VALUE_CHANGE) {
          // this.disabled = true;
          this.value = '';
          this.rawValue = 0;
          this.updateElement();
        }
      }
    }

    // si el basico del calculo es 0 y el amparo es un basico adicional
    if (this.baseCalculo === 0 && !this.esBasicoNoAdicional) {
      this.disabled = true;
      this.reset();
    }

    // deshabilite el control si el tipo de suma asegurada captura salarios
    // y se definio la captura por valor
    // y el valor base de calculo es mayor a 0
    if (this.wizardService.capturaSalarios && this.valoresAseguradosService.calculoBasePorValorAsegurado && this.baseCalculo > 0) {
      if (this.esBasicoNoAdicional) {
        if (this.tipoSumaAsegurada.codigoTipoSumaAsegurada !== 2) {
          this.disabled = true;
        }
      }
    }

    // si el grupo del amparo es 3 (Asistencias)
    if (this.amparo.codigoGrupoAmparo === 3) {
      this.disabled = true;
    }

    // si es SMMLV y es ABNA
    if (this.tipoSumaAsegurada.codigoTipoSumaAsegurada === 10) {
      if (this.esBasicoNoAdicional) {
        this.disabled = false;
      }
    }
  }

  private subscribeOnPercentValueChange() {
    
    if (!this.esBasicoNoAdicional) {
      this.valoresAseguradosService
        .subscribe({
          event: ValoresAseguradosEvents.PERCENT_VALUE_CHANGE,
          optionIndex: this.optionIndex
        }, (change) => {
          
          this.baseCalculo = this.valoresAseguradosService.calcularAmparoBase(this.dataSource, this.optionIndex);
          // si aplica calculo de amparo basico
          if (this.amparo.siNoPorcentajeBasico && !this.valoresAseguradosService.calculoBasePorValorAsegurado) {
            const multiplier = this.option.porcentajeOption.rawValue;
            this.rawValue = (this.baseCalculo * multiplier) / 100;
            if (this.rawValue.toString().length > 4) {
              this.rawValue = +((this.baseCalculo * multiplier) / 100).toString().match(/^-?\d+(?:\.\d{0,2})?/)[0];
            }
            this.value = this.rawValue.toString().replace(/(?!^)(?=(?:\d{3})+$)/g, ' ');
          }
          this.model = {
            disabled: this.disabled,
            rawValue: this.rawValue,
            value: this.value.trim().replace(/\s/g, '')
          };
          // this.validate();
        });
    }
  }

  private subscribeOnBaseCalculoChange() {
    
    if (!this.esBasicoNoAdicional || this.wizardService.calculaValorAseguradoONumeroSalarios) {
      this.valoresAseguradosService
        .subscribe({
          event: ValoresAseguradosEvents.BASE_CALCULO_CHANGE,
          optionIndex: this.optionIndex,
          amparo: this.amparo.nombreAmparo
        }, (change) => {
          this.baseCalculo = this.valoresAseguradosService.calcularAmparoBase(this.dataSource, this.optionIndex);
          // Set values for SMMLV on change
          if (this.tipoSumaAsegurada.codigoTipoSumaAsegurada === 10 || this.tipoSumaAsegurada.codigoTipoSumaAsegurada === 2) {
            this.valoresAseguradosService.calculoBasePorValorAsegurado = false;
          }
          // si aplica calculo de amparo basico
          if (this.amparo.siNoPorcentajeBasico && !this.valoresAseguradosService.calculoBasePorValorAsegurado) {
            const multiplier = this.option.porcentajeOption.rawValue;
            this.rawValue = (this.baseCalculo * multiplier) / 100;
            if (this.rawValue.toString().length > 4) {
              this.rawValue = +((this.baseCalculo * multiplier) / 100).toString().match(/^-?\d+(?:\.\d{0,2})?/)[0];
            }
            this.value = this.rawValue.toString().replace(/(?!^)(?=(?:\d{3})+$)/g, ' ');
          }
          this.model = {
            disabled: this.disabled,
            rawValue: this.rawValue,
            value: this.value.trim().replace(/\s/g, '')
          };
          this.setDisabledState(change);
          // this.validate();
        });
    }
  }

  private subscribeOnValueChange() {
    
    if (!this.esBasicoNoAdicional || this.wizardService.calculaValorAseguradoONumeroSalarios) {
      this.valoresAseguradosService
        .subscribe({
          event: ValoresAseguradosEvents.CURRENCY_VALUE_CHANGE,
          optionIndex: this.optionIndex
        }, (change) => {
          this.setDisabledState(change);
          // this.validate();
        });
    }
  }

  private validate() {
    
    if (this.wizardService.capturaSalarios) {
      const opcion = this.dataSource.map(x => {
        const maxValue: any[] = [undefined, undefined];
        maxValue[0] = x.amparo.modalidad.valores.length !== 0 ?
          ((x.amparo.modalidad.codigo === 5) ? this.SMMLV * x.amparo.modalidad.valores[0].tope :
            x.amparo.modalidad.valores[0].tope) : undefined;
        const valor = x.opciones[this.optionIndex - 1];
        if (this.element.amparo.codigoAmparo === x.amparo.codigoAmparo && valor.valorOption.rawValue === 0) {
          x.calculoBasePorValorAsegurado = false;
        }
        if (x.amparo.modalidad.codigo === 4) {
          maxValue[1] = x.amparo.modalidad.valores[1].tope;
        }
        return {
          amparo: x.amparo,
          valor,
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

  private updateElement() {
    // update element
    this.model.value = this.value.trim().replace(' ', '');
    this.model.rawValue = this.rawValue;
    this.model.disabled = this.disabled;
    this.option.numeroSalariosOption = this.model;
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
    this.valoresAseguradosService.emit(ValoresAseguradosEvents.SALARIO_VALUE_CHANGE, args);
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
    this.rawValue = Number(this.value.trim().replace(/\s/g, ''));
    if (this.maxValue && this.rawValue > this.maxValue) {
      this.rawValue = this.maxValue;
      this.value = `${this.rawValue}`;
    }
    e.target.value = this.value.trim().replace(/\s/g, '');
    this.updateElement();
    // if (this.wizardService.calculaValorAseguradoONumeroSalarios) {
    this.valoresAseguradosService.calculoBasePorValorAsegurado = false;
    this.wizardService.calculoBasePorValorAsegurado = false;
    // }
    this.emitValueChange();
    this.validate();
    this.focusNextControl();
  }
}
