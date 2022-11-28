import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ObservableInput, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

import { NotificationService } from '../shared/services/notification.service';
import { ConsultarCotizacionResponse } from './../models/cotizacion';
import { CotizacionFilter } from './../models/cotizacion-filter';
import { CotizacionApiRouterService } from './cotizacion-api-router.service';

@Injectable({
  providedIn: "root",
})
export class CotizacionReaderService {
  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  constructor(
    private httpClient: HttpClient,
    private router: Router,
    private notificationService: NotificationService
  ) {}

  private handleError(err: any): ObservableInput<any> {
    const dialogRef = this.notificationService.showAlert(err.error);
    dialogRef.afterClosed().subscribe((result) => {
      this.router.navigate(["/cotizaciones"]);
    });

    return of(false);
  }
  private handleErrorWithOutDetail(err: any): ObservableInput<any> {
    const dialogRef = this.notificationService.showAlert("Se ha presentado un error, por favor contacte al administrador");
    dialogRef.afterClosed().subscribe((result) => {
      this.router.navigate(["/cotizaciones"]);
    });

    return of(false);
  }

  consultarCotizaciones(filterArgs: CotizacionFilter) {
    
    const queryString = Object.keys(filterArgs)
      .filter((key) => filterArgs[key] !== null)
      .map((key) => key + "=" + filterArgs[key])
      .join("&");
    const url = queryString ? `${this.BASE_URL}?${queryString}` : this.BASE_URL;
    let result = this.httpClient.get<any>(url);
    return result;
  }

  consultarCotizacion(codigoCotizacion: number, version: number) {
    const url = `${this.BASE_URL}/${codigoCotizacion}`;
    const params = new HttpParams().append("version", version.toString());
    return this.httpClient
      .get<ConsultarCotizacionResponse>(url, { params: params })
      .pipe(
        map((res) => res),
        catchError((err) => {
          return this.handleError(err);
        })
      );
  }

  consultarFirmasRechazoAceptacion(codigoCotizacion: number) {
    const url = `${this.BASE_URL}/${codigoCotizacion}/firmas`;
    return this.httpClient.get<any>(url).pipe(
      map((res) => res),
      catchError((err) => {
        return this.handleError(err);
      })
    );
  }

  validateCotizacion(codigoCotizacion: number, version: number, flag: number) {
    const url = `${this.BASE_URL}/${codigoCotizacion}/validate`;
    const params = new HttpParams().append("version", version.toString()).append("flag", flag.toString());
    return this.httpClient.get<any>(url, { params: params }).pipe(
      map(res => res),
      catchError(err => {
        return this.handleErrorWithOutDetail(err);
      })
    );
  }

  getTransactions(codigoCotizacion: number, version: number) {
    const uri = `${this.BASE_URL}/${codigoCotizacion}/transactions`;
    const params = new HttpParams().append("version", version.toString());
    return this.httpClient.get<any>(uri, { params: params });
  }
}
