import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from "@angular/core";
import { Observable } from 'rxjs';
import { RolUsuarioPersonas } from '../models/usuario-personas';

@Injectable({
  providedIn: 'root'
})

export class RolesUsuarioReaderService {

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas`;

  constructor(private httpClient: HttpClient) { }

  getRolesUsuario(){
    const url = `${this.BASE_URL}/roles`
    return this.httpClient.get<any>(url);
  }

}
