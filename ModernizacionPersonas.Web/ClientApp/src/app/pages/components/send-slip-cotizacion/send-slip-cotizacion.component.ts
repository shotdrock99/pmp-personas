import { Component, OnInit, Inject } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { AceptacionCotizacionDialogComponent } from "../aceptacion-cotizacion/aceptacion-cotizacion.component";
import { environment } from "src/environments/environment";
import { UploadFileService } from 'src/app/services/upload-file.service';
import { CotizacionReaderService } from 'src/app/services/cotizacion-reader.service';
import { NotificationService } from 'src/app/shared/services/notification.service';

@Component({
  selector: "app-send-slip-cotizacion",
  templateUrl: "./send-slip-cotizacion.component.html",
  styleUrls: ["./send-slip-cotizacion.component.scss"],
})
export class SendSlipCotizacionComponent implements OnInit {
  submitted: boolean;
  showLoading: boolean;
  fileName: string;
  uploadCompleted: boolean;
  transactionId: any;
  uploadFail: boolean;
  uploadError: any;
  showError: boolean = false;
  uploadSuccessfully: boolean = false;
  readonly: boolean;
  ocultarDirector: boolean;
  firmas: any;

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  constructor(
    private formBuilder: FormBuilder,
    private uploadService: UploadFileService,
    public dialogRef: MatDialogRef<SendSlipCotizacionComponent>,
    private cotizacionReaderService: CotizacionReaderService,
    private notificacionService: NotificationService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  sendSlipCotizacionForm: FormGroup;
  cc_emails: string[] = this.data.informacionEnvio;
  resend: boolean = this.data.resend;
  para: any = this.data.tomador;

  get form() {
    return this.sendSlipCotizacionForm.controls;
  }

  ngOnInit() {
    this.readonly = this.resend;
    if (!this.readonly) {
      this.sendSlipCotizacionForm = this.formBuilder.group({
        recipients: ["", []],
        comments: ["", []],
        archivoAdjunto: [""],
      });
    } else {
      this.sendSlipCotizacionForm = this.formBuilder.group({
        recipients: ["", [Validators.required]],
        comments: ["", []],
        archivoAdjunto: [""],
      });
    }

    if (this.data.resend) {
      this.updateForm();
    }

    this.registerUploadFailEvent();
    this.registerUploadSuccessEvent();
    this.buildEmailRecipients();
  }

  private updateForm() {
    this.sendSlipCotizacionForm.patchValue({
      //recipients: this.formBuilder.array([]),
      recipients: this.data.recipients,
      comments: this.data.comments,
    });
  }

  private buildEmailRecipients() {
    this.cotizacionReaderService
      .consultarFirmasRechazoAceptacion(this.data.codigoCotizacion)
      .subscribe((res) => {
        this.ocultarDirector = res.ocultarDirector;
        this.firmas = res.firmas.filter(x => this.cc_emails.includes(x.email)).map((firma) => {
          return {
            email: firma.email,
            cargo: firma.codigoCargo
          }
        })
      });
  }

  onFileChange(event) {
    if (event.target.files.length > 0) {
      let file = event.target.files[0];
      this.fileName = file.name;
      this.sendSlipCotizacionForm.get("archivoAdjunto").setValue(file);

      this.uploadFile();
    }
  }

  uploadFile() {
    this.uploadCompleted = false;
    this.showLoading = true;
    const url = `${this.BASE_URL}/${this.data.codigoCotizacion}/slip/saveAttachment`;
    let formData = new FormData();
    let data = this.sendSlipCotizacionForm.get('archivoAdjunto').value;
    formData.append('file', data);
    this.uploadService.upload2(url, formData);
  }

  private registerUploadSuccessEvent() {
    this.uploadService.onUploadFinished.subscribe(res => {
      this.transactionId = res.transactionId;
      this.uploadCompleted = true;
      this.uploadFail = false;
      this.showLoading = false;
    });
  }

  private registerUploadFailEvent() {
    this.uploadService.onUploadFail.subscribe(res => {
      this.uploadFail = true;
      this.uploadError = res.error;
      this.showLoading = false;
    });
  }

  onConfirm(): void {
    this.submitted = true;
    const comments = this.sendSlipCotizacionForm.get("comments").value;
    const recipients = this.sendSlipCotizacionForm.get("recipients").value;
    const isvalidForm = !this.sendSlipCotizacionForm.invalid;
    if (isvalidForm ) {
      if (!this.ocultarDirector || this.readonly) {
        this.dialogRef.close({
          to: this.para.email,
          recipients: recipients,
          comments: comments,
          withCopy: this.cc_emails,
          resend: this.resend
        });
      } else {
        const dialogRef = this.notificacionService.showConfirm("La cotización no cuenta con un director comercial asignado, ¿Desea continuar?");
        dialogRef.afterClosed().subscribe(result => {
          if (result) {
            this.dialogRef.close({
              to: this.para.email,
              recipients: recipients,
              comments: comments,
              withCopy: this.cc_emails,
              resend: this.resend
            });
          }
        });

      }
      
    }
  }

  onDismiss(): void {
    this.dialogRef.close(false);
  }
}
