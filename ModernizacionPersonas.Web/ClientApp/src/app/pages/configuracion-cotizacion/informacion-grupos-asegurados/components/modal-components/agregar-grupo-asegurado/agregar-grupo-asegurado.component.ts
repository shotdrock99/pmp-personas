import { CurrencyPipe } from '@angular/common';
import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TipoSumaAsegurada } from 'src/app/models';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { PersonasReaderService } from 'src/app/services/personas-reader.service';


@Component({
  selector: 'app-agregar-grupo-asegurado',
  templateUrl: './agregar-grupo-asegurado.component.html',
  styleUrls: ['./agregar-grupo-asegurado.component.scss']
})
export class AgregarGrupoAseguradoComponent implements OnInit {
  grupoAseguradoForm: FormGroup;
  submitted = false;
  tiposSumaAsegurada: any[] = [];
  valorSalarioMinimo: number;
  esSumaFija = true;
  esSMMLV: boolean;
  currencyOptions = {
    prefix: '$ ',
    thousands: '.',
    decimal: ',',
    presicion: 0,
    allowNegative: false
  };

  currencyPipe = new CurrencyPipe('en-US');

  constructor(
    private formBuilder: FormBuilder,
    private cotizacionDataService: CotizacionPersistenceService,
    public dialogRef: MatDialogRef<AgregarGrupoAseguradoComponent>,
    private personasReaderService: PersonasReaderService,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  @ViewChild('nombreGrupo', { static: true })
  tbNombreGrupo: ElementRef;

  private nombresGrupos: any[] = this.data.nombresGrupos;

  get form() { return this.grupoAseguradoForm.controls; }

  ngOnInit(): void {
    this.grupoAseguradoForm = this.formBuilder.group({
      nombre: ['', {
        updateOn: 'blur',
        debounce: 1000,
        validators: [Validators.required, Validators.maxLength(50), Validators.pattern(/^[a-zA-ZñÑ0-9 \-_]*$/)]
      }],
      tipoSumaAsegurada: ['', Validators.required],
      sumaAseguradaMaxima: [0],
      sumaAseguradaMinima: [0],
      numeroSalariosAsegurar: [0]
    });

    this.loadTiposSumaAsegurada();
    this.registerNombreChange();
    this.registerTipoSumaAseguradaChange();
    this.registerSumaAseguradaMinimaChange();
    this.registerSumaAseguradaMaximaChange();
  }

  private registerNombreChange() {
    const ctrlNombre = this.grupoAseguradoForm.get('nombre');
    ctrlNombre
      .valueChanges
      .subscribe(change => {
        if (this.nombresGrupos.length > 0) {
          const idx = this.nombresGrupos.findIndex(x => x === change.toUpperCase());
          if (idx >= 0) {
            ctrlNombre.setErrors({ exist: true });
          }
        }
      });
  }

  private registerSumaAseguradaMaximaChange() {
    const ctrlSumaAseguradaMaxima = this.grupoAseguradoForm.get('sumaAseguradaMaxima');
    ctrlSumaAseguradaMaxima
      .valueChanges
      .subscribe(value => {
        const ctrlSumaAseguradaMinima = this.grupoAseguradoForm.get('sumaAseguradaMinima');
        const sumaAseguradaMinima = ctrlSumaAseguradaMinima.value;
        if (Number(value) < Number(sumaAseguradaMinima)) {
          ctrlSumaAseguradaMaxima.setErrors({ ltvalue: true });
          ctrlSumaAseguradaMinima.setErrors(null);
        }

        if (Number(value) === Number(sumaAseguradaMinima) && !ctrlSumaAseguradaMinima.pristine) {
          ctrlSumaAseguradaMaxima.setErrors({ eqvalue: true });
          ctrlSumaAseguradaMinima.setErrors(null);
        }
      });
  }

  private registerSumaAseguradaMinimaChange() {
    const ctrlSumaAseguradaMinima = this.grupoAseguradoForm.get('sumaAseguradaMinima');
    ctrlSumaAseguradaMinima
      .valueChanges
      .subscribe(value => {
        const ctrlSumaAseguradaMaxima = this.grupoAseguradoForm.get('sumaAseguradaMaxima');
        const sumaAseguradaMaxima = ctrlSumaAseguradaMaxima.value;
        if (Number(value) > Number(sumaAseguradaMaxima) && !ctrlSumaAseguradaMaxima.pristine) {
          ctrlSumaAseguradaMinima.setErrors({ gtvalue: true });
          ctrlSumaAseguradaMaxima.setErrors(null);
        }
        if (Number(value) === Number(sumaAseguradaMaxima) && !ctrlSumaAseguradaMaxima.pristine) {
          ctrlSumaAseguradaMinima.setErrors({ eqvalue: true });
          ctrlSumaAseguradaMaxima.setErrors(null);
        }
      });
  }

  private registerTipoSumaAseguradaChange() {
    this.grupoAseguradoForm.get('tipoSumaAsegurada')
      .valueChanges
      .subscribe(selection => {
        this.esSumaFija = selection.codigoTipoSumaAsegurada === 1;
        this.esSMMLV = selection.codigoTipoSumaAsegurada === 10;
        this.valorSalarioMinimo = selection.valorSalarioMinimo;
        this.updateSumaMinMaxValidators(selection);
      });
  }

  private updateSumaMinMaxValidators(tipoSumaAsegurada: TipoSumaAsegurada) {
    const capturaSalarios = tipoSumaAsegurada.codigoTipoSumaAsegurada === 2 || tipoSumaAsegurada.codigoTipoSumaAsegurada === 5
      || tipoSumaAsegurada.codigoTipoSumaAsegurada === 10;
    const ctrlSumaMax = this.grupoAseguradoForm.get('sumaAseguradaMaxima');
    const ctrlSumaMin = this.grupoAseguradoForm.get('sumaAseguradaMinima');
    const ctrlNumSalarios = this.grupoAseguradoForm.get('numeroSalariosAsegurar');
    if (!this.esSumaFija) {
      ctrlSumaMin.setValidators([Validators.required, Validators.min(1)]);
      ctrlSumaMax.setValidators([Validators.required, Validators.min(1)]);
    }  else {
      ctrlSumaMin.clearValidators();
      ctrlSumaMax.clearValidators();
    }

    ctrlSumaMin.updateValueAndValidity();
    ctrlSumaMax.updateValueAndValidity();
  }

  private loadTiposSumaAsegurada() {
    const tiposSumaAsegurada = this.cotizacionDataService.tiposSumaAsegurada;
    tiposSumaAsegurada.forEach(x => {
      x.disabled = (x.codigoTipoSumaAsegurada === 2 || x.codigoTipoSumaAsegurada === 5 || x.codigoTipoSumaAsegurada === 10);
    });
    this.tiposSumaAsegurada = tiposSumaAsegurada;
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  onAcceptClick(): void {
    this.submitted = true;
    const isValidForm: boolean = !this.grupoAseguradoForm.invalid;
    if (isValidForm) {
      const args = this.grupoAseguradoForm.getRawValue();
      this.dialogRef.close(args);
    }
  }
}
