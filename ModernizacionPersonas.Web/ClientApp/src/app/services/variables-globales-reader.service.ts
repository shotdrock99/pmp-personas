import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})

export class VariablesGlobalesReaderService {
  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas`;

  constructor(private httpClient: HttpClient) { }

  getVariablesGlobales() {
    const url = `${this.BASE_URL}/variablesGlobales`
    return this.httpClient.get<any>(url);
  }

}
