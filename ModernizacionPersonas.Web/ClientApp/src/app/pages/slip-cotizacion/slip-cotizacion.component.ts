import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { PageToolbarConfig } from 'src/app/models/page-toolbar-item';
import { GenerarSlipDataResponse, Slip, SlipSeccion } from 'src/app/models/slip';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { CotizacionWriterService } from 'src/app/services/cotizacion-writer.service';
import { PreviewSlipReaderService } from 'src/app/services/preview-slip-reader.service';
import { PrintService } from 'src/app/services/print.service';
import { UploadFileService } from 'src/app/services/upload-file.service';
import {
  AlertDialogComponent,
  AlertDialogModel,
} from 'src/app/shared/components/alert-dialog';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { PageToolbarBuilder } from 'src/app/shared/services/page-toolbar-builder';
import { environment } from 'src/environments/environment';

import { SendSlipCotizacionComponent } from './../components/send-slip-cotizacion/send-slip-cotizacion.component';
import { AlertDialogPreventCloseComponent } from 'src/app/shared/components/alert-dialog/alert-dialog.preventClose.component';
import { CotizacionState } from 'src/app/models';
import { CotizacionValidationResponse } from 'src/app/models/cotizacion-authorization';
import { CotizacionReaderService } from 'src/app/services/cotizacion-reader.service';
import { BehaviorSubject } from 'rxjs';
import { TableGastosTraslado } from '../../models/tableitem';
declare var $: any;
declare var jQuery: any;

