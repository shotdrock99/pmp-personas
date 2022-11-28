import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { Cotizacion } from 'src/app/models';
import { ApplicationUser } from 'src/app/models/application-user';
import {
  AuthorizationUser,
  AutorizacionAction,
  AutorizacionArgs,
  AutorizacionTipoTasa,
  ChangesArgs,
  CotizacionAuthorization,
  TransactionComment,
  UserRole,
} from 'src/app/models/cotizacion-authorization';
import { PageToolbarConfig, PageToolbarItem } from 'src/app/models/page-toolbar-item';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AutorizacionesReaderService } from 'src/app/services/autorizaciones-reader.service';
import { AutorizacionesWriterService } from 'src/app/services/autorizaciones-writer.service';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { PageToolbarBuilder } from 'src/app/shared/services/page-toolbar-builder';
import { environment } from 'src/environments/environment';
import { UploadFileService } from 'src/app/services/upload-file.service';

import {
  AuthorizationTransactionsModalComponent,
} from '../components/authorization-transactions-modal/authorization-transactions-modal.component';
import {
  RechazoAutorizacionCotizacionComponent,
} from '../components/rechazo-autorizacion-cotizacion/rechazo-autorizacion-cotizacion.component';
declare var $: any;
declare var jQuery: any;

@Component({
  selector: 'app-authorize-cotizacion',
  templateUrl: './authorize-cotizacion.component.html',
  styleUrls: ['./authorize-cotizacion.component.scss']
})
export class AuthorizeCotizacionComponent implements OnInit {
  codigoCotizacion: number;
  lastModifyUser: string;
  lastRoleModifyUser: string;
  cotizacion: Cotizacion;
  infoReadonly: boolean = true;
  private ROLES = [
    { id: 1, name: 'Auxiliar Suscripción' },
    { id: 2, name: 'Dir. Técnico Agencia' },
    { id: 3, name: 'Gerente Agencia' },
    { id: 4, name: 'Dir. Técnico Zonal' },
    { id: 5, name: 'Profesional GSP' },
    { id: 6, name: 'Coordinador GSP' },
    { id: 7, name: 'Gerente GSP' },
    { id: 8, name: 'Vice-Presidente' }];


  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private toolbarBuilder: PageToolbarBuilder,
    private uploadService: UploadFileService,
    private dialog: MatDialog,
    private cotizacionDataService: CotizacionPersistenceService,
    private autorizacionesWriterService: AutorizacionesWriterService,
    private authorizationReader: AutorizacionesReaderService,
    private authorizationWriter: AutorizacionesWriterService,
    private notificacionService: NotificationService,
    private formBuilder: FormBuilder,
    private authenticationService: AuthenticationService) { }

  toolbarConfig: PageToolbarConfig;

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  transactionId: number;
  autorizacionForm: FormGroup;
  delegationAuths: CotizacionAuthorization[] = [];
  delegationUsers: AuthorizationUser[];
  notifiedUser: string;
  submitted: boolean;
  fileName: string;
  uploadEnabled = true;
  showLoading = false;
  uploadFail = false;
  showError = false;
  uploadError: string;
  uploadCompleted = false;
  uploadSuccessfully = false;
  showHistory = false;
  showTasaSelector = false;
  authorizations: CotizacionAuthorization[];
  users: AuthorizationUser[];
  loggedUser: ApplicationUser;
  currentCotizacion: Cotizacion;
  version: number;
  numeroCotizacion: string;

  get form() { return this.autorizacionForm.controls; }

  get authorizationComments() { return (this.autorizacionForm.get('authorizationComments') as FormArray).controls; }

  tasas: AutorizacionTipoTasa[];

  ngOnInit() {
    this.initializeToolbar();

    this.loggedUser = this.authenticationService.currentUserValue;
    if (this.loggedUser.rol.roleId == 6 || this.loggedUser.rol.roleId == 7 || this.loggedUser.rol.roleId == 8) {
      this.infoReadonly = false;
    }
    const param0 = this.route.snapshot.paramMap.get('cod_cotiza');
    this.codigoCotizacion = Number(param0);

    this.autorizacionForm = this.formBuilder.group({
      authorizationComments: this.formBuilder.array([]),
      assignedUser: [''],
      observaciones: ['', [Validators.required]],
      archivoAdjunto: [''],
      gastosCompania: ['', [Validators.required]],
      utilidadCompania: ['', [Validators.required]],
      tasa: [0]
    });

    this.loadData();

    this.tasas = [{
      codigoTasa: 1,
      nombreTasa: 'Ramo Amparo',
      valor: 3.5
    }, {
      codigoTasa: 5,
      nombreTasa: 'Siniestralidad',
      valor: 4.8
    }];

    this.registerUploadSuccessEvent();
    this.registerUploadFailEvent();
  }

  initializeToolbar() {
    const items: PageToolbarItem[] = [{
      name: 'back',
      icon_path: 'home',
      label: '',
      tooltip: 'Volver a autorizaciones',
      isEnabled: true,
      fixed: true,
      onClick: () => {
        this.navigateToAutorizaciones();
      }
    }, {
      name: 'navigate_cotizacion',
      icon_path: 'open_in_new',
      label: 'Abrir cotización',
      tooltip: 'Abrir cotización',
      isEnabled: true,
      fixed: true,
      onClick: () => {
        this.navigateCotizacion();
      }
    }, {
      name: 'show_history',
      icon_path: 'history',
      label: 'Ver movimientos',
      tooltip: 'Ver movimientos',
      isEnabled: true,
      fixed: true,
      onClick: (e) => {
        this.openHistory();
      }
    }, {
      name: 'timeline',
      icon_path: 'timeline',
      label: 'Ver histórico',
      tooltip: 'Ver histórico',
      isEnabled: true,
      fixed: true,
      onClick: () => this.router.navigate(['/cotizaciones', this.codigoCotizacion, 'timeline'])
    }, {
      separator: true
    }, {
      name: 'accept',
      icon_path: 'done',
      label: 'Autorizar',
      tooltip: 'Autorizar',
      isEnabled: true,
      fixed: true,
      onClick: () => {
        this.accept();
      }
    }, {
      name: 'reject',
      icon_path: 'clear',
      label: 'No Autorizar',
      tooltip: 'No Autorizar',
      isEnabled: true,
      fixed: true,
      onClick: () => {
        this.reject();
      }
    }, {
      name: 'return_back',
      icon_path: 'swap_horiz',
      label: 'Devolver',
      tooltip: 'Devolver',
      isEnabled: true,
      fixed: true,
      onClick: () => {
        this.sendBack();
      }
    }];

    this.toolbarConfig = this.toolbarBuilder.build(items);
  }

  private navigateCotizacion() {
    this.router.navigate(['cotizaciones', this.codigoCotizacion]);
  }

  private navigateToAutorizaciones() {
    this.router.navigate(['autorizaciones']);
  }

  private buildAuthorizationComments() {
    debugger;
    const authorizationCommentsFormArray = this.autorizacionForm.get('authorizationComments') as FormArray;
    
    this.delegationUsers.forEach((user: AuthorizationUser) => {

      const control = this.formBuilder.group({
        codigoCotizacion: user.codigoCotizacion,
        versionCotizacion: user.versionCotizacion,
        codigo: user.codigo,
        codigoRol: user.codigoRol,
        nombreRol: user.nombreRol,
        codigoNivel: user.codigoNivel,
        codigoTipoAutorizacion: user.codigoTipoAutorizacion,
        activo: user.activo,
        especial: user.especial,
        notificado: user.notificado,
        observaciones: ['', [Validators.required]]
      });
      authorizationCommentsFormArray.push(control);
      // update attachment behavior
      const ctrl = this.autorizacionForm.get('archivoAdjunto');
      if (this.delegationUsers.filter(x => !x.activo).length > 0) {
        ctrl.setValidators([Validators.required]);
      } else {
        ctrl.setValidators([]);
      }

      ctrl.updateValueAndValidity();
    });
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

  private loadData() {
    this.authorizationReader.getAuthorizationControls(this.codigoCotizacion, 0)
      .subscribe(response => {
        if (!response) { return; }
        this.lastModifyUser = response.lastModifyUser;
        this.lastRoleModifyUser = response.lastRoleModifyUser;
        this.authorizations = this.authorizationWriter.sortAuthorizations(response.authorizations);
        this.authorizations.forEach(element => {
          if (element.nombreAmparo != null) {
            element.nombreAmparo = element.nombreAmparo.replace("(", " ").replace(")", " ");
          } if (element.nombreSeccion != null) {
            element.nombreSeccion = element.nombreSeccion.replace("(", " ").replace(")", " ");
          }
          if (element.codigoGrupoAsegurado == 0) {
            element.nombreAmparo = null;

          }

        });
        this.users = response.users;
        this.delegationAuths = this.authorizations.filter(x => x.codigoTipoAutorizacion === 1);
        this.delegationUsers = this.users.filter(x => !x.activo);
        ;
        /*this.delegationUsers.forEach(x => {
          x.nombreRol = this.getRole(x.codigoRol).name;
        });*/
        this.buildAuthorizationComments();
        this.loadCotizacion();
      });
  }

  private loadCotizacion() {
    this.authorizationReader.getCotizacion(this.codigoCotizacion)
      .subscribe(res => {
        this.cotizacion = res.data;
        this.cotizacionDataService.cotizacion = this.cotizacion;
        this.version = res.data.version;
        this.numeroCotizacion = res.data.numero;

        this.updateForm();
      });

  }

  private getRole(codigoRol: number): UserRole {
    return this.ROLES.find(x => x.id === codigoRol);
  }

  private updateForm() {
    const gastosCompania = this.cotizacionDataService.cotizacion.informacionNegocio.gastosCompania;
    const utilidadCompania = this.cotizacionDataService.cotizacion.informacionNegocio.utilidadesCompania;
    this.autorizacionForm.patchValue({
      gastosCompania,
      utilidadCompania
    });

    this.showTasaSelector = this.cotizacionDataService.hasMultiplesTasas;
    this.notifiedUser = this.cotizacionDataService.cotizacion.usuarioNotificado;

    if (localStorage.getItem('initialgC') == null || localStorage.getItem('initialuC') == null) {
      localStorage.setItem('initialgC', gastosCompania.toString());
      localStorage.setItem('initialuC', utilidadCompania.toString());
    }
  }

  applyChanges() {
    const formData = this.autorizacionForm.getRawValue();
    if (formData.gastosCompania !== localStorage.getItem('initialgC') || formData.utilidadCompania !== localStorage.getItem('initialuC')) {
      const message = 'Esta opción recalculará la cotización con los nuevos valores de Gastos y/o Utilidad, si da \'Aceptar\' favor ' +
        'verificar el Resumen y la ficha técnica, luego continúe con las opciones de Autorizar, NO Autorizar o Devolver';
      const dialogRef = this.notificacionService.showConfirm(message);
      dialogRef.afterClosed().subscribe((result) => {
        if (result) {
          const args: ChangesArgs = {
            codigoCotizacion: this.cotizacionDataService.cotizacion.codigoCotizacion,
            version: this.cotizacionDataService.cotizacion.version,
            gastosCompania: formData.gastosCompania,
            utilidadesCompania: formData.utilidadCompania
          };
          this.autorizacionesWriterService.applyChanges(args)
            .subscribe((res) => res);
        }
      });
    } else {
      const message = 'Para poder usar la opcion \'Aplicar\' los valores de Gastos y/o Utilidad deben ser distintos a los iniciales';
      this.notificacionService.showAlert(message);
    }
  }

  resetChanges() {
    const message = 'Esta acción recalculará la cotización con los valores originales de Gastos y/o Utilidad, si da \'Aceptar\' favor ' +
      'verificar el resumen y la ficha técnica, luego continúe con las opciones de Autorizar, NO Autorizar o Devolver';
    const dialogRef = this.notificacionService.showConfirm(message);
    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.autorizacionForm.patchValue({
          gastosCompania: parseFloat(localStorage.getItem('initialgC')),
          utilidadCompania: parseFloat(localStorage.getItem('initialuC')),
        });
        const args: ChangesArgs = {
          codigoCotizacion: this.cotizacionDataService.cotizacion.codigoCotizacion,
          version: this.cotizacionDataService.cotizacion.version,
          gastosCompania: parseFloat(localStorage.getItem('initialgC')),
          utilidadesCompania: parseFloat(localStorage.getItem('initialuC'))
        };

        this.autorizacionesWriterService.applyChanges(args)
          .subscribe((res) => res);
      }
    });
  }

  private uploadFile() {
    const cotizacion = this.cotizacionDataService.cotizacion;
    this.uploadCompleted = false;
    this.showLoading = true;
    const uploadURL = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/authorizations/soportes/upload`;
    const formData = new FormData();
    const data = this.autorizacionForm.get('archivoAdjunto').value;
    formData.append('file', data);
    this.uploadService.upload2(uploadURL, formData);
  }

  private sendAutorizacion(action: AutorizacionAction) {
    const formData = this.autorizacionForm.getRawValue();
    const tasa = this.tasas[formData.tasa];
    const comments: TransactionComment[] = this.buildTransactionComments();

    const args: AutorizacionArgs = {
      codigoCotizacion: this.cotizacionDataService.cotizacion.codigoCotizacion,
      version: this.cotizacionDataService.cotizacion.version,
      transactionId: this.transactionId,
      action,
      codigoUsuarioAutorizador: '',
      userName: this.loggedUser.userName,
      authorizationResult: {
        comments,
        gastosCompania: formData.gastosCompania,
        utilidadesCompania: formData.utilidadCompania,
        tasa
      }
    };
    this.autorizacionesWriterService.authorize(args)
      .subscribe(response => {
        // Delete Local Storage After Authorize
        localStorage.removeItem('initialgC');
        localStorage.removeItem('initialuC');
        // TODO implementar accion en resultado de autorizacion
        const message = action === AutorizacionAction.Reject ?
          `La cotizacion fue rechazada y la notificación ha sido enviada exitosamente.` :
          action === AutorizacionAction.Modify ?
            'La notificación de corrección ha sido enviada exitosamente.' : 'La notificación ha sido enviada exitosamente.';
        const currentRouter = this.router;
        this.notificacionService.showToast(message);
        setTimeout(() => {
          currentRouter.navigate(['/cotizaciones']);
        }, 2000);
      });
  }

  private buildTransactionComments(): TransactionComment[] {
    const result: TransactionComment[] = [];
    // agregar comentario de usuario asignado o quien toma la autorizacion de la cotizacion
    const message = this.autorizacionForm.get('observaciones').value;
    result.push({
      codigoUsuario: '',
      codigoRolAutorizacion: '',
      codigoTipoAutorizacion: 0,
      message
    });
    const authorizationCommentsArray = this.autorizacionForm.get('authorizationComments') as FormArray;
    const controls = authorizationCommentsArray.controls;
    controls.forEach((group: FormGroup) => {
      const rawValue = group.getRawValue();
      result.push({
        codigoUsuario: rawValue.codigo,
        codigoRolAutorizacion: rawValue.codigoRol,
        codigoTipoAutorizacion: rawValue.codigoTipoAutorizacion,
        message: rawValue.observaciones
      });
    });

    return result;
  }

  private confirmAction(action: AutorizacionAction, message: string) {
    this.submitted = true;
    const isValidForm = this.autorizacionForm.valid;
    this.validateText();
    if (isValidForm) {
      const dialogRef = this.notificacionService.showConfirm(message);
      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          this.sendAutorizacion(action);
        }
      });
    }
  }

  openHistory() {
    const dialogRef = this.dialog.open(AuthorizationTransactionsModalComponent, {
      width: '400px',
      data: {
        codigoCotizacion: this.codigoCotizacion,
        version: 0
      }
    });

    return dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // TODO
      }
    });
  }

  hideHistory(e) {
    this.showHistory = false;
  }

  onBlurHistoryContainer(e) {
    if (this.showHistory) {
      this.hideHistory(e);
    }
  }

  onFileChange(event) {
    if (event.target.files.length > 0) {
      const file = event.target.files[0];
      this.fileName = file.name;
      this.autorizacionForm.get('archivoAdjunto').setValue(file);

      this.uploadFile();
    }
  }

  sendBack() {
    const ctrl = this.autorizacionForm.get('archivoAdjunto');
    ctrl.setErrors(null);
    const action = AutorizacionAction.Modify;
    const message = `Se enviara una notificación a ${this.lastModifyUser} - ${this.lastRoleModifyUser} para ajustar la información de la ` +
      `configuración de la Cotización. ¿Confirma la notificación de Ajustes?`;
    this.confirmAction(action, message);
  }

  reject() {
    const action = AutorizacionAction.Reject;
    const dialogRef = this.dialog.open(RechazoAutorizacionCotizacionComponent, {
      data: {
        user: this.lastModifyUser,
        userRole: this.lastRoleModifyUser
      }
    });

    return dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.sendAutorizacion(action);
      }
    });
  }
  validateText() {
    var i = 0;
    var control;
    if (this.submitted == undefined) {
      control = true;
    }
    this.authorizationComments.forEach(element => {
      if (element.value.observaciones == "" && this.submitted) {
        $("#controlsInd" + i).removeClass("hideElement");
      } else {
        $("#controlsInd" + i).addClass("hideElement");
      }
      i++;
    });
  }
  accept() {


    const action = AutorizacionAction.Accept;
    const message = `Se enviara una notificación de aprobación de la Cotización a ${this.lastModifyUser} - ${this.lastRoleModifyUser}. ` +
      `¿Confirma la Autorización de la cotización?`;
    this.confirmAction(action, message);
  }
}
