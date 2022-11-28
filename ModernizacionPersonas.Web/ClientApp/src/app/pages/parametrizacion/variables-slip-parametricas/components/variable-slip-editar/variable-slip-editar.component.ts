import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-variable-slip-editar',
  templateUrl: './variable-slip-editar.component.html',
  styleUrls: ['./variable-slip-editar.component.scss']
})
export class VariableSlipEditarComponent implements OnInit {

  editForm: FormGroup;
  title: string;
  codigoVariable: number;
  nombreVariable: string;
  descripcionVariable: string;
  tipoDato: string;
  valorVariable: number;
  valorTope: number;
  tiposDatos: tipoDato[] = [
    { value: 'IN', viewValue: 'Numérico' },
    { value: 'MO', viewValue: 'Moneda' },
    { value: 'VC', viewValue: 'Texto' },
  ];

  constructor(private fb: FormBuilder,
    private dialogRef: MatDialogRef<VariableSlipEditarComponent>,
    @Inject(MAT_DIALOG_DATA) data) {
      this.title = data.title;
      this.codigoVariable = data.codigoVariable;
      this.nombreVariable = data.nombreVariable;
      this.descripcionVariable = data.descripcionVariable;
      this.tipoDato = data.tipoDato;
      this.valorVariable = data. valorVariable;
      this.valorTope = data.valorTope;
     }

  ngOnInit() {
    this.editForm = this.fb.group({
      codigoVariable: [this.codigoVariable, []],
      nombreVariable: [this.nombreVariable, []],
      descripcionVariable: [this.descripcionVariable, []],
      tipoDato: [this.tipoDato, []],
      valorVariable: [this.valorVariable, []],
      valorTope: [this.valorTope, []]
    });
  }

  edit(){
    this.dialogRef.close(this.editForm.value);
  }

}

interface tipoDato {
  value: string;
  viewValue: string;
}
