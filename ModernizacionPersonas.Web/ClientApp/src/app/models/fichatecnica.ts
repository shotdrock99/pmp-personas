import { TipoTasa } from 'src/app/models';
import { ActionResponseBase } from './action-response.base';
import { CotizacionState } from './cotizacion';
import { TipoSumaAsegurada } from './grupo-asegurado';

export interface Vigencia {
  hastaString: string;
  desdeString: any;
  desde: string;
  hasta: string;
}

export class IntermediarioFichaTecnica {
  nombre: string;
  porcentajeParticipacion: number;
}

export interface InformacionTomador {
  tomador: string;
  vigencia: Vigencia;
  actividadRiesgo: string;
  ciudad: string;
  intermediarios: IntermediarioFichaTecnica[];
  aseguradoraActual: string;
  tipoNegocio: string;
  tipoContratacion: string;
  conListadoAsegurados: boolean;
}

export interface Siniestralidad {
  vigenciaDesde: string;
  vigenciaHasta: string;
  valorIncurrido: number;
  numeroCasos: number;
  siniestralidadPromedio: number;
  IBNR: number;
  sumValorIncurridoIBNR: number;
}

export interface SiniestralidadTotales {
  sumValorincurrido: number;
  sumNumeroCasos: number;
  sumValorIBNR: number;
}

export interface TasasSiniestarlidad {
  tasaPuraRiesgo: number;
  tasaComercial: number;
  primaAnualComercial: number;
  primaIndividualComercial: number;
}

export interface InformacionSiniestralidad {
  siniestralidad: Siniestralidad[];
  totales: SiniestralidadTotales;
  proyeccionFinanciera: ProyeccionFinancieraSiniestralidad;
  tasasSiniestralidad: TasasSiniestarlidad;
}

export interface InformacionRiesgo {
  numeroAsegurados: number;
  numeroAseguradosPotencial: number;
  porcentajeEsperado: number;
  edadPromedio: number;
  valorTotalAsegurado: number;
}

export interface Amparo2 {
  codigoAmparo: number;
  nombreAmparo: string;
  siNoAdicional: boolean;
  siNoBasico: boolean;
  siNoPorcentajeBasico: boolean;
  siNoRequiereEdades: boolean;
}

export interface Amparo {
  amparo: Amparo2;
  valorAsegurado: number;
}

export interface InformacionFactorG {
  comisionIntermediario: number;
  ivaComisionIntermediario: number;
  gastosRetorno: number;
  ivaGastosRetorno: number;
  otroGgastos: number;
  gastosCompania: number;
  utilidad: number;
  totalFactorG: number;
}

export interface ProyeccionFinanciera {
  tasaComercialAnual: number;
  porcentajeDescuento: number;
  porcentajeRecargo: number;
  tasaComercialTotal: number;
}

export interface ProyeccionFinancieraSiniestralidad extends ProyeccionFinanciera {
  tasaRiesgoSiniestralidad: number;
}

export interface PyGAnual {
  primaTotal: number;
  asistencia: number;
  siniestrosIncurridos: 0;
  siniestralidad: number;
  porcentajeSinietralidad: number;
  comisionIntermediario: number;
  ivaComisionIntermediario: number;
  gastosRetorno: number;
  ivaGastosRetorno: number;
  otrosGastos: number;
  gastosCompania: number;
  utilidad: number;
  porcentajeUtilidadAnno: number;
}

export interface RangoEdad {
  edadDesde: number;
  edadHasta: number;
  cantidadAsegurados: number;
  porcentajeParticipacionAsegurados: number;
  valorAseguradoTotal: number;
  porcentajeParticipacionValorAsegurado: number;
  promedioValorAsegurado: number;
  aseguradoMayorEdad: number;
}

export interface PerfilTotales {
  sumCantidadAsegurados: number;
  sumPorcentajeParticipacionAsegurados: number;
  sumValorAseguradoTotal: number;
  sumPorcentajeParticipacionValorAsegurado: number;
  sumPromedioValorAsegurado: number;
}

export interface PerfilEdades {
  rangos: RangoEdad[];
  totales: PerfilTotales;
}

export interface RangoValor {
  valorAseguradoDesde: number;
  valorAseguradoHasta: number;
  cantidadAsegurados: number;
  porcentajeParticipacionAsegurados: number;
  valorAseguradoTotal: number;
  porcentajeParticipacionValorAsegurado: number;
  promedioValorAsegurado: number;
}

export interface PerfilValores {
  rangos: RangoValor[];
  totales: PerfilTotales;
}

export interface GrupoAseguradoFichaTecnica {
  codigo: number;
  nombre: string;
  aseguradosOpcion1: number;
  aseguradosOpcion2: number;
  aseguradosOpcion3: number;
  tipoSumaAsegurada: TipoSumaAsegurada;
  conListaAsegurados: boolean;
  numeroAsegurados: number;
  edadPromedio: number;
  amparos: AmparoFichaTecnica[];
  asistencias: AmparoFichaTecnica[];
  numeroOpciones: number;
  porcentajeEsperado: number;
  numeroAseguradosPotencial: number;
  primas: PrimasGrupoAsegurado;
  proyeccionesFinancieras: ProyeccionFinanciera[];
  valoresAseguradosTotales: ValoresAseguradoTotalOpcion[];
  hasAsistencia: boolean;
  conDistribucionAsegurados: boolean;
  visible: boolean;
}

export interface PrimasGrupoAsegurado {
  primaIndividualAnual: ValorOpcionKeyValue[];
  primaIndividualTotal: ValorOpcionKeyValue[];
  primaTotalAnual: ValorOpcionKeyValue[];
  totalPrimaAnual: ValorOpcionKeyValue[];
}

export interface ValorOpcionKeyValue {
  indiceOpcion: number;
  valor: number;
}

export interface AmparoFichaTecnica {
  nombreAmparo: string;
  codigoAmparo: number;
  codigoAmparoGrupoAsegurado: number;
  codigoGrupoAsegurado: number;
  codigoGrupoAmparo: number;
  opcionesValores: OpcionValorAsegurado[];
  siNoAdicional: boolean;
}

export interface OpcionValorAsegurado {
  codigoAmparoGrupoAsegurado: number;
  codigoOpcionValorAsegurado: number;
  indiceOpcion: number;
  numeroSalarios: number;
  porcentajeCobertura: number;
  prima: number;
  tasaComercial: number;
  tasaRiesgo: number;
  valorAsegurado: number;
}

export interface ValoresAseguradoTotalOpcion {
  indiceOpcion: number;
  valorAseguradoTotal: number;
}

export interface FichaTecnica {
  codigoCotizacion: number;
  numeroCotizacion: string;
  estadoCotizacion: CotizacionState;
  zona: string;
  sucursal: string;
  ramo: string;
  subramo: string;
  tipoTasa: TipoTasa;
  tieneSiniestralidad: boolean;
  sector: string;
  directorComercialInfo: DirectorComercialInfo;
  informacionTomador: InformacionTomador;
  gruposAsegurados: GrupoAseguradoFichaTecnica[];
  informacionFactorG: InformacionFactorG;
  // informacionRiesgo: InformacionRiesgo;
  informacionSiniestralidad: InformacionSiniestralidad;
  pygAnual: PyGAnual;
  perfilEdades: PerfilEdades;
  perfilValores: PerfilValores;
}

export interface DirectorComercialInfo {
  usuario: string;
  nombre: string;
  email: string;
}

export interface GenerarFichaTecnicaResponse extends ActionResponseBase {
  data: FichaTecnica;
}
