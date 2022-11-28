import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

interface TipoCausal {
  value: number;
  viewValue: string;
}

@Component({
  selector: 'app-causal-edit',
  templateUrl: './causal-edit.component.html',
  styleUrls: ['./causal-edit.component.scss']
})
export class CausalEditComponent implements OnInit {

  form: FormGroup;
  codigoCausal: number;
  title: string;
  causalTexto: string;
  activo = false;
  externo = false;
  solidaria = false;
  tipoCausal: number;
  tipoCausales: TipoCausal[] = [
    { value: 1, viewValue: 'Acepta' },
    { value: 2, viewValue: 'Rechazo' }
  ];

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<CausalEditComponent>,
    @Inject(MAT_DIALOG_DATA) data) {

    this.codigoCausal = data.id;
    this.title = data.title;
    this.causalTexto = data.causalTexto;
    this.activo = data.activo === 1 ? true : false;
    this.externo = data.externo === 1 ? true : false;
    this.solidaria = data.solidaria === 1 ? true : false;
    this.tipoCausal = data.tipoCausal;
  }

  ngOnInit() {
    this.form = this.fb.group({
      codigoCausal: [this.codigoCausal, []],
      causalTexto: [this.causalTexto, []],
      activo: [this.activo, []],
      externo: [this.externo, []],
      solidaria: [this.solidaria, []],
      tipoCausal: [this.tipoCausal, []],
    });
  }

  Edit() {
    this.dialogRef.close(this.form.value);
  }

  close() {
    this.dialogRef.close();
  }

}
