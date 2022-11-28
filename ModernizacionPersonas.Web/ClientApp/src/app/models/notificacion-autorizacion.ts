import { CotizacionAuthorization, AuthorizationUser } from './cotizacion-authorization';

export interface NotificacionAutorizaciones {
  autorizaciones: CotizacionAuthorization[];
  observaciones: string;
  usuarioNotificacion: AuthorizationUser;
}
