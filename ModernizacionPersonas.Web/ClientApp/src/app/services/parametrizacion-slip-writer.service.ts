import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { SeccionSlip } from "../models/seccion-slip";
import { TextoSlip } from '../models/texto-slip';

@Injectable({
  providedIn: "root",
})

export class ParametrizacionSlipWriterService {

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas`;

  constructor(private httpClient: HttpClient) { }

  createSeccionSlip(seccion: SeccionSlip) {
    seccion.especial = seccion.especial ? 1 : 0;
    const url = `${this.BASE_URL}/parametrizacion/slip/secciones`;
    return this.httpClient.post(url, seccion);
  }

  editSeccionSlip(seccion: SeccionSlip) {
    seccion.especial = seccion.especial ? 1 : 0;
    const url = `${this.BASE_URL}/parametrizacion/slip/secciones/${seccion.codigo}`;
    return this.httpClient.put(url, seccion);
  }

  createTextoSlip(model: TextoSlip) {
    const url = `${this.BASE_URL}/parametrizacion/slip/textos`;
    return this.httpClient.post(url, model);
  }

  editTextoSlip(model: TextoSlip) {
    const url = `${this.BASE_URL}/parametrizacion/slip/textos/${model.codigo}`;
    return this.httpClient.put(url, model);
  }
}
