import { Injectable } from '@angular/core';
import { AmparoGrupo } from 'src/app/models/grupo-asegurado-server';

import {
  Amparo,
  EdadIngresoPermanenciaAmparo,
  GrupoAsegurado,
  GrupoAseguradoCompleted,
  GrupoAseguradoExtended,
  OpcionValorAsegurado,
  ValorAseguradoAmparo,
} from '../models';
import { CotizacionPersistenceService } from './cotizacion-persistence.service';

@Injectable({
  providedIn: 'root'
})
export class GrupoAseguradoMapperService {
  private grupoAsegurado: GrupoAsegurado;
  SMMLV: number;

  constructor(private cotizacionPersistenceService: CotizacionPersistenceService) { }

  map(grupoAsegurado: GrupoAseguradoExtended, model: GrupoAseguradoCompleted): any {
    this.grupoAsegurado = grupoAsegurado;
    const tipoEstructura =
      model.asegurados.tipoEstructura === 'tipoUno'
      ? 1 : model.asegurados.tipoEstructura === 'tipoDos'
      ? 2 : model.asegurados.tipoEstructura === 'tipoTres'
      ? 3 : 4;
    const result = {
      codigoGrupoAsegurado: grupoAsegurado.codigoGrupoAsegurado,
      nombreGrupoAsegurado: grupoAsegurado.nombreGrupoAsegurado,
      codigoTipoSuma: grupoAsegurado.tipoSumaAsegurada.codigoTipoSumaAsegurada,
      valorMinAsegurado: grupoAsegurado.valorMinAsegurado || 0,
      valorMaxAsegurado: grupoAsegurado.valorMaxAsegurado || 0,
      numeroSalariosAsegurado: grupoAsegurado.numeroSalariosAsegurado || 0,
      conListaAsegurados: grupoAsegurado.conListaAsegurados,
      conDistribucionAsegurados: model.asegurados.conDistribucionAsegurados,
      tipoEstructura,
      valorAsegurado: model.asegurados.valorAsegurado,
      numeroAsegurados: model.asegurados.numeroAsegurados,
      aseguradosOpcion1: model.asegurados.aseguradosOpcion1 || 0,
      aseguradosOpcion2: model.asegurados.aseguradosOpcion2 || 0,
      aseguradosOpcion3: model.asegurados.aseguradosOpcion3 || 0,
      edadPromedioAsegurados: model.asegurados.edadPromedio,
      porcentajeAsegurados: model.asegurados.porcentajeAsegurados,
      numeroPotencialAsegurados: model.asegurados.numeroPotencialAsegurados || 0,
      // codigoCotizacion: this.cotizacion.CodigoCotizacion,
      amparosGrupo: this.mapAmparos(grupoAsegurado, model),
      rangosGrupo: this.mapRangos(model, grupoAsegurado.codigoGrupoAsegurado)
    };

    return result;
  }

  extendModel(grupo: GrupoAsegurado, info: any): GrupoAseguradoExtended {
    const amparosExtended = this.extendAmparos(info);
    const tipoEstructura =
      info.tipoEstructura === 1 || info.tipoEstructura === 0
      ? 'tipoUno'
      : info.tipoEstructura === 2
      ? 'tipoDos'
      : info.tipoEstructura === 3
      ? 'tipoTres'
      : 'tipoUno';
    const model: GrupoAseguradoExtended = {
      aseguradosOpcion1: info.aseguradosOpcion1,
      aseguradosOpcion2: info.aseguradosOpcion2,
      aseguradosOpcion3: info.aseguradosOpcion3,
      codigoCotizacion: info.codigoCotizacion,
      codigoGrupoAsegurado: grupo.codigoGrupoAsegurado,
      nombreGrupoAsegurado: grupo.nombreGrupoAsegurado,
      conDistribucionAsegurados: info.conDistribucionAsegurados,
      conListaAsegurados: grupo.conListaAsegurados,
      tipoEstructura,
      tipoSumaAsegurada: grupo.tipoSumaAsegurada,
      valorMaxAsegurado: info.valorMaxAsegurado,
      valorMinAsegurado: info.valorMinAsegurado,
      numeroSalariosAsegurado: info.numeroSalariosAsegurado,
      amparos: amparosExtended,
      valorAsegurado: info.valorAsegurado,
      numeroAsegurados: info.numeroAsegurados,
      edadPromedioAsegurados: info.edadPromedioAsegurados,
      porcentajeAsegurados: info.porcentajeAsegurados,
      rangosGrupo: info.rangosGrupo
    };

    return model;
  }

