import { CurrencyPipe } from '@angular/common';
import { Injectable } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { Subject } from 'rxjs';
import { AmparoGrupo, OpcionValor } from 'src/app/models/grupo-asegurado-server';

import {
  Amparo,
  EdadIngresoPermanenciaAmparo,
  EdadIngresoPermanenciaAmparoField,
  GrupoAsegurado,
  GrupoAseguradoExtended,
  OpcionValorAsegurado,
  TipoSumaAsegurada,
  ValorAseguradoAmparo,
  ValorOption,
} from '../models';
import { GrupoAseguradoMapperService } from './grupo-asegurado-mapper.service';
import { GrupoAseguradosFormFactory } from './grupos-asegurados-form.factory';
import {
  EdadesAmparoValidator,
  ValorAseguradoOpcionesValidator,
  ValorAseguradoOpcionValidator,
} from './grupos-asegurados/grupo-asegurados.validators';
import { GruposAseguradosWriterService } from './gruposasegurados-writer.service';


@Injectable({
  providedIn: 'root'
})
export class GrupoAseguradoWizardService {

  private Form: FormGroup;
  private Dirty: boolean;
  private ListAmparos: Amparo[];
  private GrupoAsegurado: GrupoAseguradoExtended = new GrupoAseguradoExtended();
  private EdadesAmparos: FormArray;
  private Amparos: FormArray;

  tipoSumaAsegurada: TipoSumaAsegurada;

  currencyPipe = new CurrencyPipe('en-US');
  basicoNoAdicionalRequired: boolean;
  esSumaFijaMultiploSueldos: boolean;
  esMultiploSueldos: boolean;
  esSMMLV: boolean;
  capturaSalarios: boolean;
  calculaValorAseguradoONumeroSalarios: boolean;

  isValoresAseguradosValidChange: Subject<boolean> = new Subject<boolean>();
  isEdadesAmparosValidChange: Subject<boolean> = new Subject<boolean>();
  calculoBasePorValorAsegurado: boolean;
  isValoresAseguradosValid = false;
  isValoresAseguradosANValid = false;
  isEdadesAmparosValid = false;
  isAseguradosValid = false;

  get formGroup() {
    return this.Form;
  }

  get amparos() {
    return this.ListAmparos;
  }

  get amparosArray() {
    return this.Form.get('amparos') as FormArray;
  }

  get valoresAseguradosArray() {
    return this.Form.get('valoresAsegurados') as FormArray;
  }

  get aseguradosArray() {
    return this.Form.get('asegurados') as FormArray;
  }

  get grupoAsegurado() {
    return this.GrupoAsegurado;
  }

  get builtAmparos() {
    return this.Amparos;
  }

  constructor(
    private formBuilder: FormBuilder,
    private gruposAseguradosWriterService: GruposAseguradosWriterService,
    private mapperService: GrupoAseguradoMapperService) {
    this.isValoresAseguradosValidChange.subscribe((value) => {
      this.isValoresAseguradosValid = value;
    });

    this.isEdadesAmparosValidChange.subscribe((value) => {
      this.isEdadesAmparosValid = value;
    });
  }

  private getEdadesGrupoPropertyName(propertyName: string) {
    return propertyName === 'edadMinimaIngreso' ? 'edadMinAsegurado' :
      propertyName === 'edadMaximaIngreso' ? 'edadMaxAsegurado' :
        propertyName === 'edadMinimaIngreso' ? 'edadMinAsegurado' :
          propertyName === 'edadMaximaPermanencia' ? 'edadMaxPermanencia' :
            'diasCarencia';
  }

  private mapEdadesAmparos(amparosGrupos: AmparoGrupo[]): EdadIngresoPermanenciaAmparo[] {
    const result: EdadIngresoPermanenciaAmparo[] = [];

    amparosGrupos.forEach(x => {
      const amparo = this.ListAmparos.find(a => a.codigoAmparo === x.codigoAmparo);
      const o: EdadIngresoPermanenciaAmparo = {
        amparo,
        edadMaximaIngreso: {
          valor: x.edadesGrupo.edadMaxAsegurado,
          disabled: false
        },
        edadMaximaPermanencia: {
          valor: x.edadesGrupo.edadMaxPermanencia,
          disabled: false
        },
        edadMinimaIngreso: {
          valor: x.edadesGrupo.edadMinAsegurado,
          disabled: false
        },
        numeroDiasCarencia: {
          valor: x.edadesGrupo.diasCarencia,
          disabled: false
        }
      };

      result.push(o);
    });

    return result;
  }

