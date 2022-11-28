import { ErrorCollection } from 'src/app/models/error-collection';

export interface SlipAmparo {
  codigoRamo: number;
  codigoSubramo: number;
  codigoAmparo: number;
  nombreAmparo: string;
  activo: boolean;
  variables?: SlipAmparoVariable[],
  descripcion: string;

  showVariables: boolean;
}

export class SlipAmparoVariable {
  //codigoPadre:number;
  codigo?: number;
  nombre: string;
  valor: number;
  valorMaximo: number;
  tipoDato: string;
  errors?: ErrorCollection;
}