@Component({
  selector: 'app-slip-cotizacion',
  templateUrl: './slip-cotizacion.component.html',
  styleUrls: ['./slip-cotizacion.component.scss'],
})
export class SlipCotizacionComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private router: Router,
    private toolbarBuilder: PageToolbarBuilder,
    private uploadService: UploadFileService,
    private cotizacionDataService: CotizacionPersistenceService,
    private cotizacionWriterService: CotizacionWriterService,
    private notificationService: NotificationService,
    private printService: PrintService,
    private slipReaderService: PreviewSlipReaderService,
    private cotizacionReaderService: CotizacionReaderService
  ) { }

  @ViewChild('printarea')
  content: ElementRef;

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  codigoCotizacion: number;
  version: number;
  tieneVigencia: boolean = true;
  numeroCotizacion: string;
  dateFormats = ['LL', 'DD/MM/YYYY'];
  pageLoaded = true;
  processing = false;
  tieneDobleAmparoDesmem = false;
  amparosTables: SlipSeccion[] = [];
  model: Slip;
  modelDialog: Slip;
  toolbarConfig: PageToolbarConfig;
  readonly: boolean;
  estadoCotizacion: CotizacionState;
  disabledTabSubject = new BehaviorSubject<any>(null);
  showPDFPreview = false;
  dataGastosTraslado: TableGastosTraslado[];
  get cotizacion() {
    return this.cotizacionDataService.cotizacion;
  }

  ngOnInit() {
    // configura el lenguage de moment
    moment().locale('es');
    this.route.data.subscribe((res) => {
      ;
      this.codigoCotizacion = res.data.codigoCotizacion;
      this.version = res.data.version;
      this.numeroCotizacion = res.data.numero;
      this.readonly = res.data.readonly;
      this.cotizacionDataService.setCotizacionState(res.data.estado);
      this.estadoCotizacion = res.data.estado;
      this.validateCotizacion(2).then((r: boolean) => {
        debugger;
        if (!r) {
          this.router.navigate(['/cotizaciones', this.codigoCotizacion, 'slip', 'config']);
        }
      });
      this.controlSlipView();




    });
    this.initializeToolbar();
    this.initializateTableGastos();
  }

  initializeToolbar() {
    const items = [
      {
        name: 'back',
        icon_path: 'home',
        label: '',
        tooltip: 'Volver a cotizaciones',
        isEnabled: true,
        fixed: true,
        onClick: () => {
          this.navigateToCotizaciones();
        },
      },
      // {
      //   name: "print",
      //   icon_path: "pageview",
      //   label: "Previsualizar",
      //   tooltip: "Previsualizar cotización",
      //   isEnabled: true,
      //   fixed: true,
      //   onClick: () => {
      //     this.printPdf();
      //   },
      // },
      {
        name: 'send',
        icon_path: 'email',
        label: 'Enviar',
        tooltip: 'Enviar cotización',
        isEnabled: true && !this.readonly,
        fixed: true,
        onClick: () => {
          this.sendCotizacion();
        },
      },
      {
        name: 'resend',
        icon_path: 'mail_outline',
        label: 'Reenviar',
        tooltip: 'Reenviar cotización',
        isEnabled: false || this.readonly,
        fixed: false,
        onClick: () => {
          this.resendCotizacion();
        },
      },
    ];

    this.toolbarConfig = this.toolbarBuilder.build(items);
  }

  navigateToCotizaciones() {
    this.router.navigate(['cotizaciones']);
  }

  triggerUnlockCotizacion() {
    this.cotizacionWriterService
      .unlockCotizacion(this.codigoCotizacion, this.version)
      .subscribe((res) => res);
  }

  private validateCotizacion(flag: number) {
    return new Promise((resolve, reject) => {
      this.cotizacionReaderService.validateCotizacion(this.codigoCotizacion, this.version, flag)
        .subscribe((res: CotizacionValidationResponse) => {
          var hasControls = false;
          if (res.validation.authorizations != null) {
            hasControls = res.validation && res.validation.authorizations.length > 0;
          }

          if (res.isValid) {
            if (hasControls && res.cotizacionState < CotizacionState.ApprovedAuthorization) {
              const cotizacion = this.cotizacionDataService.cotizacion;
              cotizacion.authorizationInfo = res.validation;
              this.notificationService.showAlert('Existen controles de autorización que deben ser revisados antes de continuar.');
              this.disabledTabSubject.next({
                tabId: 4,
                disabled: true,
                state: res.cotizacionState
              });
              resolve(false);
              return;
            }
            // si no hay controles de validacion...
            if (res.cotizacionState < CotizacionState.Sent) {
              // si hay multiples tasas configuradas...
              resolve(true);
            }
          } else {
            const message = res.validation.validationMessage.message;
            this.notificationService.showAlert(message);
            resolve(false);
          }
        });
    });
  }
  private consultarPDF() {
    let args: any;

    args = {
      codigoCotizacion: this.codigoCotizacion,
      comments: "",
      recipients: "",
      withCopy: "",
      resend: true,
      NumeroCotizacion: this.numeroCotizacion,
      Version: this.version
    };

    this.slipReaderService.ConsultarPDFSlipCotizacion(args).subscribe((response: any) => {

      $('#PDFView').attr('src', 'data:application/pdf;base64, ' + response.rutaPDF + '#toolbar=0&zoom=150');

    });
  }
  private consultarSlipData(pdfActive) {
    this.pageLoaded = false;
    this.slipReaderService
      .consultarSlip(this.codigoCotizacion, this.version)
      .subscribe((response: GenerarSlipDataResponse) => {
        if (!response) {
          return;
        }
        ;
        if (pdfActive) {
          this.modelDialog = response.data;
          this.consultarPDF();
        } else {
          this.model = response.data;
          var tieneAmp51 = false;
          var tieneAmp4 = false;
          this.model.amparos.amparos.forEach(element => {
            if (element.codigoAmparo == '4') {
              tieneAmp4 = true;
            }
            if (element.codigoAmparo == '51') {
              tieneAmp51 = true;
            }
          });
          if (tieneAmp4 && tieneAmp51) {
            this.model.amparos.amparos.forEach(element => {
              if (element.codigoAmparo != '4') {
                this.amparosTables.push(element)
              }

            });
          } else {
            this.model.amparos.amparos.forEach(element => {
              this.amparosTables.push(element);
            });
          }


          var numCot = +response.numeroCotizacion
          this.model.asunto = response.data.asunto.replace("COTIZACIÓN", "COTIZACIÓN <b>" + numCot + " VR. " + this.version + "</b>");
          // tslint:disable-next-line: max-line-length
          this.model.imagenFirmaUri = 'data:image/jpeg;base64,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';
          if (this.model.vigencia.hasta == "0001-01-01T00:00:00" && this.model.vigencia.desde == "0001-01-01T00:00:00") {
            this.tieneVigencia = false;
          } else {
            this.tieneVigencia = true;
          }
          this.model.fechaString = moment(this.model.fecha)
            .locale('es')
            .format(this.dateFormats[0]);
          this.model.vigencia.desdeString = moment(this.model.vigencia.desde)
            .locale('es')
            .format(this.dateFormats[0]);
          this.model.vigencia.hastaString = moment(this.model.vigencia.hasta)
            .locale('es')
            .format(this.dateFormats[0]);

        }
        this.cotizacionDataService.setCotizacionState(response.cotizacionState);
        this.pageLoaded = true;

      });
  }

  private refresh() {
    this.pageLoaded = false;
    this.consultarSlipData(false);
  }

  private printPdf() {
    // show alert
    this.notificationService.showToast('Generando Preview', 2000);

    this.processing = true;
    const element = this.content.nativeElement;
    // this.printService.printHtml(element);
    const options = {
      headerImageUri: this.model.infoEncabezado.headerImageUri,
      footerImageUri: this.model.infoEncabezado.footerImageUri,
      codigoRamo: this.model.codigoRamo,
      codigoSubramo: this.model.codigoSubramo,
    };

    this.printService.printFromImage(element, options, () => {
      this.processing = false;
    });
  }

  private resendCotizacion() {
    this.sendCotizacion(true);
  }

  private sendCotizacion(resend: boolean = false) {
    var modelo;
    if (this.model == undefined) {
      modelo = this.modelDialog;
    } else {
      modelo = this.model;
    }
    const dialogRef = this.dialog.open(SendSlipCotizacionComponent, {
      minWidth: '600px',
      data: {
        codigoCotizacion: this.codigoCotizacion,
        version: this.version,
        numeroCotizacion: this.numeroCotizacion,
        recipients: [],
        comments: '',
        resend,
        informacionEnvio: modelo.informacionEnvio,
        tomador: modelo.tomadorIntermediario,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        const message = 'Espere un momento mientras enviamos la propuesta.';
        const dialogData = new AlertDialogModel('Información', message);
        const dialogRef2 = this.dialog.open(AlertDialogPreventCloseComponent, {
          disableClose: true,
          maxWidth: '600px',
          data: dialogData,
        });

        this.processing = true;

        // if (!resend) {
        //   this.saveSlip().then((res) => {
        //     if (!res) {
        //       this.notificationService.showAlert(
        //         "Hubo un error generando el archivo PDF del Slip. Intente nuevamente."
        //       );
        //       return;
        //     }
        //     this.sendSlip(result, dialogRef2);
        //   });
        // } else {
        //   this.sendSlip(result, dialogRef2);
        // }
        this.sendSlip(result, dialogRef2);

      }
    });
  }

  private sendSlip(result: any, dialogRef2: any) {
    let args: any;
    if (!result.resend) {
      const recipients: string[] = result.recipients.split(', ');
      const recipients1: string[] = [];
      const to: string = result.to;
      recipients1.push(to);
      args = {
        codigoCotizacion: this.codigoCotizacion,
        comments: result.comments,
        NumeroCotizacion: this.numeroCotizacion,
        Version: this.version,
        recipients: recipients1,
        withCopy: result.withCopy.concat(recipients),
        resend: result.resend
      };
    } else {
      const recipients: string[] = result.recipients.split(', ');
      args = {
        codigoCotizacion: this.codigoCotizacion,
        comments: result.comments,
        Version: this.version,
        NumeroCotizacion: this.numeroCotizacion,
        recipients,
        resend: result.resend
      };
    }

    this.cotizacionWriterService.sendSlipCotizacion(args).subscribe((res) => {
      dialogRef2.close();

      if (res) {
        this.processing = true;

        const message = 'El correo fue enviado exitosamente.';
        const dialogData = new AlertDialogModel('Información', message);
        const dialogRef3 = this.dialog.open(AlertDialogComponent, {
          maxWidth: '400px',
          data: dialogData,
        });

        dialogRef3.afterClosed().subscribe(() => {
          this.refreshView();
        });
      }
    });
  }

  refreshView() {
    this.readonly = true;
    this.estadoCotizacion = CotizacionState.Sent;
    this.cotizacion.estado = CotizacionState.Sent;

    this.initializeToolbar();
    this.controlSlipView();

  }
  private controlSlipView() {
    if (this.estadoCotizacion > 1200) {
      this.consultarSlipData(true);
      this.showPDFPreview = true;
    } else {
      this.consultarSlipData(false);
      this.showPDFPreview = false;
    }
  }
  private saveSlip() {
    return new Promise((resolve, reject) => {
      this.uploadService.onUploadFinished.subscribe((res) => {
        resolve(res);
      });

      const element = this.content.nativeElement;
      const options = {
        headerImageUri: this.model.infoEncabezado.headerImageUri,
        footerImageUri: this.model.infoEncabezado.footerImageUri,
        codigoRamo: this.model.codigoRamo,
        codigoSubramo: this.model.codigoSubramo,
      };

      const url = `${this.BASE_URL}/${this.codigoCotizacion}/slip/save`;
      this.printService.getBlob(element, options).then((blob) => {
        const formData = new FormData();
        formData.append('file', blob);
        this.uploadService.upload2(url, formData);
      });
    });
  }

  continue() { }

  initializateTableGastos() {
    this.dataGastosTraslado = [{
      column1: 'Si, La condición clínica del paciente necesita maniobras de reanimación porque no pueda respirar, esté inconsciente, esté perdiendo sangre de manera masiva o haya perdido un miembro u órgano.',
      column2: 'Requiere el traslado en ambulancia medicalizada',
      column3: 'Trauma craneoencefálico con pérdida de la conciencia, aunque la misma no sea prolongada, trauma en tórax que restrinja la respiración, heridas profundas o extensas con difícil control del sangrado, avulsión de un miembro o parte de él.'
    }, {
      column1: 'Si, se hace evidente que el paciente se esté deteriorando desde el momento en el que ocurrió el accidente, hasta el momento en el que se está recibiendo la llamada, o si el dolor manifestado por el mismo es de carácter insoportable.',
      column2: 'Requiere traslado en ambulancia básica',
      column3: 'Trauma craneoencefálico que no mostro sintomatología desde el principio, pero que se manifestó en el transcurso de los minutos subsiguientes al accidente, dolor que aumenta de manera importante, deformidades que se hacen más notorias a medida que pasa el tiempo.'
    }, {
      column1: 'Si, La condición clínica del paciente requiere de medidas diagnósticas y terapéuticas en un servicio de urgencias, pero se encuentra estable, aunque su situación puede empeorar si no se actúa.',
      column2: 'Paciente puede trasladarse en un medio diferente a la ambulancia, siempre y cuando se encuentre acompañado de un adulto y el medio definido, se encuentre en condiciones aptas para ello.',
      column3: 'Accidentes menores en los que el paciente no pierde la conciencia y mantiene una actitud que se modifica, solo en función del trauma. El paciente se queja, pero se encuentra controlado en espera de la atención definitiva.'
    }, {
      column1: 'Si el paciente presenta condiciones médicas que no comprometen su estado general, ni representan un riesgo evidente para la vida o pérdida de miembro u órgano. No obstante, existen riesgos de complicación o secuelas de la enfermedad o lesión si no recibe la atención correspondiente.',
      column2: 'Paciente puede trasladarse en un medio diferente a la ambulancia, siempre y cuando se encuentre acompañado de un adulto y el medio definido, se encuentre en condiciones aptas para ello.',
      column3: 'Accidentes leves, que requieren la prestación de primeros auxilios, pero que no interfieren con la funcionalidad general del paciente'
    }, {
      column1: 'Si, El paciente presenta una condición clínica relacionada con problemas agudos o crónicos sin evidencia de deterioro que comprometa el estado general de paciente y no representa un riesgo evidente para la vida o la funcionalidad de miembro u órgano.',
      column2: 'Paciente puede trasladarse en un medio diferente a la ambulancia, siempre y cuando se encuentre acompañado de un adulto y el medio definido, se encuentre en condiciones aptas para ello.',
      column3: 'Situaciones en las que el paciente, requiere atención, pero esta, es susceptible de ser postergada y manejada en un entorno diferente al de un servicio de urgencias.'
    }]

  }
}
