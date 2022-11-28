import { HttpClient, HttpEventType, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

import { CotizacionPersistenceService } from './cotizacion-persistence.service';

@Injectable({
  providedIn: 'root'
})
export class GruposAseguradosReaderService {
  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  constructor(
    private httpClient: HttpClient,
    private cotizacionDataService: CotizacionPersistenceService) { }

  async consultarGrupoAsync(codigoGrupoAsegurado: number) {
    const cotizacion = this.cotizacionDataService.cotizacion;
    const url = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/gruposasegurados/${codigoGrupoAsegurado}`;
    const params = new HttpParams().append('version', cotizacion.version.toString());

    return new Promise(resolve => {
      this.httpClient.get(url, {
        params: params,
        reportProgress: true,
        observe: 'events'
      })
        .subscribe(events => {
          if (events.type == HttpEventType.UploadProgress) {
            // show progress
          } else if (events.type === HttpEventType.Response) {
            let response: any = events.body;
            resolve(response);
          }
        });
    });
  }
}
