import { Injectable } from '@angular/core';
import { Resumen, GrupoAseguradoResumen } from '../models/resumen';
import { Opcione } from 'src/app/models';

@Injectable({
  providedIn: 'root'
})
export class ResumenInlinepreprocessorService {

  private factorg: number;
  private pic: number;
  private pir: number;
  model: Resumen;

  constructor() { }

  private calcularTasaRiesgo(valorAsegurado: number, tasaRiesgo: number) {
    let result = 0;
    if (valorAsegurado > 0) {
      result = tasaRiesgo / valorAsegurado * 1000;
    }

    return result;
  }

  private calcularTasaComercialAnual(valorAsegurado: number, tasaRiesgo: number, factorG: number) {
    let result = 0;
    if (valorAsegurado > 0) {
      result = tasaRiesgo / (1 - (factorG / 100));
    }

    return result;
  }

  private calcularTasaComercialAnualSi(primaTotal: number, valorAseguradoTotal: number) {
    if (valorAseguradoTotal > 0) {
      return primaTotal / valorAseguradoTotal * 1000;
    }
    return 0;
  }
  
  private calcularTasaComercialAplicar(tasaComercialAnual: number, porcentajeDescuento: number, porcentajeRecargo: number) {
    porcentajeDescuento = Number(porcentajeDescuento);
    porcentajeRecargo = Number(porcentajeRecargo);
    return tasaComercialAnual - (tasaComercialAnual * (porcentajeDescuento / 100)) + (tasaComercialAnual * (porcentajeRecargo / 100));
  }

  private calcularValorAseguradoTotal(numeroAsegurados: number, valorAsegurado: number) {
    return numeroAsegurados * valorAsegurado;
  }

  private calcularTasaComercialAplicar2(tasaComercialAnual: number, porcentajeDescuento: number, porcentajeRecargo: number) {
    porcentajeDescuento = Number(porcentajeDescuento);
    porcentajeRecargo = Number(porcentajeRecargo);
    if (porcentajeRecargo > 0) {
      return tasaComercialAnual * (1 - (porcentajeRecargo / 100));
    }

    return tasaComercialAnual * (1 - (porcentajeDescuento / 100));
  }

  private calcularPrimaAnualIndividual(valorAsegurado: number, tasaComercialAplicar: number) {
    return valorAsegurado * tasaComercialAplicar / 1000;
  }

  private calcularPrimaAnualTotal(codigoTipoSumaAsegurada: number, valorAsegurado: number, tasaComercialAplicar: number, numeroAsegurados: number, primaAnualIndividual: number) {
    const esSumaFija = codigoTipoSumaAsegurada === 1;
    if (esSumaFija) {
      return numeroAsegurados * primaAnualIndividual;
    }

    return tasaComercialAplicar * valorAsegurado / 1000;
  }

  private calcularFactorG(model: any) {
    const _gastosCompania = parseFloat(model.gastosCompania);
    const _utilidadCompania = parseFloat(model.utilidad);
    const _comision = parseFloat(model.comision);
    const _gastosRetorno = parseFloat(model.gRetorno);
    const _otrosGastos = parseFloat(model.otrosGastos);
    const _porcentajeIvaComision = parseFloat(model.ivaComision);
    const _porcentajeIvaRetorno = parseFloat(model.ivagRetorno);

    let result = _gastosCompania + _utilidadCompania + _comision + _porcentajeIvaComision + _gastosRetorno + _porcentajeIvaRetorno + _otrosGastos;
    let res = this.round(result, 2);
    return res;
  }

  private calcularPIC(model: Resumen) {
    let result = model.comision * (model.porcentajeIvaComision / 100);
    let res = this.round(result, 2);
    return res
  }

  private calcularPIR(model: Resumen) {
    let result = model.gRetorno * (model.porcentajeIvaRetorno / 100);
    let res = this.round(result, 2);
    return res
  }

  private round(num, dec) {
    let result = Math.round((num + Number.EPSILON) * 100) / 100
    return result;
  }

