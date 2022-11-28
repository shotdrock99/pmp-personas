import { Injectable } from '@angular/core';
import { CotizacionApiRouterService } from './cotizacion-api-router.service';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UsersReaderService {

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas`;

  constructor(private httpClient: HttpClient,
    private cotizacionApiRouterService: CotizacionApiRouterService) { }

  getUsers() {
    const url = this.cotizacionApiRouterService.USERS_API_ENDPOINT;
    let result = this.httpClient.get<any>(url);
    return result;
  }

  validateUser(userName: string){
    const url =  `${this.BASE_URL}/users/${userName}/validate`
    return this.httpClient.get<any>(url);
  }

  validateIntermediario(codigoIntermediario: string) {
    const url =  `${this.BASE_URL}/users/intermediario/${codigoIntermediario}/validate`
    return this.httpClient.get<any>(url);
  }
}


