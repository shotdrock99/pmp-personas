import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AceptacionCotizacionDialogComponent } from '../aceptacion-cotizacion/aceptacion-cotizacion.component';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-continuar-cotizacion',
  templateUrl: './continuar-cotizacion.component.html',
  styleUrls: ['./continuar-cotizacion.component.scss']
})
export class ContinuarCotizacionComponent implements OnInit {

  constructor(
    private formBuilder: FormBuilder,
    public dialogRef: MatDialogRef<AceptacionCotizacionDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  lastModifyUser: string = this.data.lastModifyUser;
  continuarCotizacionForm: FormGroup;

  get form() { return this.continuarCotizacionForm.controls; }

  ngOnInit() {
    this.continuarCotizacionForm = this.formBuilder.group({
      observaciones: ['', [Validators.required]]
    });
  }

  onConfirm(): void {
    const observaciones = this.continuarCotizacionForm.get('observaciones').value;
    const isvalidForm = !this.continuarCotizacionForm.invalid;
    if (isvalidForm) {
      this.dialogRef.close({
        observaciones: observaciones
      });
    }
  }

  onDismiss(): void {
    this.dialogRef.close(false);
  }
}
