import { CotizacionState, CotizacionSectionState } from './../models/cotizacion';
import { Injectable } from '@angular/core';
import { forkJoin } from 'rxjs';
import { PersonasReaderService } from 'src/app/services/personas-reader.service';
import { Amparo, Cotizacion, PerfilEdad, PerfilValor, TipoSumaAsegurada, TipoNegocio } from '../models';
import { PeriodoFacturacion } from './../models/periodo-facturacion';
import { TipoDocumento } from './../models/tipo-documento';
import { CotizacionReaderService } from './cotizacion-reader.service';
import { ParametrizacionReaderService } from './parametrizacion-reader.service';
import { Sector } from '../models/informacion-cliente';
import { ParametrizacionApp } from '../models/parametrizacion-app';

const ESTADOS = [{
  codigoEstado: 1106,
  nombreEstado: 'En Cotización'
},
{
  codigoEstado: 1107,
  nombreEstado: 'En Resumen'
},
{
  codigoEstado: 1108,
  nombreEstado: 'En Ficha Técnica'
},
{
  codigoEstado: 1109,
  nombreEstado: 'En Configuración Slip'
},
{
  codigoEstado: 1111,
  nombreEstado: 'Pendiente de Autorización'
},
{
  codigoEstado: 1112,
  nombreEstado: 'Devuelta para revisión'
},
{
  codigoEstado: 1114,
  nombreEstado: 'No Autorizada'
},
{
  codigoEstado: 1113,
  nombreEstado: 'Autorizada'
},
{
  codigoEstado: 1115,
  nombreEstado: 'En Slip'
},
{
  codigoEstado: 1300,
  nombreEstado: 'Enviada'
},
{
  codigoEstado: 1400,
  nombreEstado: 'Aceptada'
},
{
  codigoEstado: 1500,
  nombreEstado: 'Rechazada: Por cliente'
},
{
  codigoEstado: 1600,
  nombreEstado: 'Rechazada: Por compañía'
},
{
  codigoEstado: 1700,
  nombreEstado: 'Cerrada'
},
{
  codigoEstado: 1800,
    nombreEstado: 'Cerrada/Vencida'
},
{
  codigoEstado: 1900,
  nombreEstado: 'Expedida'
},
{
  codigoEstado: 1901,
  nombreEstado: 'Solicitud Expedición'
}
];

@Injectable({
  providedIn: 'root'
})
export class CotizacionPersistenceService {

  cotizacion: Cotizacion;
  amparos: Amparo[];
  tasas: any[];
  perfilesEdad: PerfilEdad[];
  perfilesValor: PerfilValor[];
  isEdit: boolean;
  tiposSumaAsegurada: TipoSumaAsegurada[];
  tiposRiesgo: any[];
  tiposNegocio: TipoNegocio[];
  sectores: Sector[];
  periodosFacturacion: PeriodoFacturacion[];
  tiposDocumento: TipoDocumento[];
  paramatrizacionSiniestralidad: ParametrizacionApp;
  SMMLV: number;
  readonly: boolean;

  get estados(): any[] {
    return ESTADOS;
  }

  get hasMultiplesTasas(): boolean {
    return this.cotizacion.informacionNegocio.tipoTasa1 > 0 && this.cotizacion.informacionNegocio.tipoTasa2 > 0;
  }

  get hasSiniestralidad(): boolean {
    return this.cotizacion.informacionNegocio.tipoTasa1 === 5 || this.cotizacion.informacionNegocio.tipoTasa2 === 5;
  }

  constructor(
    private personasReaderService: PersonasReaderService,
    private parametrizacionReaderService: ParametrizacionReaderService,
    private cotizacionReaderService: CotizacionReaderService) { }

  crearCotizacion(model: any, codigoCotizacion: number, version: number, numeroCotizacion: string): Cotizacion {
    this.cotizacion = new Cotizacion();
    this.cotizacion.codigoCotizacion = codigoCotizacion;
    this.cotizacion.version = version;
    this.cotizacion.numero = numeroCotizacion;

    this.cotizacion.informacionBasica.sucursal = model.Sucursal;
    this.cotizacion.informacionBasica.ramo = model.Ramo;
    this.cotizacion.informacionBasica.subramo = model.Subramo;

    const codigoRamo = this.cotizacion.informacionBasica.ramo.codigoRamo;
    const codigoSubramo = this.cotizacion.informacionBasica.subramo.codigoSubRamo;
    const codigoSector = 1;

    this.resolveDependencies(codigoRamo, codigoSubramo, codigoSector);

    return this.cotizacion;
  }

  private resolveDependencies(codigoRamo: number, codigoSubramo: number, codigoSector: number) {
    const tiposSumaAseguradaPromise = this.personasReaderService.getTiposSumaAsegurada(codigoRamo, codigoSubramo);
    // const amparosPromise = this.personasReaderService.getAmparos(codigoRamo, codigoSubramo);
    const tasasPromise = this.personasReaderService.getTasas(codigoRamo, codigoSubramo, codigoSector);

    forkJoin([tiposSumaAseguradaPromise, tasasPromise])
      .subscribe(res => {
        this.tiposSumaAsegurada = res[0];
        this.tasas = res[1];
      });
  }