  private mapValoresAsegurados(amparosGrupos: AmparoGrupo[]): ValorAseguradoAmparo[] {
    const result: ValorAseguradoAmparo[] = [];

    amparosGrupos.forEach(a => {
      const amparo = this.ListAmparos.find(( am: Amparo) => am.codigoAmparo === am.codigoAmparo);
      const opciones: OpcionValorAsegurado[] = [];
      a.opcionesValores.forEach((o, idx) => {
        const r: OpcionValorAsegurado = {};
        if (amparo.codigoGrupoAmparo === 3) {
          r.primaOption = {
            value: o.prima.toString(),
            rawValue: o.prima,
            disabled: false
          };
        } else {
          r.valorOption = {
            value: o.valorAsegurado.toString(),
            rawValue: o.valorAsegurado,
            disabled: !amparo.siNoPorcentajeBasico
          };

          r.porcentajeOption = {
            value: o.valorAsegurado.toString(),
            rawValue: o.valorAsegurado,
            disabled: !amparo.siNoPorcentajeBasico
          };
        }

        opciones.push(r);
      });

      const va: ValorAseguradoAmparo = {
        amparo,
        opciones,
        calculoBasePorValorAsegurado: true
      };

      result.push(va);
    });

    return result;
  }

  private fillAmparos() {
    this.grupoAsegurado.amparos.forEach(a => {
      this.amparosArray.push(new FormControl(a));
    });
  }

  private defineValorAseguradoOpcionesGroup(amparo: Amparo, opciones: OpcionValorAsegurado[]): FormArray {
    const result: FormArray = this.formBuilder.array([], { validators: [ValorAseguradoOpcionesValidator], updateOn: 'change' });
    opciones.forEach(opt => {
      const group = this.defineValorAseguradoOpcionGroup(amparo, opt);
      result.push(group);
    });

    return result;
  }

  private defineValorAseguradoOpcionGroup(amparo: Amparo, opcion: OpcionValorAsegurado): FormGroup {
    const option = this.formBuilder.group({
      porcentaje: [opcion.porcentajeOption.rawValue],
      valor: [opcion.valorOption.rawValue],
      prima: [opcion.primaOption.rawValue],
      numeroSalarios: [opcion.numeroSalariosOption.rawValue],
      dias: [opcion.numeroDiasOption.rawValue],
      valorDiario: [opcion.valorDiarioOption.rawValue]
    }, { validators: [ValorAseguradoOpcionValidator], updateOn: 'change' });

    return option;
  }

  private configurePorcentajeOption(amparo: any, opcionValores: OpcionValor) {
    const esAsistencia = amparo.codigoGrupoAmparo === 3;
    const esAmparoBasicoNoAdicional = amparo.siNoBasico && !amparo.siNoAdicional;

    const value = opcionValores ? opcionValores.porcentajeCobertura : '';
    const option: ValorOption = {
      // habilite opcion de porcentaje si el amparo captura porcentaje
      disabled: !amparo.siNoPorcentajeBasico,
      value: value.toString(),
      rawValue: Number(value.toString().replace(/\D+/g, ''))
    };

    // deshabilite el control si el grupo no exige amparo basico no adicional
    if (!this.basicoNoAdicionalRequired && esAmparoBasicoNoAdicional) {
      option.disabled = true;
    }

    // deshabilite el control si el amparo es del grupo 3 (Asistencias)
    if (esAsistencia) {
      option.disabled = true;
    }

    // deshabilite el control si la modalidad del amparo es 4
    if (amparo.modalidad.codigo === 4) {
      option.disabled = true;
      option.value = '';
    }

    // si el control esta deshabilitado y el valor es 0, no muestr ningun valor
    if (option.disabled && option.rawValue === 0) {
      option.value = '';
    }

    return option;
  }

