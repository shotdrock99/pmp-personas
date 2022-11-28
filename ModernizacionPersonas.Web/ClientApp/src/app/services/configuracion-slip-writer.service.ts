import { HttpClient, HttpEventType, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

import { SlipConfiguracion } from '../models/slip-configuracion';
import { EdadAsegurabilidad } from './../models/edad-asegurabilidad';
import { CotizacionPersistenceService } from './cotizacion-persistence.service';
import { map, catchError } from 'rxjs/operators';
import { NotificationService } from '../shared/services/notification.service';
import { ObservableInput, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConfiguracionSlipWriterService {

  constructor(private httpClient: HttpClient,
    private notificationService: NotificationService,
    private cotizacionDataService: CotizacionPersistenceService) { }

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  private handleError(err: any): ObservableInput<any> {
    const dialogRef = this.notificationService.showAlert(err.error);
    dialogRef.afterClosed().subscribe(result => {

    });

    return of(false);
  }

  guardarConfiguracion(model: SlipConfiguracion) {
    const cotizacion = this.cotizacionDataService.cotizacion;
    const url = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/slip/configuracion`;

    const params = new HttpParams().append('version', cotizacion.version.toString());

    return this.httpClient.post(url, model, {
      params: params,
      reportProgress: true,
      observe: 'events'
    }).subscribe(events => {
      if (events.type == HttpEventType.UploadProgress) {
        // show progress
      } else if (events.type === HttpEventType.Response) {
        var response = events.body;
      }
    });
  }

  saveRangoAsegurabilidad(item: EdadAsegurabilidad) {
    const cotizacion = this.cotizacionDataService.cotizacion;
    const url = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/slip/configuracion/asegurabilidad`;

    return this.httpClient.post(url, item)
      .pipe(
        map(res => res),
        catchError(err => {
          return this.handleError(err);
        })
      );
  }

  removeRangoAsegurabilidad(codigoAsegurabilidad: number) {
    const cotizacion = this.cotizacionDataService.cotizacion;
    const url = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/slip/configuracion/asegurabilidad/${codigoAsegurabilidad}`;

    return this.httpClient.delete(url)
      .pipe(
        map(res => res),
        catchError(err => {
          return this.handleError(err);
        })
      );
  }

  updateTasaCotizacion(args: any) {
    const cotizacion = this.cotizacionDataService.cotizacion;
    const url = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/slip/configuracion/tasa`;
    return this.httpClient.post<any>(url, args);
  }
}
