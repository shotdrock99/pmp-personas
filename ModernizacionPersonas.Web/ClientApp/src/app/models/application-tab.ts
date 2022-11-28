import { CotizacionState } from '.';
export interface ApplicationTab {
  index: number;
  label: string;
  active: boolean;
  codigoEstado: CotizacionState;
  disabled: boolean;
}