  private configureValorOption(amparo: any, opcionValores: OpcionValor) {
    const esAsistencia = amparo.codigoGrupoAmparo === 3;
    const esAmparoBasicoNoAdicional = amparo.siNoBasico && !amparo.siNoAdicional;
    const esMultiploDeSueldos = this.tipoSumaAsegurada.codigoTipoSumaAsegurada === 2;

    const value = opcionValores ? opcionValores.valorAsegurado : '';
    const option: ValorOption = {
      // habilite opcion de porcentaje si el amparo captura porcentaje
      // habilite opcion de valor si el amparo captura valor
      disabled: amparo.siNoPorcentajeBasico,
      value: value.toString(),
      rawValue: Number(value.toString().replace(/\D+/g, ''))
    };

    if (esMultiploDeSueldos) {
      option.disabled = true;
    }

    // deshabilite el control si el grupo no exige amparo basico no adicional
    if (!this.basicoNoAdicionalRequired && esAmparoBasicoNoAdicional) {
      option.disabled = true;
    }

    // deshabilite el control si el amparo es del grupo 3 (Asistencias)
    if (esAsistencia) {
      option.disabled = true;
    }

    // deshabilite el control si la modalidad del amparo es 4
    if (amparo.modalidad.codigo === 4) {
      option.disabled = true;
      option.value = '';
    }

    // si el control esta deshabilitado y el valor es 0, no muestra ningun valor
    if (option.disabled && option.rawValue === 0) {
      option.value = '';
    }

    return option;
  }

  private configurePrimaOption(amparo: any, opcionValores: OpcionValor) {
    const value = opcionValores ? opcionValores.prima : '';
    const option: ValorOption = {
      // habilite opcion de porcentaje si el amparo captura porcentaje
      // hablite opcion de prima (Asistencias) si el grupo del amparo es 3
      disabled: amparo.codigoGrupoAmparo !== 3,
      value: value.toString(),
      rawValue: Number(value.toString().replace('$', ''))
    };

    // deshabilite el control si la modalidad del amparo es 4
    if (amparo.modalidad.codigo === 4) {
      option.disabled = true;
      option.value = '';
    }

    return option;
  }

  private configureNumeroSalariosOption(amparo: any, opcionValores: OpcionValor) {
    const esAsistencia = amparo.codigoGrupoAmparo === 3;
    const value = opcionValores ? opcionValores.numeroSalarios : '';
    const option: ValorOption = {
      // habilite opcion de porcentaje si el amparo captura porcentaje
      // habilite opcion de salarios si el tipo de suma asegurada no aplica
      // si tipo de suma aseguraeda es diferente a Multiplo de sueldos, Suma fija y Multiplo de Sueldos y SMMLV
      disabled: !this.capturaSalarios,
      value: value.toString(),
      rawValue: Number(value.toString().replace(/[^0-9.]/g, ''))
    };

    if (amparo.siNoPorcentajeBasico) {
      option.disabled = true;
    }

    // deshabilite el control si el amparo es del grupo 3 (Asistencias)
    if (esAsistencia) {
      option.disabled = true;
    }

    // deshabilite el control si la modalidad del amparo es 4
    if (amparo.modalidad.codigo === 4) {
      option.disabled = true;
      option.value = '';
    }

    // si el control esta deshabilitado y el valor es 0, no muestra ningun valor
    if (option.disabled && option.rawValue === 0) {
      option.value = '';
    }

    return option;
  }

  private configureDiasOption(amparo: any, opcionValores: OpcionValor) {
    const value = opcionValores ? opcionValores.numeroDias : '';
    const option: ValorOption = {
      disabled: false,
      value: value.toString(),
      rawValue: Number(value.toString().replace(/\D+/g, ''))
    };

    // deshabilite el control si la modalidad del amparo es diferente a 4
    if (amparo.modalidad.codigo !== 4) {
      option.disabled = true;
    }

    // si el control esta deshabilitado y el valor es 0, no muestra ningun valor
    if (option.disabled && option.rawValue === 0) {
      option.value = '';
    }

    return option;
  }

  private configureValorDiarioOption(amparo: any, opcionValores: OpcionValor) {
    const value = opcionValores ? opcionValores.valorDiario : '';
    const option: ValorOption = {
      disabled: false,
      value: value.toString(),
      rawValue: Number(value.toString().replace(/\D+/g, ''))
    };

    // deshabilite el control si la modalidad del amparo es diferente a 4
    if (amparo.modalidad.codigo !== 4) {
      option.disabled = true;
    }

    // si el control esta deshabilitado y el valor es 0, no muestra ningun valor
    if (option.disabled && option.rawValue === 0) {
      option.value = '';
    }

    return option;
  }

