import { CotizacionState } from '.';

export class ActionResponseBase {
  codigoCotizacion: number;
  cotizacionState: CotizacionState;
  numeroCotizacion: string;
  status: ResponseStatus;
  message: string;
}

export enum ResponseStatus {
  valid = 1,
  invalid
}
