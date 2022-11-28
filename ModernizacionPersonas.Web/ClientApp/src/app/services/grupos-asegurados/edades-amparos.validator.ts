import { Injectable } from '@angular/core';
import { CotizacionPersistenceService } from '../cotizacion-persistence.service';
import { GrupoAseguradoWizardService } from '../grupo-asegurado-wizard.service';
import { EdadIngresoPermanenciaAmparo } from 'src/app/models';

@Injectable({
  providedIn: 'root'
})
export class EdadesAmparosValidator {
  constructor(private wizardService: GrupoAseguradoWizardService,
    private cotizacionPersistenceService: CotizacionPersistenceService) { }

  private edadMaximaPermitida = 111; //this.cotizacionPersistenceService.cotizacion.parametrizacion.edadMaximaPermitida;
  private _errors = [];

  get errors() {
    return this._errors;
  }

  validate() {

    return this._errors.length === 0;
  }

  validateOption(row: EdadIngresoPermanenciaAmparo): boolean {
    let result: boolean[] = [];
    result.push(this.validateEdadMaximaMax(row));
    result.push(this.validateEdadMaxEdadMaxPermanencia(row));
    result.push(this.validateEdadMaxEdadMin(row));

    return result.every(x => x);
  }

  private validateEdadMaxEdadMaxPermanencia(row: EdadIngresoPermanenciaAmparo): boolean {
    let id = `${row.amparo.codigoAmparo}_edadMaxGtEedadMaxPerma`;
    if (row.edadMaximaIngreso.valor > row.edadMaximaPermanencia.valor) {
      this.pushError({
        id: id,
        message: 'El valor de edad máxima no puede ser mayor al valor máximo de permanencia.'
      });

      return false;
    }
    else {
      this.removeError(id);
      return true;
    }
  }

  private validateEdadMaxEdadMin(row: EdadIngresoPermanenciaAmparo) {
    let id = `${row.amparo.codigoAmparo}_edadMinGtEedadMax`;
    if (row.edadMaximaIngreso.valor < row.edadMinimaIngreso.valor) {
      this.pushError({
        id: id,
        message: 'El valor mínimo no puede ser mayor al máximo.'
      });

      return false;
    }
    else {
      this.removeError(id);
      return true;
    }
  }

  private validateEdadMaximaMax(row: EdadIngresoPermanenciaAmparo) {
    let id = `${row.amparo.codigoAmparo}_gtEdadMax`;
    if (row.edadMinimaIngreso.valor > this.edadMaximaPermitida
      || row.edadMaximaIngreso.valor > this.edadMaximaPermitida
      || row.edadMaximaPermanencia.valor > this.edadMaximaPermitida) {
      this.pushError({
        id: id,
        message: 'La edad no puede exceder 111.'
      });

      return false;
    }
    else {
      this.removeError(id);
      return true;
    }
  }

  private removeError(errorId: string) {
    let __error = this._errors.find(x => x.id === errorId);
    if (__error) {
      let idx = this._errors.indexOf(__error);
      if (idx >= 0) {
        let __previousError = this._errors.splice(idx, 1);
      }
    }
  }

  private pushError(error: any) {
    let idx = this._errors.findIndex(e => e.id === error.id);
    if (idx >= 0) {
      let __previousError = this._errors.splice(idx, 1);
    }

    this._errors.push(error);
  }
}
