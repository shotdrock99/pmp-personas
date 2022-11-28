import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CotizacionApiRouterService {
  // COTIZACION_API_ENDPOINT: string;
  PARAMETRIZACION_API_ENDPOINT: string;
  // AUTORIZACION_API_ENDPOINT:string;
  USERS_API_ENDPOINT: string;

  constructor() {
    // this.COTIZACION_API_ENDPOINT = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;
    this.PARAMETRIZACION_API_ENDPOINT = `${environment.appSettings.API_ENDPOINT}/parametrizacion`;
    // this.AUTORIZACION_API_ENDPOINT = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones/authorizations`;
    this.USERS_API_ENDPOINT = `${environment.appSettings.API_ENDPOINT}/personas/users`;
  }
}
