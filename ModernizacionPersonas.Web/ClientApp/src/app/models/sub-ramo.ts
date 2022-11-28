export class SubRamo {
  codigoRamo?: number;
  codigoSubRamo: number;
  nombreAbreviado?: string;
  nombreRamo?: string;
  nombreReducido?: string;
  nombreSubRamo?: string;
  siNoPerfilEdades?: boolean;
  siNoPerfilValores?: boolean;

  static create(nombreSubramo: string) {
    let result = new SubRamo()
    result.codigoRamo = 0;
    result.codigoSubRamo = 0;
    result.nombreAbreviado = 'NA';
    result.nombreRamo = '';
    result.nombreReducido = '';
    result.nombreSubRamo = '';

    return result;
  };
}
