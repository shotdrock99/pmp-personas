import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-generate-expedicion-web',
  templateUrl: './generate-expedicion-web.component.html',
  styleUrls: ['./generate-expedicion-web.component.scss']
})
export class GenerateExpedicionWebComponent implements OnInit {

  generateForm: FormGroup;
  submitted: boolean;

  constructor(
    private formBuilder: FormBuilder,
    private dialogRef: MatDialogRef<GenerateExpedicionWebComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  get form() {
    return this.generateForm.controls;
  }

  ngOnInit() {
    this.generateForm = this.formBuilder.group({
      to: ["", [Validators.required, Validators.email]],
      observaciones: ["", [Validators.required]]
    });
  }

  onConfirm(): void {
    this.submitted = true;
    const isValidForm = !this.generateForm.invalid;
    if (isValidForm) {
      this.dialogRef.close({
        to: this.generateForm.get("to").value,
        observaciones: this.generateForm.get("observaciones").value
      });
    }
  }

  onDismiss(): void {
    this.dialogRef.close(false);
  }

}
