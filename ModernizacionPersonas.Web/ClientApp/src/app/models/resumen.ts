import { CotizacionState, Opcione, TipoSumaAsegurada, TipoTasa } from 'src/app/models';
import { ActionResponseBase } from './action-response.base';

export interface Resumen {
  codigoCotizacion: number;
  numeroCotizacion: string;
  estadoCotizacion: CotizacionState;
  tipoTasa: TipoTasa;
  tieneSiniestralidad: boolean;
  comision: number;
  ivaComision: number;
  gRetorno: number;
  ivagRetorno: number;
  otrosGastos: number;
  gastosCompania: number;
  utilidad: number;
  factorG: number;
  porcentajeIvaComision: number;
  porcentajeIvaRetorno: number;
  gruposAsegurados: GrupoAseguradoResumen[];
}

export interface GrupoAseguradoResumen {
  aseguradosOpcion1: number;
  aseguradosOpcion2: number;
  aseguradosOpcion3: number;
  codigo: number;
  nombre: string;
  tipoSumaAsegurada: TipoSumaAsegurada;
  conDistribucionAsegurados: boolean;
  conListaAsegurados: boolean;
  numeroAsegurados: number;
  edadPromedio: number;
  amparos: AmparoResumen[];
  numeroOpciones: number;
  opciones: Opcione[];
  porcentajeEsperado: number;
  numeroAseguradosPotencial: number;

  visible: boolean;
}

export interface AmparoResumen {
  codigoAmparo: number;
  codigoGrupoAmparo?: number;
  edadMaximaIngreso?: number;
  edadMaximaPermanencia?: number;
  edadMinimaIngreso?: number;
  nombreAmparo: string;
  nombreCortoGrupoAmparo?: string;
  nombreGrupoAmparo?: string;
  siNoAdicional: boolean;
  siNoBasico: boolean;
  siNoPorcentajeBasico: boolean;
  siNoRequiereEdades: boolean;
}

export interface ProcesarResumenResponse extends ActionResponseBase {
  data: Resumen;
}
