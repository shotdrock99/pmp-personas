import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthorizationUser, CotizacionAuthorization, TransactionComment,
  NotifyCotizacionArgs, CotizacionTransaction } from 'src/app/models/cotizacion-authorization';
import { Observable, forkJoin } from 'rxjs';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { AutorizacionesWriterService } from 'src/app/services/autorizaciones-writer.service';
import { AutorizacionesReaderService } from 'src/app/services/autorizaciones-reader.service';
import { CotizacionState } from 'src/app/models';
import { startWith, map } from 'rxjs/operators';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { UploadFileService } from 'src/app/services/upload-file.service';

@Component({
  selector: 'app-informacion-autorizaciones-cotizacion',
  templateUrl: './informacion-autorizaciones-cotizacion.component.html',
  styleUrls: ['./informacion-autorizaciones-cotizacion.component.scss']
})
export class InformacionAutorizacionesCotizacionComponent implements OnInit {
  submitted = false;
  uploadSuccessfully = false;
  uploadCompleted =false;
  uploadFail=false;
  showError=false;
  showLoading=false;
  uploadError="";
  fileName="";
  notificationForm: FormGroup;
  notificationUsers: AuthorizationUser[] = [];
  filteredNotificationUsers: Observable<AuthorizationUser[]>;
  notifiedUserName: string;
  notifyDisabled = true;
  showHistory = false;
  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;
  authorizations: CotizacionAuthorization[] = [];
  hasDefaultUser: boolean;
  transactions: CotizacionTransaction[];
  lastTransaction: CotizacionTransaction;
  allowNotify: boolean;

  @Input() readonly: boolean;

  @Output() validate = new EventEmitter();

  constructor(
    private router: Router,
    private formBuilder: FormBuilder,
    private cotizacionDataService: CotizacionPersistenceService,
    private notificationService: NotificationService,
    private authorizationWriter: AutorizacionesWriterService,
    private authorizationReader: AutorizacionesReaderService,
    private uploadService: UploadFileService) { }

  get form() { return this.notificationForm.controls; }

  get selectedUsuarioNotificacion() {
    return this.notificationForm.get('usuarioNotificado').value;
  }

  ngOnInit() {
    this.notificationForm = this.formBuilder.group({
      usuarioNotificacion: ['', [Validators.required]],
      observaciones: ['', [Validators.required]],
      archivoAdjunto: ['']
    });

    this.loadData();
    this.registerUploadFailEvent();
    this.registerUploadSuccessEvent();
  }

  toggleHistory(e: Event) {
    e.stopPropagation();
    this.showHistory = !this.showHistory;
  }

  hideHistory(e: Event) {
    this.showHistory = false;
  }

  onBlurHistoryContainer(e: Event) {
    if (this.showHistory) {
      this.hideHistory(e);
    }
  }

  private loadData() {
    const codigoCotizacion = this.cotizacionDataService.cotizacion.codigoCotizacion;
    const version = this.cotizacionDataService.cotizacion.version;

    this.authorizationReader.getAuthorizationControls(codigoCotizacion, version)
      .subscribe(response => {
        if (!response) { return; }
        this.authorizations = this.authorizationWriter.sortAuthorizations(response.authorizations);
        this.authorizations.forEach(element => {
          if (element.nombreAmparo != null) {
            element.nombreAmparo = element.nombreAmparo.replace("(", " ").replace(")", " ");
          } if (element.nombreSeccion != null) {
            element.nombreSeccion= element.nombreSeccion.replace("(", " ").replace(")", " ");
          }
          if (element.codigoGrupoAsegurado == 0) {
            element.nombreAmparo = null;
    
          }
          
        });
        this.notificationUsers = response.users.filter((x: any) => x.activo);
        if (this.notificationUsers.length === 0) {
          const ctrlUsuarioNotificar = this.notificationForm.get('usuarioNotificacion');
          ctrlUsuarioNotificar.setErrors({ emptyUsers: true });
        }

        this.notifyDisabled = response.cotizacionState === CotizacionState.PendingAuthorization;
        this.notifiedUserName = this.cotizacionDataService.cotizacion.usuarioNotificado;

        this.updateForm();
        // define users observable
        this.defineUsuarioNotificacionObservable();
        this.allowNotify = this.authorizations.length > 0 && !this.readonly;
      });
  }

