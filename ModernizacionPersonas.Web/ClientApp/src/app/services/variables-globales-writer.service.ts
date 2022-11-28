import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { ParametrizacionApp } from '../models/parametrizacion-app';

@Injectable({
  providedIn: 'root'
})

export class VariablesGlobalesWriterService {
  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas`;

  constructor(private httpClient: HttpClient) { }

  editVariablesGlobales(variable: ParametrizacionApp) {
    const url = `${this.BASE_URL}/variablesGlobales/${variable.codigoVariable}`
    return this.httpClient.put<any>(url, variable);
  }

}
