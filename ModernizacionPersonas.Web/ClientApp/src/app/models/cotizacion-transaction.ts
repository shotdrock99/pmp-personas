export class CotizacionTransaction {
  codigoCotizacion: number;
  version: number;
  codigoTransaccion: number;
  codigoEstadoCotizacion: number;
  codigoUsuario: string;
  description: string;
  creationDate: Date;
  conteoControles: number;
  initials: string;
  codigoRol: number;
  nombreRol: string;
  comments: TransactionComment[];
  attachments: TransactionAttachment[];
  uNotificado: string;
}

export class TransactionComment {
  transactionId: number;
  codigoUsuario: string;
  codigoRolAutorizacion: number;
  codigoTipoAutorizacion: number;
  message: string;
}

export class TransactionAttachment {
  transactionId: number;
  name: string;
  type: string;
  uri: string;
}
