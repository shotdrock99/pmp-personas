import { SlipAmparoVariable } from '../../../../models/slip-amparo';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { SlipAmparo } from '../../../../models/slip-amparo';
import { ErrorCollection } from 'src/app/models/error-collection';
import { CotizacionState } from 'src/app/models';

@Component({
  selector: 'app-configuracion-slip-amparos',
  templateUrl: './configuracion-slip-amparos.component.html',
  styleUrls: ['./configuracion-slip-amparos.component.scss']
})
export class ConfiguracionSlipAmparosComponent implements OnInit {

  constructor() { }

  hasChanges = false;
  @Input() data: SlipAmparo[] = [];
  @Input() cotizacionState: CotizacionState;
  @Input() readonly: boolean;
  @Output() valuesChange: EventEmitter<any> = new EventEmitter();

  ngOnInit() {
  }

  getMask(variable: any) {
    let result = '99999999999999999';
    if (variable.tipoDato === 'MO') {
      result = 'separator.2';
    }

    return result;
  }

  onVariableChange(variable: SlipAmparoVariable) {
    // if (this.cotizacionState === CotizacionState.ApprovedAuthorization) {
    // this.notificationService.showToast('No es psible realizar cambios, la cotización esta en estado Aceptada.');
    this.hasChanges = true;
    this.valuesChange.emit({ hasChanges: true, dirty: true });
    // }
    if (!variable.errors) {
      variable.errors = new ErrorCollection();
    }
    if (variable.valor === 0 || !variable.valor) {
      variable.errors.add({ required: true });
    } else if (variable.valor > variable.valorMaximo) {
      variable.errors.add({ max: true });
    } else {
      // si no tiene errores
      variable.errors = null;
    }
  }

}
