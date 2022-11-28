import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ObservableInput, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

import { NotificationService } from '../shared/services/notification.service';
import { CotizacionFilter } from '../models/cotizacion-filter';

@Injectable({
  providedIn: 'root'
})
export class AutorizacionesReaderService {

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  constructor(
    private httpClient: HttpClient,
    private router: Router,
    private notificationService: NotificationService) { }

  private handleError(err: any): ObservableInput<any> {
    const dialogRef = this.notificationService.showAlert('Se presentó un error consultando la información, intente nuevamente.');
    dialogRef.afterClosed().subscribe(result => {
      this.router.navigate(['/cotizaciones']);
    });

    return of(false);
  }

  consultarCotizaciones(filterArgs: CotizacionFilter) {
    const queryString = Object.keys(filterArgs).filter(key => filterArgs[key] !== null).map(key => key + '=' + filterArgs[key]).join('&');
    const url = queryString ? `${this.BASE_URL}/authorizations?${queryString}` : `${this.BASE_URL}/authorizations`;
    const result = this.httpClient.get<any>(url);
    return result;
  }

  getCotizacion(codigoCotizacion: number) {
    const url = `${this.BASE_URL}/authorizations/${codigoCotizacion}`;
    const result = this.httpClient.get<any>(url);
    return result;
  }

  consultarCotizacion(codigoCotizacion: number, version: number) {
    const url = `${this.BASE_URL}/${codigoCotizacion}`;
    const params = new HttpParams().append('version', version.toString());

    const result = this.httpClient.get<any>(url, { params });
    return result;
  }

  getAuthorizationControls(codigoCotizacion: number, version: number) {
    const url = `${this.BASE_URL}/${codigoCotizacion}/authorizations/controls`;
    const params = new HttpParams().append('version', version.toString());

    return this.httpClient.get<any>(url, { params })
      .pipe(
        map(res => res),
        catchError(err => {
          return this.handleError(err);
        })
      );
  }

  getAuthorizationUsers(codigoCotizacion: number, version: number) {
    const url = `${this.BASE_URL}/${codigoCotizacion}/authorizations/users`;
    const params = new HttpParams().append('version', version.toString());

    return this.httpClient.get<any>(url, { params });
  }
}
