import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Rol } from '../models/rol';

@Injectable({
  providedIn: "root",
})

export class RolesWriterService {

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas`;

  constructor(private httpClient: HttpClient) { }

  editRol(rol: Rol){
    const url = `${this.BASE_URL}/roles/${rol.codigo}`
    return this.httpClient.put(url, rol);
  }

  createRol(rol: Rol){
    const url = `${this.BASE_URL}/roles`
    return this.httpClient.post(url, rol);
  }
}
