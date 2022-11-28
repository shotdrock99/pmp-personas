import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-seccion-slip-crear',
  templateUrl: './seccion-slip-crear.component.html',
  styleUrls: ['./seccion-slip-crear.component.scss']
})
export class SeccionSlipCrearComponent implements OnInit {

  createForm: FormGroup;
  title: string;
  grupo = 0;
  especial = false;
  grupos: Grupo[] = [
    { value: 0, viewValue: 'Seleccione una opción...' },
    { value: 1, viewValue: 'Portada' },
    { value: 2, viewValue: 'Amparo' },
    { value: 3, viewValue: 'Cláusula' },
    { value: 4, viewValue: 'Disposiciones Finales' },
  ];

  constructor(private fb: FormBuilder,
              private dialogRef: MatDialogRef<SeccionSlipCrearComponent>,
              @Inject(MAT_DIALOG_DATA) private data: any) {
      this.title = data.title;
     }

  ngOnInit() {
    this.createForm = this.fb.group({
      seccion: [null, [Validators.required]],
      grupo: [null, [Validators.required]],
      especial: [null, []]
    });
  }

  create() {
    if (!this.createForm.valid) {
      return;
    }
    this.dialogRef.close(this.createForm.value);
  }

}

interface Grupo {
  value: number;
  viewValue: string;
}
