import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { UploadFileService } from 'src/app/services/upload-file.service';
import { environment } from 'src/environments/environment';
import { CausalesReaderService } from 'src/app/services/causales-reader.service';
import { CotizacionReaderService } from 'src/app/services/cotizacion-reader.service';
import { NotificationService } from 'src/app/shared/services/notification.service';

@Component({
  selector: 'app-aceptacion-cotizacion',
  templateUrl: './aceptacion-cotizacion.component.html',
  styleUrls: ['./aceptacion-cotizacion.component.scss']
})
export class AceptacionCotizacionDialogComponent implements OnInit {
  transactionId: any;
  uploadFail: boolean;
  uploadError: any;

  constructor(private formBuilder: FormBuilder,
    private uploadService: UploadFileService,
    public dialogRef: MatDialogRef<AceptacionCotizacionDialogComponent>,
    private causalesReaderService: CausalesReaderService,
    private notificacionService: NotificationService,
    private cotizacionReaderService: CotizacionReaderService,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  private URL_BASE = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;
  uploadCompleted: boolean;
  showLoading: boolean;
  fileName: string;
  submitted: boolean;
  causales: CausalAdjudicacion[];
  causal: CausalAdjudicacion;
  acceptForm: FormGroup;
  showError: boolean = false;
  uploadSuccessfully: boolean = false;
  firmas: any

  get form() { return this.acceptForm.controls; }

  ngOnInit() {
    this.acceptForm = this.formBuilder.group({
      causalRechazo: ['', [Validators.required]],
      observaciones: [''],
      archivoAdjunto: ['']
    });

    this.loadCausales();
    this.registerUploadFailEvent();
    this.registerUploadSuccessEvent();
    this.getDirectores(this.data.codigoCotizacion);
  }

  private loadCausales() {
    this.causalesReaderService.getCausales().subscribe(res => {
      this.causales = res.filter(x => x.activo && x.tipoCausal === 1);
    });
  }

  onFileChange(event) {
    if (event.target.files.length > 0) {
      let file = event.target.files[0];
      this.fileName = file.name;
      this.acceptForm.get('archivoAdjunto').setValue(file);

      this.uploadFile();
    }
  }

  private getDirectores(codigoCotizacion: number) {
    this.cotizacionReaderService
      .consultarFirmasRechazoAceptacion(codigoCotizacion)
      .subscribe((res) => {
        this.firmas = res;
      });
  }

  private uploadFile() {
    this.uploadCompleted = false;
    this.showLoading = true;
    let uploadURL = `${this.URL_BASE}/${this.data.codigoCotizacion}/confirmacion/soportes/upload?version=${this.data.version}`;
    let formData = new FormData();
    let data = this.acceptForm.get('archivoAdjunto').value;
    formData.append('file', data);
    this.uploadService.upload2(uploadURL, formData);
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
    const causal = this.acceptForm.get('causalRechazo').value;
    const isvalidForm = !this.acceptForm.invalid;
    if (isvalidForm) {
      
        this.dialogRef.close({
          transactionId: this.transactionId || 0,
          causalId: causal.codigoCausal,
          observaciones: this.acceptForm.get('observaciones').value,
          to: this.firmas.tomador.email,
          withCopy: this.firmas.firmas.map(x => x.email)
        });
      
    }
  }

  onDismiss(): void {
    this.dialogRef.close(false);
  }
}

export class CausalAdjudicacion {
  public codigoCausal: number;
  causalTexto: string;
  activo: boolean;
  externo: boolean;
  solidaria: boolean;
  tipoCausal: number;
}