  private extendAmparos(info: any) {
    const amparos = this.cotizacionPersistenceService.amparos;
    const result: AmparoGrupo[] = [];
    info.amparosGrupo.forEach((a: AmparoGrupo) => {
      const amparo = amparos.find(x => x.codigoAmparo === a.codigoAmparo);
      a.codigoGrupoAmparo = amparo.codigoGrupoAmparo;
      a.edadMaximaIngreso = amparo.edadMaximaIngreso;
      a.edadMaximaPermanencia = amparo.edadMaximaPermanencia;
      a.edadMinimaIngreso = amparo.edadMinimaIngreso;
      a.nombreAmparo = amparo.nombreAmparo;
      a.nombreCortoGrupoAmparo = amparo.nombreCortoGrupoAmparo;
      a.nombreGrupoAmparo = amparo.nombreGrupoAmparo;
      a.siNoAdicional = amparo.siNoAdicional;
      a.siNoBasico = amparo.siNoBasico;
      a.siNoPorcentajeBasico = amparo.siNoPorcentajeBasico;
      a.siNoRequiereEdades = amparo.siNoRequiereEdades;
      result.push(a);
    });

    return result;
  }

  private mapAmparos(grupoAsegurado: GrupoAsegurado, model: GrupoAseguradoCompleted) {
    // si el tipo de suma asegurada es Suma Variable por Asegurado ó Saldo Deudores Ahorros Aportes
    const capturaAmparoBasicoNoAdicional = grupoAsegurado.tipoSumaAsegurada.codigoTipoSumaAsegurada !== 3
      && grupoAsegurado.tipoSumaAsegurada.codigoTipoSumaAsegurada !== 6;
    // si el grupo se configura por numero de salarios con valor asegurado por grupo
    const capturaSalarios = grupoAsegurado.tipoSumaAsegurada.codigoTipoSumaAsegurada === 2;
    if (!capturaAmparoBasicoNoAdicional || capturaSalarios) {
      grupoAsegurado.valorAsegurado = model.asegurados.valorAsegurado;
    }

    const result = [];
    const valorAseguradoAmparoBNA = model.valoresAsegurados.find(x => x.amparo.siNoBasico && !x.amparo.siNoAdicional);
    // si es tipo suma asegurada SMMLV, calcule el valor asegurado con el valor del SMMLV
    if (grupoAsegurado.tipoSumaAsegurada.codigoTipoSumaAsegurada === 10) {
      this.SMMLV = grupoAsegurado.tipoSumaAsegurada.valorSalarioMinimo || 0;
      const opcion = valorAseguradoAmparoBNA.opciones[0];
      if (opcion.numeroSalariosOption.rawValue > 0) {
        opcion.valorOption.rawValue = opcion.numeroSalariosOption.rawValue * this.SMMLV;
      }
    }

    const filteredList = model.valoresAsegurados.filter(x => Object.keys(x.amparo).length !== 0);
    filteredList.forEach((x: ValorAseguradoAmparo) => {
      const edadesGrupo = model.edadesAmparos.find(g => g.amparo.codigoAmparo === x.amparo.codigoAmparo);
      const o = {
        codigoAmparoGrupoAsegurado: 0,
        codigoGrupoAsegurado: grupoAsegurado.codigoGrupoAsegurado,
        codigoAmparo: x.amparo.codigoAmparo,
        esAmparoBasicoNoAdicional: x.amparo.siNoBasico && !x.amparo.siNoAdicional,
        opcionesValores: this.mapOpcionesValores(x.amparo, x.opciones, valorAseguradoAmparoBNA, grupoAsegurado.valorAsegurado),
        edadesGrupo: this.mapEdadesGrupo(grupoAsegurado, edadesGrupo)
      };

      result.push(o);
    });

    return result;
  }

