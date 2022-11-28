import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { CotizacionState } from '../models';
import { CotizacionPersistenceService } from './cotizacion-persistence.service';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NavegacionService {

  constructor(private cotizacionDataService: CotizacionPersistenceService,
    private router: Router,
    public dialog: MatDialog) { }

  private sectionsCount = 6;
  private _progress = 0;

  activeSectionIndex: number = 0;
  sections = {
    s0: {
      index: 0,
      labelIndex: 1,
      stateId: 1101,
      title: 'Información General',
      initialized: true,
      completed: false
    },
    s1: {
      index: 1,
      labelIndex: 2,
      stateId: 1102,
      title: 'Datos básicos Tomador',
      initialized: false,
      completed: false
    },
    s2: {
      index: 2,
      labelIndex: 3,
      stateId: 1103,
      title: 'Información de Negocio',
      initialized: false,
      completed: false
    },
    s3: {
      index: 3,
      labelIndex: 4,
      stateId: 1104,
      title: 'Información de Intermediarios',
      initialized: false,
      completed: false
    },
    s4: {
      index: 4,
      labelIndex: 5,
      stateId: 1105,
      title: 'Grupos de asegurados',
      initialized: false,
      completed: false
    },
    s5: {
      index: 5,
      labelIndex: 6,
      stateId: 1106,
      title: 'Siniestralidad',
      initialized: false,
      completed: false
    },
    activate(index) {
      let m = this;
      for (let prop in m) {
        if (m[prop]["index"] === index) {
          m[prop].initialized = true;
        }
      }
    },
    deactivate(index) {
      let m = this;
      for (let prop in m) {
        if (m[prop]["index"] === index) {
          m[prop].initialized = false;
        }
      }
    },
    getItemByIndex(index) {
      let m = this;
      for (let prop in m) {
        if (m[prop]["labelIndex"] === index + 1) {
          return m[prop];
        }
      }
    },
    getItemByStateId(stateId) {
      let m = this;
      for (let prop in m) {
        if (m[prop]["stateId"] === stateId) {
          return m[prop];
        }
      }
    },
    reset() {
      let m = this;
      for (let prop in m) {
        if (typeof m[prop] === 'object') {
          let isFirst = m[prop].index = 0;
          m[prop].initialized = isFirst;
          m[prop].completed = false;
        }
      }
    }
  }

  get progress() { return this._progress; }

  public reset() {
    this.sections.reset()
  }

  public subscribe(view: any): void {
    this.activeSectionIndex = 0;
    let cotizacion = this.cotizacionDataService.cotizacion;
    const section = this.sections.getItemByIndex(view.indexView);
    if (section) {
      // merge section events
      Object.assign(section, {
        index: view.indexView,
        continue: view.continuePromise,
        initialize: view.initializePromise
      });

      if (this.cotizacionDataService.isEdit) {
        section.completed = cotizacion.estado > section.stateId;
        section.initialized = cotizacion.estado >= section.stateId;
        // focus the current section
        const item = this.sections.getItemByStateId(cotizacion.estado);
        this.activeSectionIndex = item ? item.index : 4;
      }
    }
  }

  public completeCotizacion() {
    const cotizacion = this.cotizacionDataService.cotizacion;
    this.router.navigate(['/cotizaciones', cotizacion.codigoCotizacion, 'resumen']);
  }

  public async validateNavigate(direction: string, indexView: number) {
    let promise = new Promise(async (resolve, reject) => {
      const cotizacion = this.cotizacionDataService.cotizacion;
      const esNegocioDirecto = cotizacion.informacionNegocio.esNegocioDirecto;

      let canContinue = false;
      let currentStep = this.sections.getItemByIndex(indexView);
      let nextStep = this.sections.getItemByIndex(indexView + 1);
      if (direction === 'next') {
        if (currentStep.continue) {
          canContinue = await currentStep.continue();
          // update completed section state
          currentStep.completed = canContinue;
          if (canContinue && nextStep) {
            // si es negocio directo y la seccion actual es Informacion de Negocio
            // adelante a la siguiente seccion
            if (esNegocioDirecto && currentStep.index === 2) {
              this.sections.deactivate(nextStep.index);
              nextStep = this.sections.getItemByIndex(nextStep.index + 1);
            }

            if (!nextStep.initialized) {
              this.sections.activate(nextStep.index);
              if (nextStep.initialize) {
                nextStep.initialize();
              }

              if (!currentStep.completed) {
                currentStep.completed = true;
                this._progress += Math.round(100 / this.sectionsCount);
              }
            }

            if (!esNegocioDirecto) {
              let informacionNegocioStep = this.sections.getItemByIndex(2);
              if (informacionNegocioStep.completed && !nextStep.completed && currentStep.labelIndex !== 4) {
                this._progress -= Math.round(100 / this.sectionsCount);
              }
            }
          }

          resolve(canContinue);
        }
      }

    });

    let result = await promise;
    return result;
  }
}
