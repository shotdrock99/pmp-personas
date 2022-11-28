import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-rechazo-autorizacion-cotizacion',
  templateUrl: './rechazo-autorizacion-cotizacion.component.html',
  styleUrls: ['./rechazo-autorizacion-cotizacion.component.scss']
})
export class RechazoAutorizacionCotizacionComponent implements OnInit {

  constructor(private formBuilder: FormBuilder,
    public dialogRef: MatDialogRef<RechazoAutorizacionCotizacionComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  submitted: boolean;
  causales: CausalRechazo[] = DATA;
  causal: CausalRechazo;
  rechazoForm: FormGroup;
  user: string = this.data.user;
  userRole: string = this.data.userRole;

  get form() { return this.rechazoForm.controls; }

  ngOnInit() {
    this.rechazoForm = this.formBuilder.group({
      causalRechazo: ['', [Validators.required]]
    });
  }

  onConfirm(): void {
    var isvalidForm = !this.rechazoForm.invalid;
    if (isvalidForm) {
      this.dialogRef.close(true);
    }
  }

  onDismiss(): void {
    this.dialogRef.close(false);
  }
}

const DATA = [
  { key: 1, value: 'CONTRATO INEXISTENTE PARA VERIFICACION DE CUMULOS' },
  { key: 2, value: 'ASEGURADO(S) EXCEDE(N) LIMITE DE RETENCION' },
  { key: 3, value: 'SOLICITUD CON VIGENCIA RETROACTIVA NO PERMITIDA' },
  { key: 4, value: 'POLIZA CON CARTERA PENDIENTE' },
  { key: 5, value: 'SOLICITUD CON VIGENCIA POSTERIOR NO PERMITIDA' },
  { key: 6, value: 'SOLICITUD CON ASEGURADOS EN CATEGORIAS CON AUTORIZACION' },
  { key: 7, value: 'CATEGORIAS CON DIAS DE CARENCIA MENOR AL DEF EN EL PRODUCTO' },
  { key: 8, value: 'EXISTEN ASEGURADOS CON INGRESO RETROACTIVO' },
  { key: 9, value: 'EXISTEN CATEGORIAS CON TASAS INF A LAS DEF EN EL PRODUCTO' },
  { key: 10, value: 'CATEGORIAS CON Nro DE ASEG DIF A LAS DEF EN EL PRODUCTO' },
  { key: 11, value: 'CATEGORIAS CON LIMITES DE EDAD DIF A LOS DEF EN EL PRODUCTO' },
  { key: 12, value: 'CATEGORIAS CON EDAD MIN DE INGRESO DIF A LAS DEF EN EL PROD' },
  { key: 13, value: 'EXISTEN PARENTESCOS DIF A LOS DEFINIDOS EN EL PRODUCTO' },
  { key: 14, value: 'EXISTEN PARENTESCOS ADICIONALES A LOS DEF EN EL PRODUCTO' },
  { key: 15, value: 'OTROS' },
  { key: 16, value: 'DÍAS POSTERIORES EN ASEGURADOS' },
  { key: 17, value: 'CONTROL TASA' },
  { key: 19, value: 'CONTROL ÍNDICE COMBINADO' },
  { key: 20, value: 'CONTROL MÁXIMO NÚMERO DE ASEGURADOS POR PÓLIZA' },
  { key: 21, value: 'CONTROL MÁXIMO VALOR ASEGURADO POR PÓLIZA' },
  { key: 22, value: 'CONTROL GRUPO OCUPACIÓN' },
  { key: 23, value: 'CONTROL RIESGO DE PROHIBIDA ACEPTACION' },
  { key: 24, value: 'CONTROL RETROACTIVIDAD VIGENCIA DESDE' },
  { key: 25, value: 'CONTROL DÍAS POSTERIORES VIGENCIA DESDE' },
  { key: 26, value: 'CONTROL MÁXIMO NÚMERO DÍAS DE VIGENCIA' },
  { key: 28, value: 'CONTROL MÁXIMO VALOR ASEGURADO INDIVIDUAL ACUMULADO' },
  { key: 30, value: 'CONTROL MÁXIMO VALOR ASEGURADO INDIVIDUAL MULTIPRODUCTO' },
  { key: 32, value: 'CONTROL AMPARO AUTOMÁTICO' },
  { key: 33, value: 'CONTROL COMISIÓN' },
  { key: 34, value: 'CONTROL GASTOS DE ADMINISTRACIÓN' },
  { key: 35, value: 'CONTROL PÓLIZAS GLOBALIZADAS' },
  { key: 36, value: 'PENDIENTE ACTUALIZACION DE DATOS DEL TOMADOR' }]

export interface CausalRechazo {
  key: number;
  value: string;
}