  private mapOpcionesValores(amparo: Amparo, opciones: OpcionValorAsegurado[], valorAseguradoAmparoBNA: ValorAseguradoAmparo,
                             valorAseguradoGrupo?: number) {
    const result = [];
    opciones.forEach((o, idx) => {
      const r = {
        codigoOpcionValorAsegurado: 0,
        codigoAmparoGrupoAsegurado: 0,
        porcentajeCobertura: 0,
        numeroSalarios: 0,
        tasa: 0,
        valorAsegurado: o.valorOption.rawValue,
        prima: 0,
        numeroDias: 0,
        valorDiario: 0,
        valorAseguradoDias: 0
      };

      // es amparo basico no adicional
      const esAmparoBasicoNoAdicional = amparo.siNoBasico && !amparo.siNoAdicional;
      // define si la captura de valores asegurados es por salarios o valor
      const configuracionPorSalarios = valorAseguradoAmparoBNA.opciones[idx].numeroSalariosOption.rawValue > 0;
      const calculaPorSMMLV = this.grupoAsegurado.tipoSumaAsegurada.codigoTipoSumaAsegurada === 10;
      // si el tipo de suma asegurada es Suma variable Asegurado ó Saldo Deudores-Ahorros-Aportes
      const capturaAmparoBasicoNoAdicional = this.grupoAsegurado.tipoSumaAsegurada.codigoTipoSumaAsegurada !== 3 &&
        this.grupoAsegurado.tipoSumaAsegurada.codigoTipoSumaAsegurada !== 6;
      const esAsistencia = amparo.codigoGrupoAmparo === 3;
      const esMultiploSueldos = this.grupoAsegurado.tipoSumaAsegurada.codigoTipoSumaAsegurada === 2;
      const esSumaFijaMultiploSueldos = this.grupoAsegurado.tipoSumaAsegurada.codigoTipoSumaAsegurada === 5;
      const esSMMLV = this.grupoAsegurado.tipoSumaAsegurada.codigoTipoSumaAsegurada === 10;
      const esCombinado = this.grupoAsegurado.tipoSumaAsegurada.codigoTipoSumaAsegurada === 5;

      // si el valor asegurado fue capturado manualmente para el grupo,
      // asigne el valor a la ocion dependiendo del tipo de la misma; valor / porcentaje
      if (esAmparoBasicoNoAdicional && !capturaAmparoBasicoNoAdicional || configuracionPorSalarios && !esSMMLV && !esCombinado) {
        r.valorAsegurado = valorAseguradoGrupo;
      }

      if (esMultiploSueldos || esSumaFijaMultiploSueldos || esSMMLV) {
        // r.numeroSalarios = Math.round(o.numeroSalariosOption.rawValue);
        r.numeroSalarios = o.numeroSalariosOption.rawValue;
        if ((esMultiploSueldos /*|| esSumaFijaMultiploSueldos*/) && !amparo.siNoPorcentajeBasico) {
          r.valorAsegurado = valorAseguradoGrupo * r.numeroSalarios;
        }

        if (configuracionPorSalarios && !esAsistencia && calculaPorSMMLV) {
          if (o.numeroSalariosOption.rawValue > 0) {
            if (r.valorAsegurado > 0) {
              r.valorAsegurado = r.numeroSalarios * this.SMMLV;
            }
          }
        }
      }

      if (esAsistencia) {
        r.prima = o.primaOption.rawValue || 0;
      }

      if (amparo.siNoPorcentajeBasico) {
        r.porcentajeCobertura = o.porcentajeOption.rawValue || 0;
        let valorAseguradoBNA = valorAseguradoAmparoBNA.opciones[idx].valorOption.rawValue;
        if (!capturaAmparoBasicoNoAdicional) {
          valorAseguradoBNA = valorAseguradoGrupo;
        }

        if (valorAseguradoBNA) {
          r.valorAsegurado = valorAseguradoBNA * (r.porcentajeCobertura / 100);
        }
      }

      if (amparo.modalidad.codigo === 4) {
        r.numeroDias = o.numeroDiasOption.rawValue;
        r.valorDiario = o.valorDiarioOption.rawValue;
        r.valorAseguradoDias = r.numeroDias * r.valorDiario;
      }

      result.push(r);
    });

    return result;
  }

  private mapEdadesGrupo(grupoAsegurado: GrupoAsegurado, edadesAmparo: EdadIngresoPermanenciaAmparo) {
    const result = {
      codigoEdadPermanencia: 0,
      codigoGrupoAsegurado: grupoAsegurado.codigoGrupoAsegurado,
      codigoAmparo: edadesAmparo.amparo.codigoAmparo,
      edadMinAsegurado: edadesAmparo.edadMinimaIngreso.valor || 0,
      edadMaxAsegurado: edadesAmparo.edadMaximaIngreso.valor || 0,
      diasCarencia: edadesAmparo.numeroDiasCarencia.valor || 0,
      edadMaxPermanencia: edadesAmparo.edadMaximaPermanencia.valor || 0,
    };

    return result;
  }

  private mapRangos(model: GrupoAseguradoCompleted, codigoGrupoAsegurado: number) {
    const result = [];
    const rangos = model.asegurados.rangos;
    rangos.forEach(x => {
      const o = {
        codigoGrupoAsegurado,
        edadMinAsegurado: x.edadMinAsegurado,
        edadMaxAsegurado: x.edadMaxAsegurado,
        numeroAsegurados: x.numeroAsegurados,
        valorAsegurado: x.valorAsegurado
      };

      result.push(o);
    });

    return result;
  }
}
