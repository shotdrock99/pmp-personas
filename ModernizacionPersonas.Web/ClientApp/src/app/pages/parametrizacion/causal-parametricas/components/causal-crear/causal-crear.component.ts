import {
  PageToolbarItem,
  PageToolbarConfig,
} from "src/app/models/page-toolbar-item";
import { Causal } from "src/app/models/causal";
import { Component, OnInit, Inject } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { CausalesWriterService } from "src/app/services/causales-writer.services";
import { Router } from "@angular/router";
import { PageToolbarBuilder } from "src/app/shared/services/page-toolbar-builder";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";

interface TipoCausal {
  value: number;
  viewValue: string;
}

@Component({
  selector: "app-causal-crear",
  templateUrl: "./causal-crear.component.html",
  styleUrls: ["./causal-crear.component.scss"],
})
export class CausalCrearComponent implements OnInit {
  crearCausalForm: FormGroup;
  activoCheck = false;
  externoCheck = false;
  solidariaCheck = false;
  tipoC: number = 0;
  tipoCausales: TipoCausal[] = [
    { value: 0, viewValue: "Seleccione una opción..." },
    { value: 1, viewValue: "Acepta" },
    { value: 2, viewValue: "Rechazo" },
  ];
  title: string;

  constructor(
    private formBuilder: FormBuilder,
    private dialogRef: MatDialogRef<CausalCrearComponent>,
    @Inject(MAT_DIALOG_DATA) data
  ) {
    this.title = data.title;
  }

  ngOnInit() {
    this.crearCausalForm = this.formBuilder.group({
      descripcion: [null, [Validators.required]],
      activoCheck: [null, []],
      externoCheck: [null, []],
      solidariaCheck: [null, []],
      tipoC: [null, [Validators.required]],
    });
  }

  create() {
    if (!this.crearCausalForm.valid) {
      return;
    }
    const causal: Causal = {
      codigoCausal: 0,
      causalTexto: this.crearCausalForm.value.descripcion,
      activo: this.crearCausalForm.value.activoCheck ? 1 : 0,
      externo: this.crearCausalForm.value.externoCheck ? 1 : 0,
      solidaria: this.crearCausalForm.value.solidariaCheck ? 1 : 0,
      tipoCausal: this.crearCausalForm.value.tipoC,
    };
    this.dialogRef.close(causal);
  }
}
