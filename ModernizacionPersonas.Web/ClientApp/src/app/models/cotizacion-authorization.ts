import { CotizacionState } from './cotizacion';
import { StringValueToken } from 'html2canvas/dist/types/css/syntax/tokenizer';

export interface CotizacionValidationResponse {
  codigoCotizacion: number;
  cotizacionState: number;
  numeroCotizacion: string;
  status: CotizacionState;
  message: string;
  isValid: boolean;
  validation: CotizacionValidation;
  tasas: CotizacionTasa[];
  requireAuthorization: boolean;
}

export interface CotizacionTasa {
  codigoTasa: number;
  nombreTasa: number;
  value: number;
}

export interface GetAuthorizationsResponse {
  authorizations: CotizacionAuthorization[];
}

export interface CotizacionValidation {
  validationMessage: ValidationMessage;
  authorizations: CotizacionAuthorization[];
  users: AuthorizationUser[];
}

export interface ValidationMessage {
  codigoGrupoAsegurado: number;
  isValid: boolean;
  message: string;
}

export interface AuthorizationUser {
  codigoCotizacion: number;
  versionCotizacion: number;
  codigo: string;
  codigoRol: number;
  nombreRol?: string;
  codigoNivel: number;
  codigoTipoAutorizacion: number;
  activo: boolean;
  notificado: boolean;
  especial: boolean;
}

export interface UserRole {
  id: number;
  name: string;
}

export enum AutorizacionAction {
  Accept = 1,
  Reject,
  Modify
}

export class AutorizacionTipoTasa {
  codigoTasa: number;
  nombreTasa: string;
  valor: number;
}

export interface TransactionComment {
  transactionId?: number;
  codigoUsuario?: string;
  codigoRolAutorizacion: string;
  codigoTipoAutorizacion: number;
  message: string;
}

export interface AutorizacionResult {
  comments: TransactionComment[];
  gastosCompania: number;
  utilidadesCompania: number;
  tasa: AutorizacionTipoTasa;
}

export interface AutorizacionArgs {
  codigoCotizacion: number;
  version: number;
  transactionId?: number;
  action: AutorizacionAction;
  codigoUsuarioAutorizador: string;
  authorizationResult: AutorizacionResult;
  userName: string;
}

export interface ChangesArgs {
  codigoCotizacion: number;
  version: number,
  gastosCompania: number;
  utilidadesCompania: number;
}

export interface NotifyCotizacionArgs {
  codigoCotizacion: number;
  version: number;
  transactionId?: number;
  authorizationControls: CotizacionAuthorization[];
  authorizationUser: AuthorizationUser;
  comments: TransactionComment[];
}

export interface CotizacionAuthorization {
  codigoCotizacion: number;
  version: number;
  codigoGrupoAsegurado: number;
  nombreGrupoAsegurado: string;
  codigoSucursal: number;
  codigoRamo: number;
  codigoSubramo: number;
  codigoAmparo: number;
  nombreAmparo: string;
  campoEntrada: string;
  valorEntrada: number;
  codigoTipoAutorizacion: number;
  requiereAutorizacion: boolean;
  codigoUsuario: string;
  mensajeValidacion: string;
  nombreSeccion: string;
  siseAuth: boolean;

  items: CotizacionAuthorization[];
  displayItems: boolean;
}

export enum TipoDelegacionAutorizacion {
  porDelegacion = 1,
  directa
}

export enum TipoAutorizacion {
  pasiva,
  activa
}

export interface CotizacionTransaction {
  codigoTransaccion: number;
  codigoCotizacion: number;
  codigoEstadoCotizacion: CotizacionState;
  codigoUsuario: string;
  description: string;
  creationDate: Date;
  conteoControles: number;
  initials: string;
  codigoRol: number;
  estado: number;
  nombreRol: string;
  attachments?: AutorizacionAttachment[],
  comments: TransactionComment[],
  downloadUrl: string;
}

export interface AutorizacionAttachment {
  name: string;
  type: string;
  uri: string;
}
