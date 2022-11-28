import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

import { CotizacionTransaction } from '../models/cotizacion-authorization';

@Injectable({
  providedIn: 'root'
})
export class TransactionsReaderService {
  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  constructor(private httpClient: HttpClient) { }

  getAuthorizationTransactions(codigoCotizacion: number, version: number) {
    const uri = `${this.BASE_URL}/${codigoCotizacion}/authorizations/transactions`;
    const params = new HttpParams().append('version', version.toString());

    return this.httpClient.get<CotizacionTransaction[]>(uri, { params: params });
  }
}