  private updateGruposAsegurados(gruposAsegurados: GrupoAseguradoResumen[], esSiniestralidad: boolean) {
    gruposAsegurados.forEach(x => {
      this.updateOpciones(x.tipoSumaAsegurada.codigoTipoSumaAsegurada, x.opciones, x.numeroAsegurados, esSiniestralidad);
    })
  }
  private updateOpciones(codigoTipoSumaAsegurada: number, opciones: Opcione[], numeroAsegurados: number, esSiniestralidad: boolean) {
    opciones.forEach(x => {
      const factorg = this.factorg;
      if (!esSiniestralidad) {
        //x.tasaComercialAnual = this.calcularTasaComercialAnual(x.valorAsegurado, x.tasaRiesgo, factorg);
        x.tasaComercialAnual = this.calcularTasaComercialAnualSi(x.primaAnualTotal, x.valorAseguradoTotal);
        x.tasaComercialAplicar = this.calcularTasaComercialAplicar(x.tasaComercialAnual, x.porcentajeDescuento, x.porcentajeRecargo);
        x.primaAnualIndividual = this.calcularPrimaAnualIndividual(x.valorAsegurado, x.tasaComercialAplicar);
        x.primaAnualTotal = this.calcularPrimaAnualTotal(codigoTipoSumaAsegurada, x.valorAsegurado, x.tasaComercialAplicar, numeroAsegurados, x.primaAnualIndividual);
        x.tasaComercialAnual = this.calcularTasaComercialAnualSi(x.primaAnualTotal, x.valorAseguradoTotal);
        x.tasaComercialAplicar = this.calcularTasaComercialAplicar(x.tasaComercialAnual, x.porcentajeDescuento, x.porcentajeRecargo);
      } else {
        //x.siniestralidad.tasaComercial = this.calcularTasaComercialAnual(x.valorAsegurado, x.siniestralidad.tasaRiesgo, factorg);
        x.siniestralidad.tasaComercial = this.calcularTasaComercialAnualSi(x.primaAnualTotal, x.valorAseguradoTotal);
        x.siniestralidad.tasaComercialAplicar = this.calcularTasaComercialAplicar(x.tasaComercialAnual, x.porcentajeDescuento, x.porcentajeRecargo);
        x.siniestralidad.primaAnualIndividual = this.calcularPrimaAnualIndividual(x.valorAsegurado, x.tasaComercialAplicar);
        x.siniestralidad.primaAnualTotal = this.calcularPrimaAnualTotal(codigoTipoSumaAsegurada, x.valorAsegurado, x.tasaComercialAplicar, numeroAsegurados, x.primaAnualIndividual);
        x.siniestralidad.tasaComercial = this.calcularTasaComercialAnualSi(x.primaAnualTotal, x.valorAseguradoTotal);
        x.siniestralidad.tasaComercialAplicar = this.calcularTasaComercialAplicar(x.tasaComercialAnual, x.porcentajeDescuento, x.porcentajeRecargo);
      }
    })
  }

  private updateOpcion(codigoTipoSumaAsegurada: number, opcion: Opcione, numeroAsegurados: number, esSiniestralidad: boolean) {
    const factorg = this.factorg;
    if (!esSiniestralidad) {
      opcion.tasaComercialAnual = this.calcularTasaComercialAnualSi(opcion.primaAnualTotal, opcion.valorAseguradoTotal);
      opcion.tasaComercialAplicar = this.calcularTasaComercialAplicar(opcion.tasaComercialAnual, opcion.porcentajeDescuento, opcion.porcentajeRecargo);
      opcion.primaAnualIndividual = this.calcularPrimaAnualIndividual(opcion.valorAsegurado, opcion.tasaComercialAplicar);
      opcion.primaAnualTotal = this.calcularPrimaAnualTotal(codigoTipoSumaAsegurada, opcion.valorAsegurado, opcion.tasaComercialAplicar, numeroAsegurados, opcion.primaAnualIndividual);
    } else {
      opcion.siniestralidad.tasaComercial = this.calcularTasaComercialAnual(opcion.valorAsegurado, opcion.siniestralidad.tasaRiesgo, factorg);
      opcion.siniestralidad.tasaComercialAplicar = this.calcularTasaComercialAplicar(opcion.tasaComercialAnual, opcion.porcentajeDescuento, opcion.porcentajeRecargo);
      opcion.siniestralidad.primaAnualIndividual = this.calcularPrimaAnualIndividual(opcion.valorAsegurado, opcion.tasaComercialAplicar);
      opcion.siniestralidad.primaAnualTotal = this.calcularPrimaAnualTotal(codigoTipoSumaAsegurada, opcion.valorAsegurado, opcion.tasaComercialAplicar, numeroAsegurados, opcion.primaAnualIndividual);
    }
  }

  process(model: Resumen, esSiniestralidad: boolean) {
    this.model = Object.assign({}, model);
    // recalcular factorg
    this.pic = this.calcularPIC(model);
    this.model.ivaComision = this.pic;
    this.pir = this.calcularPIR(model);
    this.model.ivagRetorno = this.pir;
    this.factorg = this.calcularFactorG(model);
    this.model.factorG = this.factorg;
    this.updateGruposAsegurados(this.model.gruposAsegurados, esSiniestralidad);

    return this.model;
  }

  processOpcion(indiceOpcion: number, model: Resumen, esSiniestralidad: boolean) {
    this.model = Object.assign({}, model);
    model.gruposAsegurados.forEach(x => {
      let opcion = x.opciones.filter(z => z.indiceOpcion === indiceOpcion);
      this.updateOpcion(x.tipoSumaAsegurada.codigoTipoSumaAsegurada, opcion[0], x.numeroAsegurados, esSiniestralidad)
    })
    return this.model;
  }
}
