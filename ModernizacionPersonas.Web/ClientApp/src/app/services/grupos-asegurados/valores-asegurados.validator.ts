import { GrupoAseguradoWizardService } from '../grupo-asegurado-wizard.service';
import { Injectable } from '@angular/core';
import { Amparo, TipoSumaAsegurada } from 'src/app/models';

@Injectable({
  providedIn: 'root'
})
export class ValoresAseguradosValidator {

  constructor(private wizardService: GrupoAseguradoWizardService) { }

  private errorsMain = [];
  private errorsAN = [];

  get errors() {
    return this.errorsMain.every(x => this.validInsuredValues(x));
  }

  removeAllErrors(): void {
    this.errorsMain = [];
    this.errorsAN = [];
  }

  validate() {
    return this.errorsMain.every(x => this.validInsuredValues(x));
  }

  validateOption(args: any): boolean {
    
    const tipoSumaAsegurada: TipoSumaAsegurada = args.tipoSumaAsegurada;
    // fetch opction values
    const optionValues = args.opcion.map((x: any) => {
      this.wizardService.calculoBasePorValorAsegurado = x.calculoBasePorValorAsegurado;
      const propertyName = this.getPropertyName(tipoSumaAsegurada, x.amparo);
      const capturaSalario = propertyName === 'numeroSalariosOption';
      const capturaValor = propertyName === 'valorOption';
      const catchDays = propertyName === 'valorDiarioOption';
      const esBasicoNoAdicional = !x.amparo.siNoAdicional && x.amparo.siNoBasico;
      const capturaPorcentaje = x.amparo.siNoPorcentajeBasico;
      const propertyValue = this.getPropertyValue(tipoSumaAsegurada, x, propertyName);
      let res = propertyValue > 0 || (!this.wizardService.basicoNoAdicionalRequired && esBasicoNoAdicional);
      if (this.wizardService.calculaValorAseguradoONumeroSalarios && propertyValue > 0 || capturaSalario) {
        res = true;
      }
      // si es SMMLV
      if (tipoSumaAsegurada.codigoTipoSumaAsegurada === 10) {
        if (tipoSumaAsegurada.codigoTipoSumaAsegurada === 10 && propertyValue > 0) {
          res = true;
        } else {
          res = false;
        }
      }
      if(tipoSumaAsegurada.codigoTipoSumaAsegurada === 2){
        if (tipoSumaAsegurada.codigoTipoSumaAsegurada === 2 && propertyValue > 0) {
          res = true;
        } else {
          res = false;
        }
      }
      if (tipoSumaAsegurada.codigoTipoSumaAsegurada !== 2 && capturaValor) {
        if (x.maxValue[0] && propertyValue > x.maxValue[0]) {
          res = false;
        }
      }
      if (catchDays) {
        const propertyValueDays = this.getPropertyValue(tipoSumaAsegurada, x, 'numeroDiasOption');
        if (propertyValueDays <= 0) {
          res = false;
        }
        if (x.maxValue[1] && propertyValueDays > x.maxValue[1]) {
          res = false;
        }
        if (x.maxValue[0] && propertyValue > x.maxValue[0]) {
          res = false;
        }
      }
      
      return res;
    });
    const optionValuesAN = args.opcion.filter((x: any) => x.amparo.modalidad.codigo !== 4).map((x: any) => {
      this.wizardService.calculoBasePorValorAsegurado = x.calculoBasePorValorAsegurado;
      const propertyName = this.getPropertyName(tipoSumaAsegurada, x.amparo);
      const capturaSalario = propertyName === 'numeroSalariosOption';
      const capturaValor = propertyName === 'valorOption';
      const esBasicoNoAdicional = !x.amparo.siNoAdicional && x.amparo.siNoBasico;
      const propertyValue = this.getPropertyValue(tipoSumaAsegurada, x, propertyName);
      let res = propertyValue > 0 || (!this.wizardService.basicoNoAdicionalRequired && esBasicoNoAdicional);
      if (this.wizardService.calculaValorAseguradoONumeroSalarios && propertyValue > 0 || capturaSalario) {
        res = true;
      }
      // si es SMMLV
      if (tipoSumaAsegurada.codigoTipoSumaAsegurada === 10 && propertyValue > 0) {
        res = true;
      } else {
        res = false;
      }
      if (tipoSumaAsegurada.codigoTipoSumaAsegurada !== 2 && capturaValor) {
        if (x.maxValue[0] && propertyValue > x.maxValue[0]) {
          res = false;
        }
      }
      return res;
    });
    
    // la forma es valida si todos los valores de los amparos fueron asignados (*true) en la opcion
    const isValid = optionValues.every((x: boolean) => x);
    let isValidAN = optionValuesAN.every((x: boolean) => x);
    //
    if(args.opcion.filter((x: any) => x.amparo.modalidad.codigo == 4).length > 0){
      isValidAN = true;
    }
    const isEmpty = optionValues.every((x: boolean) => !x);
    const errorName = `invalid-opt-${args.optionIndex}`;
    // si la opcion es invalida, agregue el error a la lista
    // !isValid ? this.pushError(errorName, !isValid, isEmpty) : this.removeError(errorName);
    // !isValidAN ? this.pushErrorAN(errorName, !isValidAN, isEmpty) : this.removeErrorAN(errorName);
    this.pushError(errorName, !isValid, isEmpty);
    this.pushErrorAN(errorName, !isValidAN, isEmpty);
    this.wizardService.isValoresAseguradosValid = this.errorsMain.every(x => this.validInsuredValues(x));
    this.wizardService.isValoresAseguradosANValid = this.errorsAN.every(x => this.validInsuredValues(x));
    return isValid;
  }

