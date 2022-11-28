import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ObservableInput, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

import { CotizacionItemList } from '../models/cotizacion-item-list';
import { ConfirmCotizacionArgs } from '../pages/lista-cotizaciones/lista-cotizaciones.component';
import { NotificationService } from '../shared/services/notification.service';

@Injectable({
  providedIn: 'root'
})
export class CotizacionWriterService {

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  constructor(private httpClient: HttpClient,
    private router: Router,
    private notificationService: NotificationService) { }

  private handleError(err: any): ObservableInput<any> {
    const dialogRef = this.notificationService.showAlert(err.error || "Se presentó un error en la ejecución del proceso, intente nuevamente.");
    dialogRef.afterClosed().subscribe(result => {
      this.router.navigate(['/cotizaciones']);
    });

    return of(false);
  }

  initializeCotizacion(codigoSucursal, codigoRamo, codigoSubramo, codigoZona) {
    var sModel = {
      codigoSucursal: codigoSucursal,
      codigoRamo: codigoRamo,
      codigoSubramo: codigoSubramo,
      codigoZona: codigoZona
    };

    return this.httpClient.post(this.BASE_URL, sModel)
      .pipe(
        map(res => {
          return res;
        }),
        catchError(err => {
          return this.handleError(err);
        })
      );
  }

  copyCotizacion(codigoCotizacion: number, version: number) {
    const url = `${this.BASE_URL}/${codigoCotizacion}/copy`;
    const params = new HttpParams().append('version', version.toString());

    return this.httpClient.post(url, {}, { params: params });
  }

  copyAltCotizacion(codigoCotizacion: number, version: number) {
    const url = `${this.BASE_URL}/${codigoCotizacion}/copyalt`;
    const params = new HttpParams().append('version', version.toString());

    return this.httpClient.post(url, {}, { params: params });
  }

  createVersionCotizacion(codigoCotizacion: number, version: number) {
    const url = `${this.BASE_URL}/${codigoCotizacion}/version`;
	const params = new HttpParams().append('version', version.toString());

    return this.httpClient.post(url, {}, { params: params }).pipe(
      map((res) => res),
      catchError((err) => {
        return this.handleError(err);
      })
    );
  }

  confirmCotizacion(args: ConfirmCotizacionArgs) {
    const url = `${this.BASE_URL}/${args.codigoCotizacion}/confirm`;
    return this.httpClient.post(url, args);
  }

  continueCotizacion(codigoCotizacion: number, reason: string) {
    const url = `${this.BASE_URL}/${codigoCotizacion}/continue`;
    const args = {
      reason: reason
    };

    return this.httpClient.post(url, args)
      .pipe(
        map(res => res),
        catchError(err => {
          return this.handleError(err);
        })
      );
  }

  lockCotizacion(codigoCotizacion: number, version: number) {
    const url = `${this.BASE_URL}/${codigoCotizacion}/lock`;
    const params = new HttpParams()
      .append('version', version.toString());

    return this.httpClient.post(url, {}, { params: params });
  }

  unlockCotizacion(codigoCotizacion: number, version: number) {
    const url = `${this.BASE_URL}/${codigoCotizacion}/unlock`;
    return this.httpClient.post(url, {});
  }

  sendSlipCotizacion(args: any) {
    const url = `${this.BASE_URL}/${args.codigoCotizacion}/slip/send`;

    return this.httpClient.post(url, args)
      .pipe(
        map(res => res),
        catchError(err => {
          return this.handleError(err);
        })
      );
  }

  generateExpedicion(args: any) {
    const url = `${this.BASE_URL}/${args.codigoCotizacion}/expedicion`;
    return this.httpClient.post(url, args).pipe(
      map((res) => res),
      catchError((err) => {
        return this.handleError(err);
      })
    );
  }
}

export enum ConfirmacionCotizacion {
  accepted = 1,
  rejected,
  rejectedByClient,
  rejectedByCompany
}
