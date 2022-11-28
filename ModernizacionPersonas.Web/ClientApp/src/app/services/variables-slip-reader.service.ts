import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})

export class VariablesSlipReaderService {

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas`;

  constructor(private httpClient: HttpClient) { }

  getVariablesSlip() {
    const url = `${this.BASE_URL}/parametrizacion/slip/variables`
    return this.httpClient.get<any>(url);
  }

  getVariablesSlipByCodigoTexto(codigo: number) {
    const url = `${this.BASE_URL}/parametrizacion/slip/variables?codigoTexto=${codigo}`
    return this.httpClient.get<any>(url);
  }

  getUnsedVariablesSlip() {
    const url = `${this.BASE_URL}/parametrizacion/slip/variables/unused`
    return this.httpClient.get<any>(url);
  }

}
