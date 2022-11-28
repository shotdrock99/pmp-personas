import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MAT_MOMENT_DATE_FORMATS, MomentDateAdapter } from '@angular/material-moment-adapter';
import * as moment from 'moment';
import { RegistroSiniestralidad } from '../../informacion-siniestralidad.component';
import { Siniestralidad } from 'src/app/models';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-registro-siniestralidad',
  templateUrl: './registro-siniestralidad.component.html',
  styleUrls: ['./registro-siniestralidad.component.scss'],
  providers: [{provide: MAT_DATE_LOCALE, useValue: 'es-CO'},
              {provide: DateAdapter,
              useClass: MomentDateAdapter,
              deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS]},
              {provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS}]
})
export class RegistroSiniestralidadComponent implements OnInit {

  minDate: Date;
  maxDate: Date;

  @Input() model: RegistroSiniestralidad;
  @Output() OnValidate = new EventEmitter();
  @Input() readonly: boolean;
  @Input() anyosSiniestralidad$: Observable<number>;
  anyosSiniestralidad: number;

  ngOnInit() {
    // Set the minimum to January 1st 3 years in the past.
    this.anyosSiniestralidad$.subscribe(anyos => {
      this.anyosSiniestralidad = anyos;
      const currentYear = new Date().getFullYear();
      this.minDate = new Date(currentYear - anyos, 0, 1);
      this.maxDate = new Date(currentYear - anyos, 11, 31);
    });
  }

  validate(e) {
    this.clearErrors();
    const fechaFinal = moment(this.model.fechaInicial).clone().add(1, 'year').subtract(1, 'day');
    const fIY = moment(this.model.fechaInicial).year();
    const fIM = moment(this.model.fechaInicial).month();
    const fID = moment(this.model.fechaInicial).date();
    const fechaFinalMinimunRange = new Date(fIY, fIM + 10, fID);
    const fechaFinalMaxRange = new Date(fIY, fIM + 12, fID);
    const fechaFinalHoy = new Date();
    if (this.model.index < (this.anyosSiniestralidad - 1)) {
      this.model.fechaFinal = fechaFinal.toDate();
      if (this.model.fechaFinal < fechaFinal.toDate()) {
        this.model.setError({ finalLtYear: true });
      }
    }

    if (this.anyosSiniestralidad !== 1 && (this.model.fechaInicial > this.model.fechaFinal)) {
      this.model.setError({ inicialGtFinal: true });
    }

    if (!this.model.fechaInicial) {
      this.model.setError({ inicialRequired: true });
    }

    if (this.model.fechaInicial && !this.model.fechaFinal) {
      this.model.setError({ finalRequired: true });
    }

    if (this.model.fechaFinal < fechaFinalMinimunRange || this.model.fechaFinal > fechaFinalMaxRange) {
      this.model.setError({ finalRange: true });
    }

    if (this.model.fechaFinal > fechaFinalHoy) {
      this.model.setError({ finalToday: true });
    }

    const args: any = { opcion: this.model };
    this.OnValidate.emit(args);
  }

  private clearErrors() {
    this.model.errors = {};
  }
}
