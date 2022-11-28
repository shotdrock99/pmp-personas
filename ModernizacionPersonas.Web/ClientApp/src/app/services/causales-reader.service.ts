import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment';
import { CotizacionApiRouterService } from './cotizacion-api-router.service';

@Injectable({
  providedIn: 'root'
})

export class CausalesReaderService {

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas`;

  constructor(private httpClient: HttpClient,
    private cotizacionApiRouterService: CotizacionApiRouterService) { }

    getCausales(){
      const url = `${this.BASE_URL}/causales`
      let result = this.httpClient.get<any>(url);
      return result;
    }
}
