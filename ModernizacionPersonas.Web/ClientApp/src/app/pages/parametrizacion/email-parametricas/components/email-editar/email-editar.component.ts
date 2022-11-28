import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { EmailParametrizacionReaderService } from 'src/app/services/email-parametrizacion-reader.service';
import { EmailParametrizacion } from 'src/app/models/email-parametrizacion';

@Component({
  selector: "app-email-editar",
  templateUrl: "./email-editar.component.html",
  styleUrls: ["./email-editar.component.scss"],
})
export class EmailEditarComponent implements OnInit {
  editForm: FormGroup;
  title: string;
  codigoParametrizacionEmail: number;
  codigoSeccion: number = 2;
  codigoTemplate: number;
  subject: string;
  texto: string;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<EmailEditarComponent>,
    private emailParametrizacionReader: EmailParametrizacionReaderService,
    @Inject(MAT_DIALOG_DATA) data
  ) {
    this.title = data.title;
    this.codigoTemplate = data.codigoTemplate;
    this.subject = data.subject;
  }

  ngOnInit() {
    this.getTextosEmailByTemplate(this.codigoTemplate, this.codigoSeccion);
    this.editForm = this.fb.group({
      codigoParametrizacionEmail: [this.codigoParametrizacionEmail, []],
      subject: [this.subject, []],
      codigoTemplate: [this.codigoTemplate, []],
      codigoSeccion: [this.codigoSeccion, []],
      texto: [this.texto, []],
    });
  }

  private getTextosEmailByTemplate(codigoTemplate: number, codigoSeccion: number) {
    this.emailParametrizacionReader
      .getTextosEmailByTemplate(codigoTemplate, codigoSeccion)
      .subscribe((response) => {
        this.editForm.patchValue({
          codigoParametrizacionEmail: response.codigoParametrizacionEmail,
          texto: response.texto,
        });
      });
  }

  edit() {
    let response: EmailParametrizacion = {
      codigoParametrizacionEmail: this.editForm.value
        .codigoParametrizacionEmail,
      codigoSeccion: this.editForm.value.codigoSeccion,
      codigoTemplate: this.editForm.value.codigoTemplate,
      texto: this.editForm.value.texto,
    };
    this.dialogRef.close(response);
  }
}
