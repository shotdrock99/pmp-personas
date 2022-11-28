import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { UploadFileService } from 'src/app/services/upload-file.service';
import { environment } from 'src/environments/environment';
import { CausalesReaderService } from 'src/app/services/causales-reader.service';
import { CotizacionReaderService } from 'src/app/services/cotizacion-reader.service';

@Component({
  selector: "app-cerrar-cotizacion",
  templateUrl: "./cerrar-cotizacion.component.html",
  styleUrls: ["./cerrar-cotizacion.component.scss"],
})
export class CerrarCotizacionDialogComponent implements OnInit {
  transactionId: any;
  uploadFail: boolean;
  uploadError: any;

  constructor(
    private formBuilder: FormBuilder,
    private uploadService: UploadFileService,
    public dialogRef: MatDialogRef<CerrarCotizacionDialogComponent>,
    private causalesReaderService: CausalesReaderService,
    private cotizacionReaderService: CotizacionReaderService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  private URL_BASE = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;
  fileName: any;
  uploadCompleted: boolean;
  showLoading: boolean;
  submitted: boolean;
  causales: CausalCierre[];
  causales1: CausalCierre[];
  causal: CausalCierre;
  closeForm: FormGroup;
  showError: boolean = false;
  uploadSuccessfully: boolean = false;
  firmas: any;

  get form() {
    return this.closeForm.controls;
  }

  ngOnInit() {
    this.closeForm = this.formBuilder.group({
      tipoRechazo: ["1"],
      causalRechazo: ["", [Validators.required]],
      observaciones: [""],
      archivoAdjunto: [""],
    });

    this.loadCausales();
    this.registerUploadSuccessEvent();
    this.registerUploadFailEvent();
    this.registerTipoRechazoChange();
    this.getDirectores(this.data.codigoCotizacion);
  }

  private loadCausales() {
    this.causalesReaderService.getCausales().subscribe((res) => {
      this.causales = res.filter(
        (x) => x.externo && x.activo && x.tipoCausal !== 1
      );
      this.causales1 = res;
    });
  }

  private registerTipoRechazoChange() {
    this.closeForm.get("tipoRechazo").valueChanges.subscribe((val) => {
      if (val === "1") {
        const ds = this.causales1.filter(
          (x) => x.externo && x.activo && x.tipoCausal !== 1
        );
        this.causales = ds;
      } else {
        const ds = this.causales1.filter(
          (x) => x.solidaria && x.activo && x.tipoCausal !== 1
        );
        this.causales = ds;
      }
    });
  }

  onFileChange(event) {
    if (event.target.files.length > 0) {
      let file = event.target.files[0];
      this.fileName = file.name;
      this.closeForm.get("archivoAdjunto").setValue(file);

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
    let data = this.closeForm.get("archivoAdjunto").value;
    formData.append("file", data);
    this.uploadService.upload2(uploadURL, formData);
  }

  private registerUploadSuccessEvent() {
    this.uploadService.onUploadFinished.subscribe((res) => {
      this.transactionId = res.transactionId;
      this.uploadCompleted = true;
      this.uploadFail = false;
      this.showLoading = false;
    });
  }

  private registerUploadFailEvent() {
    this.uploadService.onUploadFail.subscribe((res) => {
      this.uploadFail = true;
      this.uploadError = res.error;
      this.showLoading = false;
    });
  }

  onConfirm(): void {
    const causal = this.closeForm.get("causalRechazo").value;
    const tipoRechazo = this.closeForm.get("tipoRechazo").value;
    const isvalidForm = !this.closeForm.invalid;
    // const wc[] = this.firmas.firmas.map(x => x.email);
    if (isvalidForm) {
      this.dialogRef.close({
        transactionId: this.transactionId || 0,
        tipoRechazo: tipoRechazo,
        causalId: causal.codigoCausal,
        observaciones: this.closeForm.get("observaciones").value,
        to: this.firmas.tomador.email,
        withCopy: this.firmas.firmas.map(x => x.email)
      });
    }
  }

  onDismiss(): void {
    this.dialogRef.close(false);
    1;
  }
}

export class CausalCierre {
  public codigoCausal: number;
  causalTexto: string;
  activo: boolean;
  externo: boolean;
  solidaria: boolean;
  tipoCausal: number;
}
