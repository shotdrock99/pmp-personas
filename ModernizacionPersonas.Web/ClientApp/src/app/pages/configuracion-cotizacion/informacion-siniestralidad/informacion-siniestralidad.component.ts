import { NotificationService } from 'src/app/shared/services/notification.service';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import * as moment from 'moment';
import {
  RegistroSiniestralidadComponent,
} from '@pages/configuracion-cotizacion/informacion-siniestralidad/components/registro-siniestralidad/registro-siniestralidad.component';
import { NavegacionService } from 'src/app/services/navegacion.service';
import { SiniestralidadWriterService } from 'src/app/services/siniestralidad-writer.service';

import { InformacionSiniestralidadViewData } from './../../../models/cotizacion';
import { Observable } from 'rxjs';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';

@Component({
  selector: 'app-informacion-siniestralidad',
  templateUrl: './informacion-siniestralidad.component.html',
  styleUrls: ['./informacion-siniestralidad.component.scss']
})
export class InformacionSiniestralidadComponent implements OnInit {
  indexView = 5;
  submitted: boolean;
  isValidForm: boolean;
  formErrors: any = {};
  keys: string[];
  fechaMaxima: Date;
  fechaInicialActual: Date = null;

  constructor(
    private navigationService: NavegacionService,
    private cotizacionDataService: CotizacionPersistenceService,
    private notificationService: NotificationService,
    private siniestralidadWriter: SiniestralidadWriterService) { }

  @Input() data: InformacionSiniestralidadViewData;
  @Input() readonly: boolean;
  @Input() anyosSiniestralidad$: Observable<number>;
  anyosSiniestralidad: number;

  model: any = {};

  ngOnInit() {
    this.navigationService.subscribe({
      indexView: this.indexView,
      continuePromise: () => {
        return this.continue();
      },
      initializePromise: () => {
        this.initializeSection();
      }
    });
    if (this.cotizacionDataService.isEdit) {
      this.initializeSection();
    }
  }

  private generarRegistroSiniestralidad(indexOrigen: number): RegistroSiniestralidad {
    let fechaInicialReadonly: boolean;
    let fechaFinalReadonly: boolean;
    if (this.anyosSiniestralidad === 1) {
      fechaInicialReadonly = false;
      fechaFinalReadonly = false;
    } else if ((indexOrigen + 1) === 1) {
      fechaInicialReadonly = false;
      fechaFinalReadonly = true;
    }  else if ((indexOrigen + 1) === this.anyosSiniestralidad) {
      fechaInicialReadonly = true;
      fechaFinalReadonly = false;
    } else {
      fechaInicialReadonly = true;
      fechaFinalReadonly = true;
    }
    return {
      index: indexOrigen,
      anno: indexOrigen + 1,
      fechaInicial: null,
      fechaInicialReadonly,
      fechaFinal: null,
      fechaFinalReadonly,
      numeroCasos: 0,
      valorIncurrido: 0,
      errors: {},
      setError(error) {
        const key = Object.keys(error)[0];
        const value = Object.values(error)[0];
        this.errors[key] = value;
      }
    };
  }

  private updateForm() {
    if (this.data.informacionSiniestralidad.length === this.anyosSiniestralidad) {
      for (const informacionSiniestralidad of this.data.informacionSiniestralidad) {
        this.model['anno' + informacionSiniestralidad.anno].fechaInicial = informacionSiniestralidad.fechaInicial;
        this.model['anno' + informacionSiniestralidad.anno].fechaFinal = informacionSiniestralidad.fechaFinal;
        this.model['anno' + informacionSiniestralidad.anno].numeroCasos = informacionSiniestralidad.numeroCasos;
        this.model['anno' + informacionSiniestralidad.anno].valorIncurrido = informacionSiniestralidad.valorIncurrido;
      }
    } else {
      this.setFormError({ empty: true });
    }
    this.validateForm();
  }

  private setFormError(error: any) {
    const key = Object.keys(error)[0];
    const value = Object.values(error)[0];
    this.formErrors[key] = value;
  }

  private clearFormErrors() {
    this.formErrors = [];
  }

  private initializeSection() {
    this.registerAnyosSiniestralidad();
  }

  private registerAnyosSiniestralidad(): void {
    this.anyosSiniestralidad$.subscribe(anyos => {
        this.submitted = false;
        this.clearFormErrors();
        this.anyosSiniestralidad = anyos;
        this.model = {};
        for (let index = 0; index < this.anyosSiniestralidad; index++) {
          this.model['anno' + (index + 1)] = this.generarRegistroSiniestralidad(index);
        }
        this.keys = Object.keys(this.model);
        if (this.data) {
          this.updateForm();
        } else {
          this.setFormError({ empty: true });
        }
      });
  }

