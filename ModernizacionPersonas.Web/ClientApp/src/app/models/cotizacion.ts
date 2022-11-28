import { InformacionSiniestralidad } from './fichatecnica';
import { GrupoAsegurado, Siniestralidad } from './grupo-asegurado';
import { InformacionIntermediario } from './informacion-intermediario';
import { Sucursal, Ramo, SubRamo } from '.';
import { CotizacionValidation } from './cotizacion-authorization';
import { ActionResponseBase } from './action-response.base';
import { SiniestralidadModel } from '../pages/configuracion-cotizacion/informacion-siniestralidad/informacion-siniestralidad.component';
import { ApplicationUser } from './application-user';

export enum CotizacionSectionState {
  Pending = 0,
  Completed = 1
}

export interface CotizacionView {
  state: CotizacionSectionState;
}

export interface GastosCompania {
  anno: number;
  porcentaje: number;
}

export interface UtilidadesCompania {
  anno: number;
  base: number;
  tope: number;
}

export interface InformacionIntermediariosViewData {
  intermediarios: InformacionIntermediario[];
}

export interface InformacionGruposAseguradosViewData {
  gruposAsegurados: GrupoAsegurado[];
}

export interface InformacionSiniestralidadViewData {
  informacionSiniestralidad: SiniestralidadResumen[];
}

export interface SiniestralidadResumen {
  codigoSiniestralidad: number;
  codigoCotizacion: number;
  anno: number;
  fechaInicial: Date;
  fechaFinal: Date;
  valorIncurrido: number;
  numeroCasos: number;
}

export enum CotizacionState {
  Open = 1000,
  InProgress = 1100,
  Created = 1101,
  OnDatosTomador = 1102,
  OnInformacionNegocio = 1103,
  OnIntermediarios = 1104,
  OnGruposAsegurados = 1105,
  OnSiniestralidad = 1106,
  OnResumen = 1107,
  OnFichaTecnica = 1108,
  OnSlipConfiguration = 1109,
  Validated = 1110,
  PendingAuthorization = 1111,
  Lookover = 1112, // en revision por usuario cotizador
  ApprovedAuthorization = 1113,
  RefusedAuthorization = 1114,
  OnSlip = 1115,
  Resumed = 1200,
  Sent = 1300,
  Accepted = 1400,

  RejectedByClient = 1500,
  RejectedByCompany = 1600,
  Closed = 1700,
  Issued = 1900, // expedida
  ExpeditionRequest = 1901,
  Expired = 1800
}

export class Cotizacion {
  public codigoCotizacion: number;
  public version?: number;
  public numero: string;
  public usuarioNotificado: string;
  public user: ApplicationUser;
  public lastAuthorId: number;
  public lastAuthorName: string;
  public informacionBasica: InformacionBasicaViewData;
  public informacionBasicaTomador: InformacionBasicaTomadorViewData;
  public informacionNegocio: InformacionNegocioViewData;
  public informacionIntermediarios: InformacionIntermediariosViewData;
  public informacionGruposAsegurados: InformacionGruposAseguradosViewData;
  public informacionSiniestralidad: InformacionSiniestralidadViewData;
  public gastosCompania: GastosCompania;
  public utilidadesCompania: UtilidadesCompania;
  public estado: CotizacionState;
  public readonly: boolean;
  public authorizationInfo?: CotizacionValidation;
  public blocked?: boolean;
  public parametrizacion: any = {
    edadMaximaPermitida: 111
  };

  get requiereConfigurarPerfilEdades() {
    const esTipoTasaRangoEdades = this.informacionNegocio.tipoTasa1 === 3 || this.informacionNegocio.tipoTasa2 === 3;
    return this.informacionBasica.subramo.siNoPerfilEdades && esTipoTasaRangoEdades;
  }

  constructor() {
    this.informacionBasica = new InformacionBasicaViewData();
    this.informacionBasicaTomador = new InformacionBasicaTomadorViewData();
    this.informacionNegocio = new InformacionNegocioViewData();
  }

  updateInformacionSiniestralidad(model: SiniestralidadModel) {

  }
}

export class Amparo {
  codigoAmparo: number;
  codigoGrupoAmparo: number;
  edadMaximaIngreso: number;
  edadMaximaPermanencia: number;
  edadMinimaIngreso: number;
  nombreAmparo: string;
  nombreCortoGrupoAmparo: string;
  nombreGrupoAmparo: string;
  siNoBasico: boolean;
  siNoPorcentajeBasico: boolean;
  siNoAdicional: boolean;
  siNoRequiereEdades: boolean;
  modalidad: Modalidad;
}

export interface Modalidad {
  codigo: number;
  nombre: string;
  valores: ValoresModalidad[];
}

export interface ValoresModalidad {
  tope: number;
  valor: number;
}

export class InformacionBasicaViewData implements CotizacionView {
  public state: CotizacionSectionState;

  public codigoSucursal?: number;
  public sucursal: Sucursal;
  public codigoRamo: number;
  public ramo: Ramo;
  public codigoSubramo: number;
  public subramo: SubRamo;

  constructor() {
  }
}

export class InformacionBasicaTomadorViewData implements CotizacionView {
  public state: CotizacionSectionState;

  public codigoTomador: number;
  public tipoDocumento: string;
  public numeroDocumento: string;
  public primerNombre: string;
  public segundoNombre: string;
  public primerApellido: string;
  public segundoApellido: string;
  public actividadEconomica: string;
  public pais: string;
  public departamento: string;
  public municipio: string;
  public direccion: string;
  public email: string;
  public telefono: string;
  public nombreContacto: string;
  public telefonoContacto1: string;
  public telefonoContacto2: string;
}

export class InformacionNegocioViewData implements CotizacionView {
  public state: CotizacionSectionState;

  public nombreAseguradora: string;
  public fechaInicio: string;
  public fechaFin: string;
  public actividad: string;
  public anyosSiniestralidad: number;
  public periodoFacturacion: string;
  public tipoRiesgo: string;
  public tipoNegocio: string;
  public tipoContratacion: string;
  public sector: string;
  public tipoTasa1: number;
  public tipoTasa2: number;
  public porcentajeRetorno: number;
  public porcentajeOtrosGastos: number;
  public otrosGastos: number;
  public gastosCompania: number;
  public utilidadesCompania: number;
  public porcentajeComision: number;
  public esNegocioDirecto: boolean;
  public conListaAsegurados: boolean;
  public perfilEdad: any;
  public perfilValor: any;
  public lastAuthorId: number;
  public lastAuthorName: string;
  public usuarioDirectorComercial: string;
  public nombreDirectorComercial: string;
  public emailDirectorComercial: string;

  constructor() {
  }
}

export interface PerfilEdad {
  codigoPerfil: number;
  nombrePerfil: string;
}

export interface RangoPerfilEdad {
  codigoPerfil: number;
  orden: number;
  edadDesde: number;
  edadHasta: number;
}

export interface PerfilValor {
  codigoPerfil: number;
  nombrePerfil: string;
}

export interface RangoPerfilValor {
  codigoPerfil: number;
  orden: number;
  valorDesde: number;
  valorHasta: number;
}

export interface ConsultarCotizacionResponse extends ActionResponseBase {
  data: Cotizacion;
}
