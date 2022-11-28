import { environment } from '../../environments/environment';
import { Injectable } from '@angular/core';
import { CotizacionApiRouterService } from "./cotizacion-api-router.service";
import { HttpClient } from "@angular/common/http";
import { Causal } from '../models/causal';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})

export class CausalesWriterService {

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas`;

  constructor(private httpClient: HttpClient) { }

  createCausal(causal: Causal) {
    const url = `${this.BASE_URL}/causales`
    return this.httpClient.post(url, causal);
  }

  disableCausal(codigoCausal: number) {
    const url = `${this.BASE_URL}/causales/${codigoCausal}`
    return this.httpClient.delete(url);
  }

  updateCausal(causal: Causal) {
    causal.activo = causal.activo ? 1 : 0;
    causal.externo = causal.externo ? 1 : 0;
    causal.solidaria = causal.solidaria ? 1 : 0;
    const url = `${this.BASE_URL}/causales/${causal.codigoCausal}`
    return this.httpClient.put(url, causal);
  }
}
