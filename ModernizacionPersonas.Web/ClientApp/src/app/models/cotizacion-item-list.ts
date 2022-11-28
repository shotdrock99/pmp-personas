import { Tomador } from 'src/app/models/tomador';
import { CotizacionState } from '.';

export interface CotizacionItemList {
  codigoCotizacion: number;
  version: number;
  fechaCreacion: string;
  fechaModificacion: string;
  numeroCotizacion: string;
  zona: string;
  sucursal: string;
  ramo: string;
  subramo: string;
  tomador: string;
  usuarioNotificado: string;
  usuarioAutorizador: string;
  codigoEstado: CotizacionState;
  closed: boolean;
  lastAuthor: string;
  locked: boolean;
  btnFichaAlterna: number;
  checked: boolean;
}
