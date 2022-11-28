import { NotificationService } from './../../../../shared/services/notification.service';
import { CotizacionState } from './../../../../models/cotizacion';
import { FormGroup, FormBuilder } from '@angular/forms';
import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { variable } from '@angular/compiler/src/output/output_ast';
import { ErrorCollection } from 'src/app/models/error-collection';
import { SlipClausula, SlipClausulaVariable } from '../../../../models/slip-clausula';
import { CotizacionFilter } from 'src/app/models/cotizacion-filter';

@Component({
  selector: 'app-configuracion-slip-clausulas',
  templateUrl: './configuracion-slip-clausulas.component.html',
  styleUrls: ['./configuracion-slip-clausulas.component.scss']
})
export class ConfiguracionSlipClausulasComponent implements OnInit {

  claususlasForm: FormGroup;
  hasChanges = false;
  isValid = true;
  allComplete = false;

  constructor(
    private formBuilder: FormBuilder,
    private notificationService: NotificationService) { }

  @Input() data: SlipClausula[];
  @Input() cotizacionState: CotizacionState;
  @Input() readonly: boolean;
  @Output() valuesChange: EventEmitter<any> = new EventEmitter();

  @Output()
  refresh: EventEmitter<any> = new EventEmitter();

  ngOnInit() {
    this.validateForm();
  }

  get valid() {
    this.validateForm();
    return this.isValid;
  }

  executeRefresh() {
    this.refresh.emit();
  }

  getMask(value: any) {
    let result = '99999999999999999';
    if (value.tipoDato === 'MO') {
      result = 'separator.2';
    }

    return result;
  }

  onChangeAsegurabilidad(args: any): void {
    if (args.dirty) {
      this.valuesChange.emit({ dirty: true });
    }
  }

  private validateForm() {
    this.isValid = true;
    this.data.forEach(clausula => {
      if (clausula.activo) {
        this.validateClausula(clausula);
      }
    });
  }

  private validateClausula(clausula: any) {
    const variables = clausula.variables;
    variables.forEach((v: any) => {
      this.onVariableChange(v);
    });
  }

  onClausulaSelect(clausula: any) {
    if (clausula.activo) {
      this.validateClausula(clausula);
    }
    this.valuesChange.emit({ dirty: true });
  }

  onVariableChange(clausulaVariable: SlipClausulaVariable) {
    this.valuesChange.emit({ dirty: true });
    this.isValid = false;
    const value = Number(clausulaVariable.valor);
    if (!clausulaVariable.errors) {
      clausulaVariable.errors = new ErrorCollection();
    }

    if (value === 0 || !value) {
      clausulaVariable.errors.add({ required: true });
    }
    // else if (value > variable.valorMaximo) {
    //   variable.errors.add({ 'max': true });
    // }
    // tslint:disable-next-line: one-line
    else {
      // si no tiene errores
      clausulaVariable.errors = null;
      this.isValid = true;

      // if (this.cotizacionState === CotizacionState.ApprovedAuthorization) {
      // this.notificationService.showToast('No es psible realizar cambios, la cotización esta en estado Aceptada.');
      this.hasChanges = true;
      this.valuesChange.emit({ hasChanges: true });
      // }
    }
  }

  someComplete(): boolean {
    if (this.data == null) {
      return false;
    }
    return this.data.filter(t => t.activo).length > 0 && !this.allComplete;
  }

  setAll(activo: boolean) {
    this.allComplete = activo;
    if (this.data == null) {
      return;
    }
    this.data.forEach(t => t.activo = activo);
    this.valuesChange.emit({ hasChanges: true});
  }

  updateAllComplete() {
    this.allComplete = this.data != null && this.data.every(t => t.activo);
  }

}

