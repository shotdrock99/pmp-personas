import { SoligesproUser } from './../models/soligespro-user';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Ramo, SubRamo, TipoNegocio, TipoDocumento, Amparo, TipoSumaAsegurada, PerfilEdad, PerfilValor, RangoPerfilEdad, RangoPerfilValor, Sucursal } from '../models';
import { environment } from '../../environments/environment';
import { CotizacionApiRouterService } from './cotizacion-api-router.service';
import { Observable, throwError, ObservableInput, of } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { NotificationService } from '../shared/services/notification.service';
@Injectable({
  providedIn: 'root'
})
export class PersonasReaderService {
  constructor(private httpClient: HttpClient,
    private cotizacionApiRouterService: CotizacionApiRouterService,
    private notificationService: NotificationService
    ) { }

  get token() { return localStorage.getItem('jwt'); }

  private handleError(error) {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      // Get client-side error
      errorMessage = error.error.message;
    } else {
      // Get server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }

    window.alert(errorMessage);
    return throwError(errorMessage);
  }
  public handleWarning(err: any): ObservableInput<any> {
    const dialogRef = this.notificationService.showAlert(err);
    dialogRef.afterClosed().subscribe((result) => {
      console.log("Tomador bloqueado desde Vetos")
    });

    return of(false);
  }

  getZonas(): Observable<any> {
    const url = `${environment.appSettings.API_ENDPOINT}/personas/zonas`;
    return this.httpClient.get<any>(url)
      .pipe(
        retry(1),
        catchError(this.handleError)
      );
  }
  getSucursales(codigoZona: number = 0): Observable<Sucursal[]> {
    const url = `${environment.appSettings.API_ENDPOINT}/personas/sucursales?codigoZona=${codigoZona}`;
    return this.httpClient.get<Sucursal[]>(url)
      .pipe(
        retry(1),
        catchError(this.handleError)
      );
  }
  getRamosPorSucursal(codSucursal: number) {
    const url = `${environment.appSettings.API_ENDPOINT}/personas/sucursales/${codSucursal}/ramos`;
    return this.httpClient.get<Ramo[]>(url);
  }
  getRamos() {
    const url = `${environment.appSettings.API_ENDPOINT}/personas/ramos`;
    return this.httpClient.get<Ramo[]>(url);
  }
  getRamo(codRamo: number) {
    const url = `${environment.appSettings.API_ENDPOINT}/personas/ramos/${codRamo}`;
    return this.httpClient.get<Ramo>(url);
  }
  getSubRamosPorRamo(codRamo: number) {
    const url = `${environment.appSettings.API_ENDPOINT}/personas/ramos/${codRamo}/subramos`;
    return this.httpClient.get<SubRamo[]>(url);
  }
  getTiposRiesgo() {
    const url = `${environment.appSettings.API_ENDPOINT}/personas/tiposriesgo`;
    return this.httpClient.get<any[]>(url);
  }
  getSectores(codigoRamo: number, codigoSubramo: number) {
    const url = `${environment.appSettings.API_ENDPOINT}/personas/sectores?codigoRamo=${codigoRamo}&codigoSubramo=${codigoSubramo}`;
    return this.httpClient.get<any[]>(url);
  }
  getTasas(codigoRamo: number, codigoSubramo: number, codigoSector: number) {
    const url = `${environment.appSettings.API_ENDPOINT}/personas/tasas?codigoRamo=${codigoRamo}&codigoSubramo=${codigoSubramo}&codigoSector=${codigoSector}`;
    return this.httpClient.get<any[]>(url);
  }
  getTiposContratacion() {
    const url = `${environment.appSettings.API_ENDPOINT}/personas/tiposcontratacion`;
    return this.httpClient.get<any[]>(url);
  }
  getTiposContratacionxNegocio(codigoTipoNegocio: number) {
    return this.httpClient.get<any[]>(`${environment.appSettings.API_ENDPOINT}/personas/tiposcontratacion?codigoTipoNegocio=${codigoTipoNegocio}`);
  }
  getTiposNegocio() {
    return this.httpClient.get<TipoNegocio[]>(`${environment.appSettings.API_ENDPOINT}/personas/tiposnegocio`);
  }
  getTiposDocumento() {
    return this.httpClient.get<TipoDocumento[]>(`${environment.appSettings.API_ENDPOINT}/personas/tiposdocumento`);
  }
  getAmparos(codigoRamo: number, codigoSubramo: number, codigoSector) {
    return this.httpClient.get<Amparo[]>(`${environment.appSettings.API_ENDPOINT}/personas/amparos?codigoRamo=${codigoRamo}&codigoSubramo=${codigoSubramo}&codigoSector=${codigoSector}`);
  }
  getAmparosxRamo(codigoRamo: number) {
    return this.httpClient.get<Amparo[]>(`${environment.appSettings.API_ENDPOINT}/personas/amparos?codigoRamo=${codigoRamo}`);
  }
  consultarTomador(codigoTipoDocumento: number, numeroDocumento: string) {
    return this.httpClient.get<any>(`${environment.appSettings.API_ENDPOINT}/personas/tomador?codigoTipoDocumento=${codigoTipoDocumento}&numeroDocumento=${numeroDocumento}`);
  }
  consultarIntermediario(codigoSucursal: number, codigoIntermediario: number) {
    return this.httpClient.get<any>(`${environment.appSettings.API_ENDPOINT}/personas/intermediario?codigoSucursal=${codigoSucursal}&codigoIntermediario=${codigoIntermediario}`);
  }
  consultarIntermediarioPorDocumento(codigoSucursal: number, codigoTipoDocumento: number, numeroDocumento: string) {
    return this.httpClient.get<any>(`${environment.appSettings.API_ENDPOINT}/personas/intermediariopordocumento?codigoSucursal=${codigoSucursal}&codigoTipoDocumento=${codigoTipoDocumento}&numeroDocumento=${numeroDocumento}`);
  }
  getDirectoresComerciales(codigoSucursal: number) {
    return this.httpClient.get<SoligesproUser[]>(`${environment.appSettings.API_ENDPOINT}/personas/directorescomerciales?codigoSucursal=${codigoSucursal}`);
  }
  getTiposSumaAsegurada(codigoRamo: number, codigoSubramo: number) {
    return this.httpClient.get<TipoSumaAsegurada[]>(`${environment.appSettings.API_ENDPOINT}/personas/tipossumaasegurada?codigoramo=${codigoRamo}&codigoSubramo=${codigoSubramo}`);
  }
  getPerfilesEdad() {
    return this.httpClient.get<PerfilValor[]>(`${environment.appSettings.API_ENDPOINT}/personas/perfiles/edad`);
  }
  getRangosPorPerfilEdad(codigoPerfil: number) {
    return this.httpClient.get<RangoPerfilEdad[]>(`${environment.appSettings.API_ENDPOINT}/personas/perfiles/edad/rangos?codigoPerfil=${codigoPerfil}`);
  }
  getPerfilesValor() {
    return this.httpClient.get<PerfilEdad[]>(`${environment.appSettings.API_ENDPOINT}/personas/perfiles/valor`);
  }
  getRangosPorPerfilValor(codigoPerfil: number) {
    return this.httpClient.get<RangoPerfilValor[]>(`${environment.appSettings.API_ENDPOINT}/personas/perfiles/valor/rangos?codigoPerfil=${codigoPerfil}`);
  }
}