  resolveCotizacionData(codigoCotizacion: number, version: number, resolveDependencies: boolean): Promise<Cotizacion> {
    this.isEdit = false;
    this.cotizacion = new Cotizacion();
    const informacionNegocio = this.cotizacionReaderService.consultarCotizacion(codigoCotizacion, version);
    return new Promise((resolve, reject) => {
      if (resolveDependencies) {
        const codigoRamo = 15;
        const codigoSubramo = 1;
        const tiposRiesgoPromise = this.personasReaderService.getTiposRiesgo();
        const tiposNegocioPromise = this.personasReaderService.getTiposNegocio();
        const sectoresPromise = this.personasReaderService.getSectores(codigoRamo, codigoSubramo);
        const perfilesEdadPromise = this.personasReaderService.getPerfilesEdad();
        const perfilesValorPromise = this.personasReaderService.getPerfilesValor();
        const periodosFacturacionPromise = this.parametrizacionReaderService.getPeriodoFacturacion();
        const tiposDocumentoPromise = this.personasReaderService.getTiposDocumento();
        const paramatrizacionSiniestralidadPromise = this.parametrizacionReaderService.getSiniestralidad();
        const SMMLVPromise = this.parametrizacionReaderService.getSMMLV();

        forkJoin([tiposRiesgoPromise, tiposNegocioPromise, sectoresPromise, perfilesEdadPromise, perfilesValorPromise,
          periodosFacturacionPromise, tiposDocumentoPromise, paramatrizacionSiniestralidadPromise, SMMLVPromise])
          .subscribe(res => {
            this.tiposRiesgo = res[0];
            this.tiposNegocio = res[1];
            this.sectores = res[2];
            this.perfilesEdad = res[3];
            this.perfilesValor = res[4];
            this.periodosFacturacion = res[5];
            this.tiposDocumento = res[6];
            this.paramatrizacionSiniestralidad = res[7];
            this.SMMLV = res[8];
          });
      }
      if (codigoCotizacion > 0) {
        this.cotizacionReaderService.consultarCotizacion(codigoCotizacion, version)
          .subscribe(res => {
            if (!res) { return; }
            res.data.readonly = res.readonly;
            this.cotizacion = Object.assign(this.cotizacion, res.data);
            const codigoRamo = this.cotizacion.informacionBasica.ramo.codigoRamo;
            const codigoSubramo = this.cotizacion.informacionBasica.subramo.codigoSubRamo;
            const codigoSector = 1;
            const sectoresPromise = this.personasReaderService.getSectores(codigoRamo, codigoSubramo);
            const tiposSumaAseguradaPromise = this.personasReaderService.getTiposSumaAsegurada(codigoRamo, codigoSubramo);
            const amparosPromise = this.personasReaderService.getAmparos(codigoRamo, codigoSubramo, codigoSector);
            const tasasPromise = this.personasReaderService.getTasas(codigoRamo, codigoSubramo, codigoSector);
            forkJoin([tiposSumaAseguradaPromise, amparosPromise, tasasPromise, sectoresPromise])
              .subscribe(result => {
                this.tiposSumaAsegurada = result[0];
                this.amparos = result[1];
                this.tasas = result[2];
                this.sectores = result[3];
                this.isEdit = true;
                resolve(this.cotizacion);
              });
          });
      } else {
        resolve(this.cotizacion);
      }
    });
  }

  updateInformacionNegocio(model: any) {
    const informacionNegocio = this.cotizacion.informacionNegocio;
    informacionNegocio.nombreAseguradora = model.NombreAseguradora;
    informacionNegocio.fechaInicio = model.FechaInicio;
    informacionNegocio.fechaFin = model.FechaFin;
    informacionNegocio.periodoFacturacion = model.PeriodoFacturacion;
    informacionNegocio.tipoRiesgo = model.TipoRiesgo;
    informacionNegocio.tipoNegocio = model.TipoNegocio;
    informacionNegocio.tipoContratacion = model.TipoContratacion;
    informacionNegocio.sector = model.Sector;
    informacionNegocio.tipoTasa1 = model.TipoTasa1;
    informacionNegocio.tipoTasa2 = model.TipoTasa2;
    informacionNegocio.porcentajeRetorno = model.PorcentajeRetorno;
    informacionNegocio.porcentajeOtrosGastos = model.PorcentajeOtrosGastos;
    informacionNegocio.otrosGastos = model.OtrosGastos;
    informacionNegocio.porcentajeComision = model.PorcentajeComision;
    informacionNegocio.esNegocioDirecto = model.EsNegocioDirecto;
    informacionNegocio.conListaAsegurados = model.ConListaAsegurados;
    informacionNegocio.perfilEdad = model.PerfilEdad;
    informacionNegocio.perfilValor = model.PerfilValor;
    informacionNegocio.actividad = model.Actividad;
    informacionNegocio.anyosSiniestralidad = model.AnyosSiniestralidad;

    // update state Informacion de Negocio
    informacionNegocio.state = CotizacionSectionState.Completed;
  }

  loadAmparos(selection: any) {
    const codigoRamo = this.cotizacion.informacionBasica.ramo.codigoRamo;
    const codigoSubramo = this.cotizacion.informacionBasica.subramo.codigoSubRamo;
    const codigoSector = selection;

    this.personasReaderService.getAmparos(codigoRamo, codigoSubramo, codigoSector)
      .subscribe(res => {
        this.amparos = res;
      });
  }

  setCotizacionState(codigoEstado: CotizacionState) {
    this.cotizacion.estado = codigoEstado;
  }

  reset() {
    this.isEdit = false;
    this.cotizacion = new Cotizacion();
  }
}
