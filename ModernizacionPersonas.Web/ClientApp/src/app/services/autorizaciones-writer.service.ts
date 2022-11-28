import { NotificationService } from './../shared/services/notification.service';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

import { AutorizacionArgs, ChangesArgs, NotifyCotizacionArgs } from '../models/cotizacion-authorization';
import { CotizacionPersistenceService } from './cotizacion-persistence.service';
import { map, catchError } from 'rxjs/operators';
import { ObservableInput, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AutorizacionesWriterService {

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  constructor(
    private httpClient: HttpClient,
    private notificationService: NotificationService,
    private cotizacionDataService: CotizacionPersistenceService) { }

  private handleError(err: any, handler?: any): ObservableInput<any> {
    const dialogRef = this.notificationService.showAlert(err.error);
    dialogRef.afterClosed().subscribe(result => {
      if (handler) {
        handler();
      }
    });

    return of(false);
  }

  notify(args: NotifyCotizacionArgs, handler?: any) {
    const cotizacion = this.cotizacionDataService.cotizacion;
    const url = `${this.BASE_URL}/${args.codigoCotizacion}/authorizations/notify`;
    const params = new HttpParams().append('version', cotizacion.version.toString());

    return this.httpClient.post<any>(url, args)
      .pipe(
        map(res => res),
        catchError(err => {
          return this.handleError(err, handler);
        })
      );
  }

  authorize(args: AutorizacionArgs) {
    const cotizacion = this.cotizacionDataService.cotizacion;
    const url = `${this.BASE_URL}/${args.codigoCotizacion}/authorizations/authorize`;
    const params = new HttpParams().append('version', cotizacion.version.toString());

    return this.httpClient.post<any>(url, args, { params });
  }

  applyChanges(args: ChangesArgs) {
    const cotizacion = this.cotizacionDataService.cotizacion;
    const url = `${this.BASE_URL}/${args.codigoCotizacion}/authorizations/applyChanges`;

    return this.httpClient.post<any>(url, args);
  }

  sortAuthorizations(authorizations: any): any {
    let sortedAuthorizations = [];
    var authoSinGrupos = authorizations.filter((x: any) => x.codigoGrupoAsegurado === 0 && x.codigoTipoAutorizacion != 2);
    var authoClausulas = authorizations.filter((x: any) => x.codigoGrupoAsegurado === 0 && x.codigoTipoAutorizacion == 2);
    var authoConGrupos = authorizations.filter((x: any) => x.codigoGrupoAsegurado != 0 || x.codigoTipoAutorizacion == 0);
    
    authoSinGrupos = authoSinGrupos.sort(function (a, b) {
      if (a.codigoTipoAutorizacion < b.codigoTipoAutorizacion) { return -1; }
      if (a.codigoTipoAutorizacion > b.codigoTipoAutorizacion) { return 1; }
      return 0;
    });
    let group = authoConGrupos.reduce((r, a) => {
      r[a.nombreGrupoAsegurado] = [...r[a.nombreGrupoAsegurado] || [], a];
      return r;
    }, {});
    group = Object.entries(group);
    let prueba = [];
    group.forEach(element => {
      element[1].sort(function (a, b) {
        if (a.nombreAmparo < b.nombreAmparo) { return -1; }
        if (a.nombreAmparo > b.nombreAmparo) { return 1; }
        return 0;
      });
      prueba.push(element[1]);
    });
    authoConGrupos = prueba;
    authoClausulas = authoClausulas.sort(function (a, b) {
      if (a.nombreSeccion < b.nombreSeccion) { return -1; }
      if (a.nombreSeccion > b.nombreSeccion) { return 1; }
      return 0;
    });
    sortedAuthorizations.push(authoSinGrupos);
    sortedAuthorizations.push(authoConGrupos);
    sortedAuthorizations.push(authoClausulas);
    sortedAuthorizations = this.sortAuthorizationsSubItems(sortedAuthorizations.flat());
    /*let authNG = authorizations.filter((x: any) => x.codigoGrupoAsegurado === 0);
    let authNGS = authNG.filter((x: any) => x.siseAuth);
    authNGS = authNGS.sort((a: any, b: any) => {
      return a.codigoTipoAutorizacion - b.codigoTipoAutorizacion || a.mensajeValidacion.localeCompare(b.mensajeValidacion);
    });
    authNGS = authNG.sort(function (a, b) {
      if (a.nombreSeccion < b.nombreSeccion) { return -1; }
      if (a.nombreSeccion > b.nombreSeccion) { return 1; }
      return 0;
    });
    sortedAuthorizations.push(authNGS);
    let authG = authorizations.filter((x: any) => x.codigoGrupoAsegurado !== 0 && x.codigoAutorizacion !== 0);
    const authME = authorizations.filter((x: any) => x.codigoAutorizacion === 0);
    authG = authG.sort((a: any, b: any) => {
      const coverageA = a.nombreAmparo == null ? '' : a.nombreAmparo;
      const coverageB = b.nombreAmparo == null ? '' : b.nombreAmparo;
      return a.nombreGrupoAsegurado.localeCompare(b.nombreGrupoAsegurado) || coverageA.localeCompare(coverageB);
    });
    sortedAuthorizations.push(authME);
    sortedAuthorizations.push(authG);
    let authNGNS = authNG.filter((x: any) => x.siseAuth === false);
    authNGNS = authNGNS.sort((a: any, b: any) => {
      const sectionA = a.nombreSeccion == null ? '' : a.nombreSeccion;
      const sectionB = b.nombreSeccion == null ? '' : b.nombreSeccion;
      return a.codigoTipoAutorizacion - b.codigoTipoAutorizacion || sectionA.localeCompare(sectionB);
    });
    sortedAuthorizations.push(authNGNS);
    sortedAuthorizations = this.sortAuthorizationsSubItems(sortedAuthorizations.flat());*/
    return sortedAuthorizations.flat();
  }

  sortAuthorizationsSubItems(sortedAuthorizations: any): any {
    sortedAuthorizations.forEach((auth: any, index: number) => {
      if (auth.items !== null && auth.items !== undefined) {
        auth.items.forEach((element: any, indexI: number) => {
          if (element.items !== null && element.items !== undefined) {
            sortedAuthorizations[index].items[indexI].items = this.sortAuthorizations(element.items);
          }
        });
      }
    });

    return sortedAuthorizations;
  }
}
