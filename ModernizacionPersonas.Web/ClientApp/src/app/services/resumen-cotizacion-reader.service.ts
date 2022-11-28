import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ObservableInput, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { environment } from 'src/environments/environment';

import { ProcesarResumenResponse } from '../models/resumen';

@Injectable({
  providedIn: 'root'
})
export class ResumenCotizacionReaderService {
  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  constructor(private httpClient: HttpClient,
    private router: Router,
    private notificationService: NotificationService) { }


  private handleError(err: any): ObservableInput<any> {
    let error = err.status === 500 ? 'Hubo un error generando el resumen de la cotización.' : err.error;
    const dialogRef = this.notificationService.showAlert(error);
    dialogRef.afterClosed().subscribe(result => {
      this.router.navigate(['/cotizaciones']);
    });

    return of(false);
  }

  consultarResumen(codigoCotizacion: number, version: number) {
    const params = new HttpParams().append('version', version.toString());

    const url = `${this.BASE_URL}/${codigoCotizacion}/resumen`;
    return this.httpClient.get<ProcesarResumenResponse>(url, { params: params })
      .pipe(
        map(res => res),
        catchError(err => {
          return this.handleError(err);
        })
      );
  }
}
