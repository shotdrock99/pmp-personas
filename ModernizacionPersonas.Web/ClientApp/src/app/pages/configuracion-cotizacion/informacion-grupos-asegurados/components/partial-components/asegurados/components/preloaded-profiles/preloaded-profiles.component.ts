
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray } from '@angular/forms';
import { RangoPerfilEdad, IRango } from 'src/app/models';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { RangosErrors } from '../profiles/profiles.component';

@Component({
  selector: 'preloaded-profiles',
  templateUrl: './preloaded-profiles.component.html',
  styleUrls: ['./preloaded-profiles.component.scss']
})
export class PreloadedProfilesComponent implements OnInit {
  currentRango: IRango;
  originalDataRango: IRango;

  constructor(private cotizacionPersistenceService: CotizacionPersistenceService) { }

  cotizacion = this.cotizacionPersistenceService.cotizacion;

  rangos: IRango[] = [];
  allowConfigurarRango: boolean = true;
  errors: RangosErrors = new RangosErrors;

  @Output()
  public onRangosChange = new EventEmitter();

  @Input()
  submitted: boolean;

  @Input()
  model: RangoPerfilEdad[];

  @Input()
  data: FormArray;

  ngOnInit() {
    if (this.data.length > 0) {
      this.rangos = this.data.getRawValue();
    }
    else {
      let idx = this.rangos.length + 1;
      this.model.forEach(x => {
        this.rangos.push({
          codigoRangoGrupoAsegurado: idx,
          edadMinAsegurado: x.edadDesde,
          edadMaxAsegurado: x.edadHasta,
          numeroAsegurados: 0,
          valorAsegurado: 0
        });
      });
    }
  }

  private validateRango(range: IRango) {
    // validar que la cantidad de asegurados sea mayor a cero
    this.errors.aseguradosZero = range.numeroAsegurados < 1;
    // validar que el valor asegurado sea mayor a cero
    this.errors.valorAseguradoZero = range.valorAsegurado < 1;
    return Object.values(this.errors).every(x => !x);
  }

  editRango(index, range: IRango) {
    if (this.allowConfigurarRango) {
      this.originalDataRango = Object.assign({}, range);
      this.currentRango = range;
      range.isEdit = true;
      this.allowConfigurarRango = false;
    }
    else {
      //this.cancelEditRango(null, this.currentRango);
    }
  }

  cancelEditRango(index, range: IRango) {
    range = this.originalDataRango;
    range.isEdit = false;
    this.allowConfigurarRango = true;
  }

  saveRango(index: number, range: IRango) {
    let isValid = this.validateRango(range);
    if (isValid) {
      this.allowConfigurarRango = true;
      range.isEdit = false;
    }

    this.onRangosChange.emit(this.rangos);
  }
}
