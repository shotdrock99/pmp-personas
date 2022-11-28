import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

@Injectable({
  providedIn: "root",
})

export class PermisosReaderService {
  
  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas`;

  constructor(private httpClient: HttpClient) {}

  getPermisos() {
    const url = `${this.BASE_URL}/roles/permisos`;
    return this.httpClient.get<any>(url);
  }
}
