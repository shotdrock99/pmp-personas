import { CotizacionTasa } from '../../../../models/cotizacion-authorization';
import { Component, Inject, ViewChild } from '@angular/core';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-tasa-selector',
  templateUrl: './tasa-selector.component.html',
  styleUrls: ['./tasa-selector.component.scss']
})
export class TasaSelectorComponent {
  tasas: any[];
  constructor(public dialogRef: MatDialogRef<TasaSelectorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TasaSelectorModel) {
    this.tasas = data.tasas;
  }

  @ViewChild('group', { static: true })
  group: MatButtonToggleModule;

  selectedTasa = this.data.tasas[0].codigoTasa;

  onConfirm(): void {
    this.dialogRef.close(this.group);
  }

  onDismiss(): void {
    this.dialogRef.close(false);
  }

}

export class TasaSelectorModel {
  tasas: CotizacionTasa[];
}
