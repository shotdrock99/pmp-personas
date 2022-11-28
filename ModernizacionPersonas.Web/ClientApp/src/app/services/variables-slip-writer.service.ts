import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { VariableSlip } from '../models/variable-slip';

@Injectable({
  providedIn: "root",
})

export class VariablesSlipWriterService {

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/parametrizacion`;

  constructor(private httpClient: HttpClient) { }

  editVariableSlip(variable: VariableSlip) {
    const url = `${this.BASE_URL}/slip/variables/${variable.codigoVariable}`;
    return this.httpClient.put(url, variable);
  }

  createVariableSlip(variable: VariableSlip) {
    variable.valorVariable = variable.tipoDato === 'VC' ? -1 : variable.valorVariable
    variable.valorTope = variable.tipoDato === 'VC' ? -1 : variable.valorTope
    const url = `${this.BASE_URL}/slip/variables`;
    return this.httpClient.post(url, variable);
  }
}