  private configureValorDiarioDiasOption(amparo: any, opcionValores: OpcionValor) {
    const value = opcionValores ? opcionValores.valorAseguradoDias : '';
    const option: ValorOption = {
      disabled: true,
      value: value.toString(),
      rawValue: Number(value.toString().replace(/\D+/g, ''))
    };

    // deshabilite el control si la modalidad del amparo es diferente a 4
    if (amparo.modalidad.codigo !== 4) {
      option.disabled = true;
    }

    // si el control esta deshabilitado y el valor es 0, no muestra ningun valor
    if (option.disabled && option.rawValue === 0) {
      option.value = '';
    }

    return option;
  }

  private defineBasicoNoAdicionalRequired() {
    switch (this.tipoSumaAsegurada.codigoTipoSumaAsegurada) {
      case 1:  // Suma Fija
      case 2:  // Multiplo de sueldos
      case 5:  // Suma fija y múltiplo de sueldos
      case 10: // SMMLV
        this.basicoNoAdicionalRequired = true;
        break;
      case 3: // Suma variable por asegurado
      case 6: // Saldo Deudores-Ahorros-Aportes
        this.basicoNoAdicionalRequired = false;
        break;
    }
  }

  private configure() {
    this.defineBasicoNoAdicionalRequired();
  }

  init(grupoAsegurado: GrupoAsegurado, grupoAseguradoInfo: any) {
    const formFactory = new GrupoAseguradosFormFactory(this.formBuilder);
    this.Form = formFactory.form;
    this.EdadesAmparos = this.Form.get('edadesAmparos') as FormArray;
    this.Amparos = this.Form.get('amparos') as FormArray;

    this.tipoSumaAsegurada = grupoAsegurado.tipoSumaAsegurada;
    this.GrupoAsegurado = this.mapperService.extendModel(grupoAsegurado, grupoAseguradoInfo);

    // update service variables
    this.esSumaFijaMultiploSueldos = this.tipoSumaAsegurada.codigoTipoSumaAsegurada === 5;
    this.esMultiploSueldos = this.tipoSumaAsegurada.codigoTipoSumaAsegurada === 2;
    this.esSMMLV = this.tipoSumaAsegurada.codigoTipoSumaAsegurada === 10;
    this.calculaValorAseguradoONumeroSalarios = this.esSumaFijaMultiploSueldos || this.esSMMLV;
    this.capturaSalarios = this.esMultiploSueldos || this.calculaValorAseguradoONumeroSalarios;

    this.configure();

    // TODO avoid timeout implementation
    setTimeout(() => {
      this.fillAmparos();
    }, 300);
  }

  pushAmparo(amparo: Amparo) {
    this.amparosArray.push(new FormControl(amparo));
  }

  removeAmparo(amparo: Amparo) {
    const amparoEncontrado = this.Amparos.value.find((a: Amparo) => a.codigoAmparo === amparo.codigoAmparo);
    const index = this.amparosArray.value.indexOf(amparoEncontrado);
    if (index >= 0) {
      this.amparosArray.removeAt(index);
    }
  }

  addValorAsegurado(valorAseguradoItem: ValorAseguradoAmparo) {
    const valorAseguradoOpcionesArray = this.defineValorAseguradoOpcionesGroup(valorAseguradoItem.amparo, valorAseguradoItem.opciones);
    const group = this.formBuilder.group({
      amparo: valorAseguradoItem.amparo,
      opciones: [valorAseguradoItem.opciones, [ValorAseguradoOpcionValidator]]
      // opciones: valorAseguradoOpcionesArray
    });

    this.valoresAseguradosArray.push(group);
  }

  removeValorAsegurado(amparo: Amparo, opciones: any[]) {
    const idx = this.valoresAseguradosArray.value.findIndex(x => x.amparo.codigoAmparo === amparo.codigoAmparo);
    // remove form formarray
    this.valoresAseguradosArray.removeAt(idx);
    // remove from ds model
    this.grupoAsegurado.amparos.splice(idx, 1);
  }

  clearValoresAsegurados() {
    while (this.valoresAseguradosArray.length) {
      this.valoresAseguradosArray.removeAt(0);
    }
  }

