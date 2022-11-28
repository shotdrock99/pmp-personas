import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { Component, OnInit, EventEmitter, Output, Input } from '@angular/core';
import { FormGroup, FormArray } from '@angular/forms';
import { IRango } from 'src/app/models';
import { GrupoAseguradoWizardService } from 'src/app/services/grupo-asegurado-wizard.service';

export class RangosErrors {
  edadMinGtMax: boolean;
  edadesGtMaxPermitido: boolean;
  aseguradosZero: boolean;
  valorAseguradoZero: boolean;
  invalid: boolean;
  edadMinLtLastMax: boolean;
  edadMinFirstRg: boolean;

  constructor() {
    this.edadMinGtMax = false;
    this.edadesGtMaxPermitido = false;
    this.aseguradosZero = false;
    this.valorAseguradoZero = false;
    this.edadMinFirstRg = false;
  }

  reset() {
    this.edadMinGtMax = false;
    this.edadesGtMaxPermitido = false;
    this.aseguradosZero = false;
    this.valorAseguradoZero = false;
    this.edadMinFirstRg = false;
  }
}

@Component({
  selector: 'app-range-profiles',
  templateUrl: './profiles.component.html',
  styleUrls: ['./profiles.component.scss']
})
export class ProfilesComponent implements OnInit {

  constructor(
    private cotizacionPersistenceService: CotizacionPersistenceService,
    private gruposAseguradosWizarService: GrupoAseguradoWizardService) { }

  cotizacion = this.cotizacionPersistenceService.cotizacion;
  amparos = this.gruposAseguradosWizarService.builtAmparos;

  rangos: IRango[] = [];
  allowCreateRango = true;
  errors: RangosErrors = new RangosErrors();

  @Input() submitted: boolean;
  @Input() data: FormArray;

  get form() {
    return this.parentForm;
  }

  @Output() rangosChange = new EventEmitter();
  @Input() parentForm: FormGroup;

  ngOnInit() {
    this.rangos = this.data.getRawValue();
  }

  private validateRango(index : number, range: IRango) {
    const amparoBasico = this.amparos.value.filter(x => !x.siNoAdicional && x.siNoBasico);
    if (this.rangos.length > 1 && range.codigoRangoGrupoAsegurado > 0) {
      
      if(index != 0){
        const last = this.rangos[index - 1];
        // validar si la edad minima del regsitro actual es menor a la edad maxima del registro anterior
        this.errors.edadMinLtLastMax = (range.edadMinAsegurado < last.edadMaxAsegurado) || (range.edadMinAsegurado == last.edadMaxAsegurado);
        
      }
      
    }
    // validar que la edad minima no sea mayor a la maxima
    this.errors.edadMinGtMax = (range.edadMinAsegurado > range.edadMaxAsegurado);
    // validar que las edades no sean mayor a 111
    this.errors.edadesGtMaxPermitido = (range.edadMaxAsegurado > this.cotizacion.parametrizacion.edadMaximaPermitida
      || range.edadMinAsegurado > this.cotizacion.parametrizacion.edadMaximaPermitida);
    // validar que la cantidad de asegurados sea mayor a cero
    this.errors.aseguradosZero = range.numeroAsegurados < 1;
    // validar que el valor asegurado sea mayor a cero
    this.errors.valorAseguradoZero = range.valorAsegurado < 1;
    // Validar que el valor edad mínima del primer rango sea mayor o igual que la edad mínima del amparo básico
    if (range.codigoRangoGrupoAsegurado === 1) {
      this.errors.edadMinFirstRg = range.edadMinAsegurado < amparoBasico[0].edadMinimaIngreso;
    }

    return Object.values(this.errors).every(x => !x);
  }

  addRango() {
    const amparoBasico = this.amparos.value.filter(x => !x.siNoAdicional && x.siNoBasico);
    const lastIndex = this.rangos.length - 1;
    let edadMinAsegurado = 0;
    if (lastIndex >= 0) {
      edadMinAsegurado = Number(this.rangos[lastIndex].edadMaxAsegurado);
    }

    const idx = this.rangos.length + 1;
    if (idx === 1) {
      this.rangos.push({
        codigoRangoGrupoAsegurado: idx,
        edadMinAsegurado: amparoBasico[0].edadMinimaIngreso,
        edadMaxAsegurado: 0,
        numeroAsegurados: 0,
        valorAsegurado: 0,
        isEdit: true
      });
    } else {
      this.rangos.push({
        codigoRangoGrupoAsegurado: idx,
        edadMinAsegurado: edadMinAsegurado + 1,
        edadMaxAsegurado: 0,
        numeroAsegurados: 0,
        valorAsegurado: 0,
        isEdit: true
      });
    }
    this.submitted = true;
    this.allowCreateRango = false;
  }
  editRango(index, range: IRango) {
    
    /*this.rangos.forEach(element => {
      element.isEdit = false;
    });*/
    range.isEdit = true;
    this.allowCreateRango = false;

  }

  cancelEditRango(index, range: IRango) {
    const isValid = this.validateRango(index, range);
    if (isValid && range.isEdit) {
      range.isEdit = false;
    } else {
      this.removeRango(index, range);
    }
    this.allowCreateRango = true;
  }

  saveRango(index: number, range: IRango) {
    const isValid = this.validateRango(index, range);
    if (isValid) {
      this.allowCreateRango = true;
      range.isEdit = false;
    }

    this.rangosChange.emit(this.rangos);
  }

  removeRango(index, rango: IRango) {
    const idx = this.rangos.findIndex(x => x.codigoRangoGrupoAsegurado === rango.codigoRangoGrupoAsegurado);
    if (idx >= 0) {
      this.rangos.splice(idx, 1);
    }
    // remove form formArray
    this.data.removeAt(idx);
    this.errors.reset();
    this.rangosChange.emit(this.rangos);
  }
}
