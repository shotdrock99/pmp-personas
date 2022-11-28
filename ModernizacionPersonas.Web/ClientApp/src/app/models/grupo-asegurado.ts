import { Amparo } from '.';
import { AmparoGrupo } from './grupo-asegurado-server';

export class GrupoAsegurado {
  aseguradosOpcion1?: number;
  aseguradosOpcion2?: number;
  aseguradosOpcion3?: number;
  codigoCotizacion: number;
  codigoGrupoAsegurado?: number;
  nombreGrupoAsegurado: string;
  codigoTipoSuma?: number;
  conDistribucionAsegurados?: boolean;
  tipoSumaAsegurada?: TipoSumaAsegurada;
  valorMaxAsegurado?: number;
  valorMinAsegurado?: number;
  numeroSalariosAsegurado?: number;
  conListaAsegurados?: boolean;
  tipoEstructura?: string;
  valorAsegurado: number;
  numeroAsegurados?: number;
  edadPromedioAsegurados?: number;
  porcentajeAsegurados?: number;

  configured?: boolean;
}

export class GrupoAseguradoExtended extends GrupoAsegurado {
  codigoCotizacion: number;
  amparos: AmparoGrupo[];
  rangosGrupo: IRango[];
}

export class TipoSumaAsegurada {
  codigoTipoSumaAsegurada: number;
  nombreTipoSumaAsegurada: string;
  valorSalarioMinimo?: number;
  disabled?: boolean;
}

export interface IRango {
  codigoGrupoAsegurado?: number;
  codigoRangoGrupoAsegurado: number;
  edadMinAsegurado: number;
  edadMaxAsegurado: number;
  numeroAsegurados: number;
  valorAsegurado: number;
  isEdit?: boolean;
}

export interface Opcione {
  indiceOpcion: number;
  valorAseguradoTotal: number;
  valorAsegurado: number;
  amparo: Amparo;
  tasaRiesgo: number;
  tasaComercialAnual: number;
  porcentajeDescuento: number;
  porcentajeRecargo: number;
  tasaComercialAplicar: number;
  primaAnualIndividual: number;
  primaAnualTotal: number;
  tieneTasaSiniestralidad?: boolean;
  siniestralidad?: Siniestralidad;
  configurado: boolean;
}

export class Siniestralidad {
  amparo: Amparo;
  tasaRiesgo: number;
  tasaComercial: number;
  porcentajeDescuento: number;
  porcentajeRecargo: number;
  tasaComercialAplicar: number;
  primaAnualIndividual: number;
  primaAnualTotal: number;
}

export interface EdadIngresoPermanenciaAmparo {
  amparo: Amparo;
  edadMinimaIngreso: EdadIngresoPermanenciaAmparoField;
  edadMaximaIngreso: EdadIngresoPermanenciaAmparoField;
  edadMaximaPermanencia: EdadIngresoPermanenciaAmparoField;
  numeroDiasCarencia: EdadIngresoPermanenciaAmparoField;
}

export interface EdadIngresoPermanenciaAmparoField {
  disabled: boolean;
  rawValue?: number;
  valor: number;
}

export interface ValorAseguradoAmparo {
  amparo: Amparo;
  opciones: OpcionValorAsegurado[];
  calculoBasePorValorAsegurado: boolean;
}

export interface OpcionValorAsegurado {
  porcentajeOption?: ValorOption;
  valorOption?: ValorOption;
  primaOption?: ValorOption;
  numeroSalariosOption?: ValorOption;
  tasa?: number;
  valorDiarioOption?: ValorOption;
  numeroDiasOption?: ValorOption;
  valorDiarioDiasOption?: ValorOption;
}

export class ValorOption {
  disabled: boolean;
  rawValue?: number;
  value: string;

  static createNew(): ValorOption {
    const result: ValorOption = {
      disabled: true,
      rawValue: 0,
      value: null
    };

    return result;
  }
}

export class Asegurados {
  valorAsegurado: number;
  numeroAsegurados: number;
  aseguradosOpcion1: number;
  aseguradosOpcion2: number;
  aseguradosOpcion3: number;
  edadPromedio: number;
  porcentajeAsegurados: number;
  numeroPotencialAsegurados: number;
  conListaAsegurados: boolean;
  conDistribucionAsegurados: boolean;
  tipoEstructura: string;
  rangos: IRango[];
}

export class GrupoAseguradoCompleted {
  grupoAsegurado: GrupoAsegurado;
  selectedRamos: Amparo[];
  valoresAsegurados: ValorAseguradoAmparo[];
  edadesAmparos: EdadIngresoPermanenciaAmparo[];
  asegurados: Asegurados;
}

export enum ValoresAseguradosEvents {
  BASE_CALCULO_CHANGE = 'BASE_CALCULO_CHANGE',
  PRIMA_VALUE_CHANGE = 'PRIMA_VALUE_CHANGE',
  PERCENT_VALUE_CHANGE = 'PERCENT_VALUE_CHANGE',
  SALARIO_VALUE_CHANGE = 'SALARIO_VALUE_CHANGE',
  CURRENCY_VALUE_CHANGE = 'CURRENCY_VALUE_CHANGE',
  DIAS_VALUE_CHANGE = 'DIAS_VALUE_CHANGE',
  VALOR_DIARIO_CHANGE= 'VALOR_DIARIO_CHANGE'
}

export interface OpcionesUsadas {
  opcion1: boolean;
  opcion2: boolean;
  opcion3: boolean;
}
