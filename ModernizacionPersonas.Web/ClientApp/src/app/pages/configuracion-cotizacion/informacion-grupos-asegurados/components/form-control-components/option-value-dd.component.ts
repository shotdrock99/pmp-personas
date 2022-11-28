import { Component, Input, OnInit } from '@angular/core';
import { Amparo, OpcionValorAsegurado, ValorAseguradoAmparo, ValoresAseguradosEvents } from 'src/app/models';
import { ValoresAseguradosService } from 'src/app/services/valores-asegurados.service';

@Component({
  selector: 'app-option-value-dd',
  template: `<div class="currency-container">
    <input #input type="text" [disabled]="true"
    value="{{ value | currency : 'USD' : 'symbol' : '1.0-4'}}" [readonly]="true" /> </div>`,
  styleUrls: ['./styles.scss'],
})
export class OptionValueDDComponent implements OnInit {

  amparo: Amparo;
  value: number;
  model: any;
  option: OpcionValorAsegurado;

  constructor(private valoresAseguradosService: ValoresAseguradosService) {}

  @Input()
  element: ValorAseguradoAmparo;

  @Input()
  optionIndex: number;

  ngOnInit() {
    this.option = this.element.opciones[this.optionIndex - 1];
    this.model = this.option.valorDiarioDiasOption;
    this.amparo = this.element.amparo;
    this.value = this.model.value;

    this.subscribeOnDiasChange();
    this.subscribeOnDailyValueChange();
  }

  private subscribeOnDiasChange() {
    this.valoresAseguradosService.subscribe({
        event: ValoresAseguradosEvents.DIAS_VALUE_CHANGE,
        optionIndex: this.optionIndex,
        amparo: this.amparo.nombreAmparo
    }, (change) => {
        if (change.amparo.codigoAmparo ===  this.amparo.codigoAmparo && change.optionIndex === this.optionIndex) {
            this.value = change.opcion.rawValue * this.option.valorDiarioOption.rawValue;
        }
    });
  }

  private subscribeOnDailyValueChange() {
    this.valoresAseguradosService.subscribe({
        event: ValoresAseguradosEvents.VALOR_DIARIO_CHANGE,
        optionIndex: this.optionIndex,
        amparo: this.amparo.nombreAmparo
    }, (change) => {
        if (change.amparo.codigoAmparo === this.amparo.codigoAmparo && change.optionIndex === this.optionIndex) {
            this.value = change.opcion.rawValue * this.option.numeroDiasOption.rawValue;
        }
    });
  }
}
