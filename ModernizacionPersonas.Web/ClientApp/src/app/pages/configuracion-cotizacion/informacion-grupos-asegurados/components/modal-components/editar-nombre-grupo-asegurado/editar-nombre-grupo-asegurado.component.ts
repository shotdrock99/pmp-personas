import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AgregarGrupoAseguradoComponent } from '../agregar-grupo-asegurado/agregar-grupo-asegurado.component';
import { PersonasReaderService } from 'src/app/services/personas-reader.service';

@Component({
  selector: 'app-editar-nombre-grupo-asegurado',
  templateUrl: './editar-nombre-grupo-asegurado.component.html',
  styleUrls: ['./editar-nombre-grupo-asegurado.component.scss']
})
export class EditarNombreGrupoAseguradoComponent implements OnInit {
  grupoAseguradoForm: FormGroup;
  submitted = false;

  private nombresGrupos: any[] = this.data.nombresGrupos;

  constructor(private formBuilder: FormBuilder,
    private cotizacionDataService: CotizacionPersistenceService,
    public dialogRef: MatDialogRef<AgregarGrupoAseguradoComponent>,
    private personasReaderService: PersonasReaderService,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  get form() { return this.grupoAseguradoForm.controls; }

  ngOnInit() {
    this.grupoAseguradoForm = this.formBuilder.group({
      nombre: [this.data.nombreGrupo, {
        updateOn: 'blur',
        debounce: 1000,
        validators: [Validators.required, Validators.maxLength(50), Validators.pattern(/^[a-zA-ZñÑ0-9 \-_]*$/)]
      }]
    });

    this.registerNombreChange();
  }

  private registerNombreChange() {
    let ctrlNombre = this.grupoAseguradoForm.get('nombre');
    ctrlNombre
      .valueChanges
      .subscribe(change => {
        if (this.nombresGrupos.length > 0) {
          let idx = this.nombresGrupos.findIndex(x => x === change.toUpperCase())
          if (idx >= 0) {
            ctrlNombre.setErrors({ 'exist': true });
          }
        }
      });
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  onAcceptClick(): void {
    this.submitted = true;
    var isValidForm: boolean = !this.grupoAseguradoForm.invalid;
    if (isValidForm) {
      var args = this.grupoAseguradoForm.getRawValue();
      this.dialogRef.close(args);
    }
  }

}