  private updateOptionValues(option: RegistroSiniestralidad) {
    if (option.index === 0) {
      for (let index = 0; index < this.anyosSiniestralidad; index++) {
        if (this.anyosSiniestralidad === 1) {
          if (!moment(this.fechaInicialActual).isSame(moment(option.fechaInicial))) {
            this.fechaInicialActual = moment(option.fechaInicial).clone().toDate();
            this.model['anno' + (index + 1)].fechaInicial = moment(option.fechaInicial).clone().toDate();
            this.model['anno' + (index + 1)].fechaFinal = moment(
              this.model['anno' + (index + 1)].fechaInicial).clone().add(1, 'year').subtract(1, 'day').toDate();
          }
        } else if ((index + 1) === 1) {
          this.model['anno' + (index + 1)].fechaInicial = moment(option.fechaInicial).clone().toDate();
          this.model['anno' + (index + 1)].fechaFinal = moment(
            this.model['anno' + (index + 1)].fechaInicial).clone().add(1, 'year').subtract(1, 'day').toDate();
        } else if ((index + 1) === this.anyosSiniestralidad) {
          this.model['anno' + (index + 1)].fechaInicial = moment(this.model['anno' + ((index + 1) - 1)].fechaFinal).add(1, 'day').toDate();
          const fechaFinal = moment(this.model['anno' + (index + 1)].fechaInicial).clone().add(1, 'year').toDate();
          const fechaFinalHoy = new Date();
          this.model['anno' + (index + 1)].fechaFinal = fechaFinal > fechaFinalHoy ? fechaFinalHoy : fechaFinal;
          this.validarUltimaVigenciaFechaFinal((index + 1));
        } else {
          this.model['anno' + (index + 1)].fechaInicial = moment(
            this.model['anno' + ((index + 1) - 1)].fechaFinal).add(1, 'day').toDate();
          this.model['anno' + (index + 1)].fechaFinal =  moment(
            this.model['anno' + (index + 1)].fechaInicial).clone().add(1, 'year').subtract(1, 'day').toDate();
        }
      }
    }
  }

  private validarUltimaVigenciaFechaFinal(numeroOpcion: number): void {
    this.model['anno' + numeroOpcion].errors = {};
    const fechaFinal = moment(this.model['anno' + numeroOpcion].fechaInicial).clone().add(1, 'year').subtract(1, 'day');
    const fIY = moment(this.model['anno' + numeroOpcion].fechaInicial).year();
    const fIM = moment(this.model['anno' + numeroOpcion].fechaInicial).month();
    const fID = moment(this.model['anno' + numeroOpcion].fechaInicial).date();
    const fechaFinalMinimunRange = new Date(fIY, fIM + 10, fID);
    const fechaFinalMaxRange = new Date(fIY, fIM + 12, fID);
    const fechaFinalHoy = new Date();
    if (this.model['anno' + numeroOpcion].fechaInicial > this.model['anno' + numeroOpcion].fechaFinal) {
      this.model['anno' + numeroOpcion].setError({ inicialGtFinal: true });
    }
    if (this.model['anno' + numeroOpcion].fechaInicial && !this.model['anno' + numeroOpcion].fechaFinal) {
      this.model['anno' + numeroOpcion].setError({ finalRequired: true });
    }

    if (this.model['anno' + numeroOpcion].fechaFinal < fechaFinalMinimunRange ||
    this.model['anno' + numeroOpcion].fechaFinal > fechaFinalMaxRange) {
      this.model['anno' + numeroOpcion].setError({ finalRange: true });
    }

    if (this.model['anno' + numeroOpcion].fechaFinal > fechaFinalHoy) {
      this.model['anno' + numeroOpcion].setError({ finalToday: true });
    }
  }

  private validate(option: RegistroSiniestralidad) {
    this.clearFormErrors();
    const isValidDate = moment(option.fechaInicial).isValid();
    if (!isValidDate) {
      this.setFormError({ invalidDate: true });
    }
    this.fechaMaxima = moment().subtract(1, 'day').toDate();
    if (option.index === 0 && option.fechaInicial > this.fechaMaxima) {
      this.setFormError({ initialGtToday: true });
    }
    const fechaMinima = moment('01/01/1900').toDate();
    if (option.index === 0 && option.fechaInicial < fechaMinima) {
      option.setError({ invalidDate: true });
    }
    // if (option.index == 2 && option.fechaFinal > this.fechaMaxima) {
    //   option.setError({ finalGtToday: true });
    // }
    this.updateOptionValues(option);
    this.validateForm();
  }

  private validateForm() {
    const result: boolean[] = [];
    this.keys.forEach(key => {
      const opt: RegistroSiniestralidad = this.model[key];
      if (typeof opt === 'object') {
        const isValid = Object.keys(opt.errors).length === 0 && Object.keys(this.formErrors).length === 0;
        result.push(isValid);
      }
    });
    this.isValidForm = result.every(x => x);
  }

  validateOpcion(args) {
    this.validate(args.opcion);
  }

  async continue() {
    const promise = new Promise((resolve, reject) => {
      this.submitted = true;
      if (this.isValidForm) {
        const toast = this.notificationService.showToast('Guardando Siniestralidad', 0);
        this.siniestralidadWriter.Write(this.model)
          .subscribe(res => {
            toast.dismiss();
            resolve(true);
          });
      }
    });
    const result = await promise;
    return result;
  }
}

export interface RegistroSiniestralidad {
  index: number;
  anno: number;
  fechaInicial: Date;
  fechaInicialReadonly: boolean;
  fechaFinal: Date;
  fechaFinalReadonly: boolean;
  numeroCasos: number;
  valorIncurrido: number;
  errors: {
    invalidDate?: boolean;
    inicialRequired?: boolean;
    inicialGtbsiblingfinal?: boolean;
    inicialGtFinal?: boolean;
    finalRequired?: boolean;
    finalLtYear?: boolean;
    finalGtToday?: boolean;
    finalRange?: boolean;
    finalToday?: boolean;
  };
  setError(error: any): void;
}

export interface SiniestralidadModel {
  anno1: RegistroSiniestralidad;
  anno2: RegistroSiniestralidad;
  anno3: RegistroSiniestralidad;
  getOptionByIndex(index: number): any;
}
