import { Component, OnInit, Inject } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";

@Component({
  selector: "app-variable-slip-crear",
  templateUrl: "./variable-slip-crear.component.html",
  styleUrls: ["./variable-slip-crear.component.scss"],
})

export class VariableSlipCrearComponent implements OnInit {

  createForm: FormGroup;
  title: string;
  tipoDato: string = 'S';
  tiposDatos: tipoDato[] = [
    { value: "S", viewValue: "Seleccione una opción..." },
    { value: "IN", viewValue: "Numérico" },
    { value: "MO", viewValue: "Moneda" },
    { value: "VC", viewValue: "Texto" },
  ];

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<VariableSlipCrearComponent>,
    @Inject(MAT_DIALOG_DATA) data) {
    this.title = data.title;
  }

  ngOnInit() {
    this.createForm = this.fb.group({
      nombreVariable: [null, [Validators.required]],
      descripcionVariable: [null, [Validators.required, Validators.maxLength(250)]],
      tipoDato: [null, []],
      valorVariable: [null, [Validators.min(0), Validators.required]],
      valorTope: [null, [Validators.min(0), Validators.required]],
    });
  }

  create(){
    if (!this.createForm.valid) {
      return;
    }
    this.dialogRef.close(this.createForm.value);
  }

}

interface tipoDato {
  value: string;
  viewValue: string;
}
