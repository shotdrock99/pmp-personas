import { Injectable } from '@angular/core';
import { GrupoAseguradoWizardService } from './grupo-asegurado-wizard.service';
import { ValoresAseguradosEvents, TipoSumaAsegurada } from '../models';



export class SubscribeArgs {
  event: ValoresAseguradosEvents;
  optionIndex: number;
  amparo?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ValoresAseguradosService {

  private _subscribers: any[] = [];
  private _tipoSumaAsegurada: TipoSumaAsegurada;
  private _displayedColumns: string[];
  private _columnHeaders: { porcentajeColumnHeaderText: string; valorColumnHeaderText: string; };
  private _calculoBasePorValorAsegurado: boolean;

  get calculoBasePorValorAsegurado() {
    return this._calculoBasePorValorAsegurado;
  }

  set calculoBasePorValorAsegurado(value) {
    this._calculoBasePorValorAsegurado = value;
    this.wizardService.calculoBasePorValorAsegurado = value;
  }

  get tipoSumaAsegurada() {
    return this._tipoSumaAsegurada;
  }

  get tableColumns() {
    return this._displayedColumns;
  }

  get columnHeaders() {
    return this._columnHeaders;
  }

  constructor(private wizardService: GrupoAseguradoWizardService) { }

  private defineDisplayedColumns() {
    let columns = [
      'amparo',
      'opcion1porcentaje'
    ];

    if (this.wizardService.capturaSalarios) {
      if (this.wizardService.calculaValorAseguradoONumeroSalarios) {
        columns.push('opcion1valor');
      }

      columns.push('numerosueldos');
    }
    else {
      columns.push('opcion1valor');
    }

    let tipoSumaFijaColumns = [
      'opcion2porcentaje',
      'opcion2valor',
      'opcion3porcentaje',
      'opcion3valor'
    ];

    this._displayedColumns = this.tipoSumaAsegurada.codigoTipoSumaAsegurada === 1
      ? columns.concat(tipoSumaFijaColumns)
      : columns;
  }

  private defineHeaders() {
    let columnHeaders = {
      porcentajeColumnHeaderText: '%',
      valorColumnHeaderText: 'Valor'
    };

    switch (this.tipoSumaAsegurada.codigoTipoSumaAsegurada) {
      // Header text para tipo de suma Multiplo de Sueldos (2)
      case 2:
        columnHeaders.porcentajeColumnHeaderText = '% o Prima';
        columnHeaders.valorColumnHeaderText = 'Número de Sueldos';
        break;
      // Header text para tipo de suma Suma Variable por asegurado (3)
      // y Saldo Deudores-Ahorros-Aportes (6)
      // y Suma fija y múltiplo de sueldos (5)
      case 3:
      case 5:
      case 6:
        columnHeaders.porcentajeColumnHeaderText = '% o Prima';
        columnHeaders.valorColumnHeaderText = 'Valor asegurado';
        break;
      case 10:
        columnHeaders.porcentajeColumnHeaderText = '% o Prima';
        columnHeaders.valorColumnHeaderText = 'Valor asegurado';
        break;
    }

    this._columnHeaders = columnHeaders;
  }

  private configure(tipoSumaAsegurada: TipoSumaAsegurada) {
    this._tipoSumaAsegurada = tipoSumaAsegurada;

    this.defineHeaders();
    this.defineDisplayedColumns();
  }

  init(tipoSumaAsegurada: TipoSumaAsegurada) {
    this._subscribers = [];

    this.configure(tipoSumaAsegurada);
  }

  calcularAmparoBase(dataSource: any[], optionIndex?: number) {
    let amparoBaseCalculo = dataSource.find(x => x.amparo.siNoAdicional === false);
    if (amparoBaseCalculo) {
      let opcionesAmparoBaseCalculo = amparoBaseCalculo.opciones;
      if (!optionIndex) {
        optionIndex = 1;
      }

      let opcionAmparoBaseCalculo = opcionesAmparoBaseCalculo[optionIndex - 1];

      

      if (this.wizardService.capturaSalarios) {
        this.calculoBasePorValorAsegurado = opcionAmparoBaseCalculo.valorOption.rawValue > 0;
        if (this._tipoSumaAsegurada.codigoTipoSumaAsegurada === 2 || this._tipoSumaAsegurada.codigoTipoSumaAsegurada === 10) {
          this.calculoBasePorValorAsegurado = false;
        }
        if (!this.calculoBasePorValorAsegurado) {
          return opcionAmparoBaseCalculo.numeroSalariosOption.rawValue;
        }
        else {
          return opcionAmparoBaseCalculo.valorOption.rawValue;
        }
      }

      return opcionAmparoBaseCalculo.valorOption.rawValue;
    }

    return 0;
  }

  validateForm(args: any) {

  }

  subscribe(args: SubscribeArgs, fn: (e: any, args: any) => void) {
    this._subscribers.push({
      timestamp: Date.now(),
      args: args,
      fn: fn
    });
  }

  emit(event: ValoresAseguradosEvents, args: any) {
    
    args.event = event;
    let callbacks = this._subscribers.filter(x => x.args.event === event && x.args.optionIndex === args.optionIndex);
    callbacks.forEach(c => c.fn(args));
  }
}
