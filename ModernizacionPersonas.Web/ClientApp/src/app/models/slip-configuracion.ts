import { ActionResponseBase } from './action-response.base';
import { SlipClausula } from './slip-clausula';
import { SlipAmparo } from './slip-amparo';

export interface SlipConfiguracion {
  codigoCotizacion: number;
  numeroCotizacion: string;
  codigoDepartamento: number;
  codigoCiudad: number;
  tomador: TomadorSlip;
  actividad: string;
  diasValidezCotizacion: number;
  amparos: SlipAmparo[];
  clausulas: SlipClausula[];
  condiciones: string;
  snCambioClausulas: boolean;
}

export interface TomadorSlip {
  codigoTomador: number;
  nombre: string;
  direccion: string;
  telefono: string;
  email: string;
  codigoDepartamento: number;
  codigoCiudad: number;
  esIntermediario: boolean;
  nombreTomadorSlip: string;
}

export interface GenerarSlipConfiguracionResponse extends ActionResponseBase {
  data: SlipConfiguracion;
}
