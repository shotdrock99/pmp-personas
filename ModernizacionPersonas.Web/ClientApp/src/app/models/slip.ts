import { EdadAsegurabilidad } from './edad-asegurabilidad';
import { ActionResponseBase } from './action-response.base';
import { Vigencia } from './fichatecnica';
import { CotizacionState } from 'src/app/models';

export interface Slip {
  codigoCotizacion: number;
  estadoCotizacion: CotizacionState;
  codigoRamo: number;
  codigoSubramo: number;
  fechaString: string;
  infoEncabezado: SlipPageHeader;
  imagenProductoUri: string;
  imagenFirmaUri: string;
  descripcion: string;
  ciudad: string;
  fecha: string;
  tomador: SlipTomador;
  tomadorIntermediario: SlipTomador;
  asunto: string;
  tipoPoliza: string;
  diasVigencia: number;
  diasVigenciaWords: string;
  vigencia: Vigencia;
  dirigidoSlip: DirigidoSlip;
  infoGeneral: SlipInformacionGeneralSection;
  amparos: SlipAmparosSection;
  gruposAsegurados: SlipGruposAsegurados;
  clausulas: SlipClausulasSection;
  condiciones: SlipCondicionesSection;
  disposiciones: SlipDisposicionesSection;
  informacionEnvio: string[];
  nombreSucursal: string;
}

export interface SlipGruposAsegurados {
  gruposAsegurados: SlipGrupoAsegurado[];
  tituloSeccion: string;
}

export interface SlipGrupoAsegurado {
  codigoGrupoAsegurado: number;
  nombre: string;
  snTasaMensual: boolean;
  edades: EdadesAmparoSlip[];
  valorMaximo: ValorMaximoAseguradoIndividualSlip;
  valoresAmparos: ValoresAseguradosAmparoSlip[];
}

export interface SlipSeccion {
  codigoSeccion: string;
  codigoAmparo: string;
  tituloSeccion: string;
  texto: string;
}

export interface SlipPageHeader {
  headerImageUri: string;
  footerImageUri: string;
  imagen1Uri: string;
  imagen2Uri: string;
}

export interface SlipTomador {
  nombre: string;
  codigoTipoDocumento: number;
  numeroDocumento: string;
  actividad: string;
  direccion: string;
  telefono: string;
  departamento: string;
  ciudad: string;
}

export interface EdadesAmparoSlip {
  nombreAmparo: string;
  edadMinimaIngreso: number;
  edadMaximaIngreso: number;
  edadMaximaPermanencia: number;
}

export interface ValorMaximoAseguradoIndividualSlip {
  secciones: string;
  tituloSeccion: string;
}

export interface ValoresAseguradosAmparoSlip {
  codigoTipoSumaAsegurada: number;
  nombreAmparo: string;
  countOpciones: number;
  opciones: OpcionValoresAseguradosAmparoSlip[];
}

export interface OpcionValoresAseguradosAmparoSlip {
  valorAseguradoIndividual?: number;
  valorAsegurado: number;
  valorTasaComercial: number;
  tipoValor: number;
  tasaMensual: number;
  numeroDias: number;
  tablaValoresDiarios: boolean;
  valorDiario: boolean;
}

export interface SlipAmparosSection {
  tituloSeccion: string;
  amparos: SlipSeccion[];
}

export interface SlipClausulasSection {
  tituloSeccion: string;
  secciones: SlipSeccion[];
  asegurabilidad: EdadAsegurabilidad[];
}

export interface SlipCondicionesSection {
  tituloSeccion: string;
  secciones: string;
}

export interface SlipInformacionGeneralSection {
  tituloSeccion: string;
  secciones: SlipSeccion[];
}

export interface SlipDisposicionesSection {
  tituloSeccion: string;
  secciones: SlipSeccion[];
}

export interface GenerarSlipDataResponse extends ActionResponseBase {
  data: Slip;
}

export interface DirigidoSlip {
  nombre: string;
  direccion: string;
  telefono: string;
}
