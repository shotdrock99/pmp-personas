import { EdadAsegurabilidad } from './edad-asegurabilidad';
import { ErrorCollection } from 'src/app/models/error-collection';

export interface SlipClausula {
  codigoSeccion: number;
  codigoRamo: number;
  codigoSubramo: number;
  nombre: string;
  activo: boolean;
  variables?: SlipClausulaVariable[];
  asegurabilidad?: EdadAsegurabilidad[];
  descripcion: string;
}

export class SlipClausulaVariable {
  // codigoPadre:number;
  codigo?: number;
  codigoVariable: number;
  nombre: string;
  valor: number;
  valorMaximo: number;
  tipoDato: string;
  errors?: ErrorCollection;
}
