import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})

export class ParametrizacionSlipReaderService {

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas`;

  constructor(private httpClient: HttpClient) { }

  getSeccionesSlip() {
    const url = `${this.BASE_URL}/parametrizacion/slip/secciones`
    return this.httpClient.get<any>(url);
  }

  getTextosSlip() {
    const url = `${this.BASE_URL}/parametrizacion/slip/textos`
    return this.httpClient.get<any>(url);
  }

}