  addEdadesAmparo(edadesAmparoItem: EdadIngresoPermanenciaAmparo) {
    const group = this.formBuilder.group({
      amparo: this.formBuilder.control(edadesAmparoItem.amparo),
      edadMinimaIngreso: this.formBuilder.control(edadesAmparoItem.edadMinimaIngreso),
      edadMaximaIngreso: this.formBuilder.control(edadesAmparoItem.edadMaximaIngreso),
      edadMaximaPermanencia: this.formBuilder.control(edadesAmparoItem.edadMaximaPermanencia),
      numeroDiasCarencia: this.formBuilder.control(edadesAmparoItem.numeroDiasCarencia)
    }, EdadesAmparoValidator);

    this.EdadesAmparos.push(group);
  }

  removeEdadesAmparo(amparo: Amparo) {
    const idx = this.EdadesAmparos.value.findIndex(x => x.amparo.codigoAmparo === amparo.codigoAmparo);
    this.EdadesAmparos.removeAt(idx);
  }

  createEdadIngresoPermanenciaField(amparo: Amparo, propertyName: string): EdadIngresoPermanenciaAmparoField {
    let edadesGrupo;
    const amparoInfo = this.grupoAsegurado.amparos.find(x => x.codigoAmparo === amparo.codigoAmparo);
    if (amparoInfo) {
      edadesGrupo = amparoInfo.edadesGrupo;
    }

    const edadesGrupoPropertyName = this.getEdadesGrupoPropertyName(propertyName);
    const value = edadesGrupo ? edadesGrupo[edadesGrupoPropertyName] : amparo[propertyName];

    const r: EdadIngresoPermanenciaAmparoField = {
      valor: amparo.siNoRequiereEdades ? value : amparo.codigoGrupoAmparo === 3 ? '' : 0,
      disabled: !amparo.siNoRequiereEdades
    };

    return r;
  }

  patchFormValue(model: any) {
    const amparos = model.amparosGrupo.map(x => {
      return this.ListAmparos.find(a => a.codigoAmparo === x.codigoAmparo);
    });

    const selectedAmparos = {
      amparos
    };
    const valoresAsegurados = this.mapValoresAsegurados(model.amparosGrupo);
    const edadesAmparos = this.mapEdadesAmparos(model.amparosGrupo);

    this.Form.reset({
      selectedAmparos,
      valoresAsegurados,
      edadesAmparos
    });
  }

  patchSelectedAmparos() {
    this.GrupoAsegurado.amparos.map(x => {
      const amparo = this.ListAmparos.find(a => a.codigoAmparo === x.codigoAmparo);
      (this.amparosArray as FormArray).push(new FormControl(amparo.codigoAmparo));
    });
  }

  generateValorAseguradoOpciones(amparo: Amparo): OpcionValorAsegurado[] {
    const result: OpcionValorAsegurado[] = [];
    let opcionesValores: any[] | OpcionValor[];

    const grupo = this.grupoAsegurado;
    const amparosGrupo = grupo.amparos.find(x => x.codigoAmparo === amparo.codigoAmparo);
    if (amparosGrupo) {
      opcionesValores = amparosGrupo.opcionesValores;
    }

    // Push options
    const optionsCount = this.tipoSumaAsegurada.codigoTipoSumaAsegurada === 1 ? 3 : 1;
    for (let i = 0; i <= optionsCount - 1; i++) {

      let opcionValores: any;
      if (amparosGrupo) {
        opcionValores = opcionesValores[i];
      }

      // define options
      const porcentajeOption = this.configurePorcentajeOption(amparo, opcionValores);
      const valorOption = this.configureValorOption(amparo, opcionValores);
      const primaOption = this.configurePrimaOption(amparo, opcionValores);
      const numeroSalariosOption = this.configureNumeroSalariosOption(amparo, opcionValores);
      const diasOption = this.configureDiasOption(amparo, opcionValores);
      const valorDiarioOption = this.configureValorDiarioOption(amparo, opcionValores);
      const valorDiarioDiasOption = this.configureValorDiarioDiasOption(amparo, opcionValores);

      const opt: OpcionValorAsegurado = {
        porcentajeOption,
        valorOption,
        primaOption,
        numeroSalariosOption,
        numeroDiasOption: diasOption,
        valorDiarioOption,
        valorDiarioDiasOption
      };

      result.push(opt);
    }

    return result;
  }

  deleteAseguradosAsync(codigoGrupoAsegurado: number) {
    return this.gruposAseguradosWriterService.deleteAseguradosAsync(codigoGrupoAsegurado);
  }
}
