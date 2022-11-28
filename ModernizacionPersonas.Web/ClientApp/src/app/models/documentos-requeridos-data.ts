import { TableItem } from './tableitem';

export class DocumentosRequeridosData {
  columns: string[];
  data: DocumentosRequeridosTableItem[];
}

export class DocumentosRequeridosTableItem {
  index: number;
  descripcion: string;
  valores: boolean[]
}
