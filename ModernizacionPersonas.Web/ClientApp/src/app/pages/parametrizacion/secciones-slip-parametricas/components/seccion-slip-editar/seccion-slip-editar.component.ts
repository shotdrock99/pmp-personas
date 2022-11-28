import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-seccion-slip-editar',
  templateUrl: './seccion-slip-editar.component.html',
  styleUrls: ['./seccion-slip-editar.component.scss']
})
export class SeccionSlipEditarComponent implements OnInit {

  editForm: FormGroup;
  title: string;
  codigo: number;
  seccion: string;
  grupo: number;
  especial: boolean;
  grupos: Grupo[] = [
    { value: 1, viewValue: 'Portada' },
    { value: 2, viewValue: 'Amparo' },
    { value: 3, viewValue: 'Cláusula' },
    { value: 4, viewValue: 'Disposiciones Finales' },
  ];

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<SeccionSlipEditarComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any) {
      this.title = data.title;
      this.codigo = data.codigo;
      this.seccion = data.seccion;
      this.grupo = data.grupo;
      this.especial = data.especial === 1 ? true : false;
     }

  ngOnInit() {
    this.editForm = this.fb.group({
      codigo: [this.codigo, []],
      seccion: [this.seccion.toUpperCase(), []],
      grupo: [this.grupo, []],
      especial: [this.especial, []]
    });
  }

  edit() {
    this.dialogRef.close(this.editForm.value);
  }

}

interface Grupo {
  value: number;
  viewValue: string;
}