  private getPropertyValue(tipoSumaAsegurada: TipoSumaAsegurada, x: any, propertyName: string): any {
    let result = x.valor[propertyName].rawValue;
    // SMMLV
    if (tipoSumaAsegurada.codigoTipoSumaAsegurada === 10) {
      let key: string;
      const value1 = x.valor[key = 'valorOption'].rawValue;
      const value2 = x.valor[key = 'numeroSalariosOption'].rawValue;
      result = result > 0 ? result : value1 > 0 ? value1 : value2 > 0 ? value2 : 0;
    }
    return result;
  }

  private getPropertyName(tipoSumaAsegurada: TipoSumaAsegurada, amparo: Amparo): string {
    let propertyName = 'valorOption';
    if (amparo.siNoPorcentajeBasico) {
      propertyName = 'porcentajeOption';
    } else {
      if (this.wizardService.capturaSalarios) {
        if (!this.wizardService.calculoBasePorValorAsegurado) {
          propertyName = 'numeroSalariosOption';
        }
      }
    }
    // si el amparo es del grupo de Asistencias (codigo grupo 3)
    if (amparo.codigoGrupoAmparo === 3) {
      propertyName = 'primaOption';
    }
    if (amparo.modalidad.codigo === 4) {
      propertyName = 'valorDiarioOption';
    }

    return propertyName;
  }

  private removeError(errorName: string) {
    const error = this.errorsMain.find(x => x.name === errorName);
    if (error) {
      const idx = this.errorsMain.indexOf(error);
      const previousError = this.errorsMain.splice(idx, 1);
    }
  }

  private removeErrorAN(errorName: string) {
    const error = this.errorsAN.find(x => x.name === errorName);
    if (error) {
      const idx = this.errorsAN.indexOf(error);
      const previousError = this.errorsAN.splice(idx, 1);
    }
  }

  private pushError(errorName: string, isInvalid: boolean, isEmpty: boolean) {
    const error: any = {};
    let key: string;
    error[key = 'name'] = errorName;
    error[key = 'invalid'] = isInvalid;
    error[key = 'empty'] = isEmpty;
    error[key = 'required'] = errorName === 'invalid-opt-1';
    const idx = this.errorsMain.findIndex(e => e.name === errorName);
    if (idx >= 0) {
      const previousError = this.errorsMain.splice(idx, 1);
    }
    this.errorsMain.push(error);
  }

  private pushErrorAN(errorName: string, isInvalid: boolean, isEmpty: boolean) {
    const error: any = {};
    let key: string;
    error[key = 'name'] = errorName;
    error[key = 'invalid'] = isInvalid;
    error[key = 'empty'] = isEmpty;
    error[key = 'required'] = errorName === 'invalid-opt-1';
    const idx = this.errorsAN.findIndex(e => e.name === errorName);
    if (idx >= 0) {
      const previousError = this.errorsAN.splice(idx, 1);
    }
    this.errorsAN.push(error);
  }

  private validInsuredValues(error: any) {
    let returnValue = false;
    if (error.empty && !error.required) {
      returnValue = true;
    } else {
      if (!error.invalid) {
        returnValue = true;
      }
    }
    return returnValue;
  }
}
