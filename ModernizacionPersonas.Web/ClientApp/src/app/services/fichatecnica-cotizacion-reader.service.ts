import { GenerarFichaTecnicaResponse } from './../models/fichatecnica';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { CotizacionApiRouterService } from './cotizacion-api-router.service';
import { version } from 'punycode';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class FichatecnicaCotizacionReaderService {
  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  constructor(private httpClient: HttpClient) {}

  consultarFichaTecnica(codigoCotizacion: number, version: number) {
    const url = `${this.BASE_URL}/${codigoCotizacion}/fichatecnica`;
    const params = new HttpParams().append('version', version.toString());

    let result = this.httpClient.get<GenerarFichaTecnicaResponse>(url, { params: params });
    return result;
  }
}
