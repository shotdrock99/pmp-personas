import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ObservableInput, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { environment } from 'src/environments/environment';

import { GuardarResumenArgs } from '../models/guardar-resumen-args';
import { CotizacionApiRouterService } from './cotizacion-api-router.service';
import { CotizacionPersistenceService } from './cotizacion-persistence.service';

@Injectable({
  providedIn: 'root'
})
export class ResumenCotizacionWriterService {

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  constructor(private httpClient: HttpClient,
    private cotizacionPersistenceService: CotizacionPersistenceService,
    private notificationService: NotificationService) { }

  private handleError(err: any): ObservableInput<any> {
    const dialogRef = this.notificationService.showAlert(err.error);
    dialogRef.afterClosed().subscribe(result => {
      // TODO
      // redirect
    });

    return of(false);
  }


  SaveResumen(model: GuardarResumenArgs) {
    const cotizacion = this.cotizacionPersistenceService.cotizacion;
    const url = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/resumen`;
    return this.httpClient.post(url, model)
      .pipe(
        map(res => res),
        catchError(err => {
          return this.handleError(err);
        })
      );
  }

  SaveResumen1(model: any) {
    const cotizacion = this.cotizacionPersistenceService.cotizacion;
    const url = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/resumen`;
    return this.httpClient.post(url, model)
      .pipe(
        map(res => res),
        catchError(err => {
          return this.handleError(err);
        })
      );
  }
}
