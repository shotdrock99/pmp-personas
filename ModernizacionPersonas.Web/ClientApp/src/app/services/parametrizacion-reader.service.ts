import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Sucursal, Ramo, SubRamo } from '../models';
import { TipoDocumento } from '../models/tipo-documento';
import { Pais } from '../models/pais';
import { Departamento } from '../models/departamento';
import { Municipio } from '../models/municipio';
import { ActividadEconomica } from '../models/actividad-economica';
import { TipoNegocio } from '../models/tipo-negocio';
import { PeriodoFacturacion } from '../models/periodo-facturacion';
import { environment } from '../../environments/environment';
import { CotizacionApiRouterService } from './cotizacion-api-router.service';
import { ParametrizacionApp } from '../models/parametrizacion-app';
import { Observable } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ParametrizacionReaderService {

  constructor(
    private httpClient: HttpClient,
    private cotizacionApiRouterService: CotizacionApiRouterService) { }

  private getSucursales() {
    const result = this.httpClient.get<Sucursal[]>(`${this.cotizacionApiRouterService.PARAMETRIZACION_API_ENDPOINT}/sucursales`);
    return result;
  }
  private getRamosPorSucursal(codSucursal: number) {
    const result = this.httpClient.get<Ramo[]>(
      `${this.cotizacionApiRouterService.PARAMETRIZACION_API_ENDPOINT}/sucursales/${codSucursal}/ramos`);
    return result;
  }
  private getRamos() {
    const result = this.httpClient.get<Ramo[]>(`${this.cotizacionApiRouterService.PARAMETRIZACION_API_ENDPOINT}/ramos`);
    return result;
  }
  private getRamo(codRamo: number) {
    const result = this.httpClient.get<Ramo>(`${this.cotizacionApiRouterService.PARAMETRIZACION_API_ENDPOINT}/ramos/${codRamo}`);
    return result;
  }
  private getSubRamosPorRamo(codRamo: number) {
    const result = this.httpClient.get<SubRamo[]>(
      `${this.cotizacionApiRouterService.PARAMETRIZACION_API_ENDPOINT}/ramos/${codRamo}/subramos`);
    return result;
  }
  private getTiposSumaAsegurada() {
    const result = this.httpClient.get<Ramo[]>(`${this.cotizacionApiRouterService.PARAMETRIZACION_API_ENDPOINT}/ramos`);
    return result;
  }
  // Obtiene tipos de documento
  private getTipoDocumento() {
    const result = this.httpClient.get<TipoDocumento[]>(`${this.cotizacionApiRouterService.PARAMETRIZACION_API_ENDPOINT}/tipodocumento`);
    return result;
  }
  // Obtiene posibles tipos de Actividad Economica
  private getActividadEconomica() {
    const result = this.httpClient.get<ActividadEconomica[]>(
      `${this.cotizacionApiRouterService.PARAMETRIZACION_API_ENDPOINT}/actividadeconomica`);
    return result;
  }
  private getTipoRiesgo() {
    const result = this.httpClient.get<any[]>(`${this.cotizacionApiRouterService.PARAMETRIZACION_API_ENDPOINT}/tiponegocio`);
    return result;
  }
  // Obtiene posibles tipos de Negocio
  private getTiposNegocio() {
    const result = this.httpClient.get<TipoNegocio[]>(`${this.cotizacionApiRouterService.PARAMETRIZACION_API_ENDPOINT}/tiponegocio`);
    return result;
  }
  // Obtiene listado de paises
  getPaises() {
    const result = this.httpClient.get<Pais[]>(`${this.cotizacionApiRouterService.PARAMETRIZACION_API_ENDPOINT}/paises`);
    return result;
  }
  // Obtiene listado de departamentos
  getDepartamentos() {
    const result = this.httpClient.get<Departamento[]>(`${this.cotizacionApiRouterService.PARAMETRIZACION_API_ENDPOINT}/departamentos`);
    return result;
  }
  // Obtiene listado de periodos de facturación
  getPeriodoFacturacion() {
    const result = this.httpClient.get<PeriodoFacturacion[]>(
      `${this.cotizacionApiRouterService.PARAMETRIZACION_API_ENDPOINT}/periodofacturacion`);
    return result;
  }
  // Obtiene lsitado de Municipios dependiendo el Dpartamento seleccionado
  getMunicipiosPorDepartamento(codDepartamento: number) {
    const result = this.httpClient.get<Municipio[]>(
      `${this.cotizacionApiRouterService.PARAMETRIZACION_API_ENDPOINT}/municipios?codDepartamento=${codDepartamento}`);
    return result;
  }

  getSiniestralidad(): Observable<any> {
    return this.httpClient.get<ParametrizacionApp[]>(
      `${this.cotizacionApiRouterService.PARAMETRIZACION_API_ENDPOINT}/siniestralidad`).pipe(
        map((data: ParametrizacionApp[]) => data[0])
      );
  }

  getSMMLV(): Observable<number> {
    return this.httpClient.get<number>(`${this.cotizacionApiRouterService.PARAMETRIZACION_API_ENDPOINT}/smmlv`);
  }
}