  private updateForm() {
    const user = this.notificationUsers.find(u => u.notificado);
    if (user) {
      this.hasDefaultUser = true;
      this.notificationForm.setValue({
        usuarioNotificacion: user,
        observaciones: '',
        archivoAdjunto: ['']
      });

      this.notificationForm.get('usuarioNotificacion').disable();
    }

    if (this.notifyDisabled || this.readonly) {
      // disable form controls
      this.notificationForm.get('observaciones').disable();
      this.notificationForm.get('usuarioNotificacion').disable();
    }
  }
  onFileChange(event) {
    if (event.target.files.length > 0) {
      const file = event.target.files[0];
      this.fileName = file.name;
      this.notificationForm.get('archivoAdjunto').setValue(file);

      this.uploadFile();
    }
  }
  private uploadFile() {
    const cotizacion = this.cotizacionDataService.cotizacion;
    this.uploadCompleted = false;
    this.showLoading = true;
    ;
    const uploadURL = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/authorizations/soportes/saveAttachmentToAuth`;
    const formData = new FormData();
    const data = this.notificationForm.get('archivoAdjunto').value;
    formData.append('file', data);
    this.uploadService.upload2(uploadURL, formData);
  }
  private registerUploadSuccessEvent() {
    this.uploadService.onUploadFinished.subscribe(res => {
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
  private defineUsuarioNotificacionObservable() {
    this.filteredNotificationUsers = this.notificationForm.get('usuarioNotificacion')
      .valueChanges
      .pipe(
        startWith(''),
        map(value => this._filterUsuariosNotificacion(value))
      );
  }

  private _filterUsuariosNotificacion(value: string): AuthorizationUser[] {
    if (typeof (value) === 'string') {
      return this.notificationUsers.filter(option => option.codigo.toLowerCase().includes(value.toLowerCase()));
    }
  }

  displayFn(field: any, item: any): string {
    return item[field];
  }

  notify() {
    this.submitted = true;
    const isValidForm = this.notificationForm.valid;
    if (isValidForm) {
      const ctrlUsuarioNotificacion = this.notificationForm.get('usuarioNotificacion');

      // replace last notified user
      const notifiedUser: AuthorizationUser = ctrlUsuarioNotificacion.value;
      this.notifiedUserName = notifiedUser.codigo;

      const dialogRef = this.hasDefaultUser ?
        this.notificationService.showAlert(
          `Se enviará la solicitud de autorización a <b>${notifiedUser.codigo} - ${notifiedUser.nombreRol}</b>`
          ) :
        this.notificationService.showConfirm(
          `¿Confirma el envío de la solicitud de autorización a <b>${notifiedUser.codigo} - ${notifiedUser.nombreRol}</b>?.`
          );

      dialogRef.afterClosed().subscribe(res => {
        if (res) {
          this.notifyAuthorization(notifiedUser);
        }
      });
    }
  }

  refresh() {
    this.loadData();
  }

  private notifyAuthorization(notifiedUser: AuthorizationUser) {
    const codigoCotizacion = this.cotizacionDataService.cotizacion.codigoCotizacion;
    const version = this.cotizacionDataService.cotizacion.version;
    const ctrlObservaciones = this.notificationForm.get('observaciones');

    const comments: TransactionComment[] = [{
      codigoRolAutorizacion: '',
      codigoTipoAutorizacion: 0,
      message: ctrlObservaciones.value
    }];

    const args: NotifyCotizacionArgs = {
      codigoCotizacion,
      version,
      // transactionId: 0,
      authorizationControls: this.authorizations,
      authorizationUser: notifiedUser,
      comments
    };
    const currentThis = this;
    this.authorizationWriter.notify(args, () => {
      currentThis.validate.emit();
    })
      .subscribe(res => {
        if (res) {
          this.notificationService.showToast(`La notificación ha sido enviada exitosamente.`);
          const currentRouter = this.router;
          setTimeout(() => {
            currentRouter.navigate(['/cotizaciones']);
          }, 2000);
        }
      });
  }
}
