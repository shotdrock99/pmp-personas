import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { ContinuarCotizacionComponent } from 'src/app/pages/components/continuar-cotizacion/continuar-cotizacion.component';
import { MatDialog } from '@angular/material/dialog';
import { NotificationService } from '../../services/notification.service';
import { Cotizacion, CotizacionState } from 'src/app/models';
import { CotizacionWriterService } from 'src/app/services/cotizacion-writer.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { Router } from '@angular/router';
import { ApplicationUser } from 'src/app/models/application-user';

@Component({
  selector: 'app-readonly-cotizacion-toast',
  templateUrl: './readonly-cotizacion-toast.component.html',
  styleUrls: ['./readonly-cotizacion-toast.component.scss'],
})
export class ReadonlyCotizacionToastComponent implements OnInit {
  constructor(
    private router: Router,
    private dialog: MatDialog,
    private notificationService: NotificationService,
    private authenticationService: AuthenticationService,
    private cotizacionWriterService: CotizacionWriterService
  ) { }

  @Output() reload: EventEmitter<any> = new EventEmitter();
  @Input() readonly: boolean;
  @Input() estadoCotizacion: CotizacionState;
  @Input() data: Cotizacion;

  alertClass = 'alert-warning';
  lastModifyUser: string;
  lastModifyUserSucursal: number;
  cotizacionSucursal: number;
  cotizacionZona: number;
  loggedUser: ApplicationUser;
  dtzUser: boolean;
  message = '';
  allowRequest = false;

  ngOnInit() {
    this.loggedUser = this.authenticationService.currentUserValue;
    this.estadoCotizacion = this.data.estado;
    this.cotizacionSucursal = this.data.informacionBasica.sucursal.codigoSucursal;
    this.cotizacionZona = this.data.informacionBasica.sucursal.codigoZona;
    this.dtzUser = this.loggedUser.rol.roleName.toLowerCase().includes('zonal');
    if (this.data.user) {
      this.lastModifyUser = this.data.user.externalInfo.loginUsuario;
      this.lastModifyUserSucursal = this.data.user.externalInfo.sucursal;
    }
    if (
      (this.cotizacionSucursal === this.loggedUser.externalInfo.sucursal &&
        this.data.estado < CotizacionState.Sent &&
        this.data.estado !== CotizacionState.PendingAuthorization) ||
      (this.loggedUser.externalInfo.sucursal === 800 &&
        this.data.estado < CotizacionState.Sent &&
        this.data.estado !== CotizacionState.PendingAuthorization) ||
      (this.dtzUser &&
        this.loggedUser.externalInfo.zona === this.cotizacionZona &&
        this.data.estado < CotizacionState.Sent &&
        this.data.estado !== CotizacionState.PendingAuthorization)
    ) {
      this.allowRequest = true;
    }
    if (this.estadoCotizacion === CotizacionState.PendingAuthorization) {
      this.message = `La cotización esta bloqueada por <b>${this.data.usuarioNotificado}</b> y esta en estado <b>Pendiente por` +
        ` Autorización</b> y puede ser consultada en modo de solo lectura.`;
    } else if (this.estadoCotizacion === CotizacionState.Accepted) {
      this.alertClass = 'alert-success';
      this.message = `La cotización esta en estado <b>Aceptada</b> y puede ser consultada en modo de solo lectura.`;
    } else if (this.estadoCotizacion === CotizacionState.Sent) {
      this.alertClass = 'alert-info';
      this.message = `La cotización esta en estado <b>Enviada</b> y puede ser consultada en modo de solo lectura.`;
    } else if (this.estadoCotizacion === CotizacionState.RejectedByCompany) {
      this.alertClass = 'alert-danger';
      this.message = `La cotización esta en estado <b>Rechazada por la Compañía</b> y solo puede ser consultada en modo de solo lectura.`;
    } else if (this.estadoCotizacion === CotizacionState.RejectedByClient) {
      this.alertClass = 'alert-danger';
      this.message = `La cotización esta en estado <b>Rechazada por el Cliente</b> y solo puede ser consultada en modo de solo lectura.`;
    } else if (this.estadoCotizacion === CotizacionState.Closed) {
      this.alertClass = 'alert-info';
      this.message = `La cotización esta en estado <b>Cerrada</b> y solo puede ser consultada en modo de solo lectura.`;
    } else if (this.estadoCotizacion === CotizacionState.Expired) {
      this.alertClass = 'alert-info';
      this.message = `La cotización esta en estado <b>Cerrada/Vencida</b> y solo puede ser consultada en modo de solo lectura.`;
    } else if (this.estadoCotizacion === CotizacionState.Issued) {
      this.alertClass = 'alert-success';
      this.message = `La cotización esta en estado <b>Expedida</b> y puede ser consultada en modo de solo lectura.`;
    } else if (this.estadoCotizacion === CotizacionState.ExpeditionRequest) {
      this.alertClass = 'alert-info';
      this.message = `La cotización esta en estado <b>Solicitud Expedición</b> y puede ser consultada en modo de solo lectura.`;
    } else {
      this.message = `La cotización esta bloqueada por <b>${this.lastModifyUser}</b> y puede ser consultada en modo de solo lectura.`;
    }
  }

  requireContinueCotizacion() {
    // Validate locked
    if (this.data.blocked) {
      const messsage = `La cotización está abierta por <b>${this.lastModifyUser}</b> y no puede ser requerida en este momento.`;
      this.notificationService.showAlert(messsage);
      return;
    }
    const dialogRef = this.dialog.open(ContinuarCotizacionComponent, {
      maxWidth: '600px',
      data: {
        codigoCotizacion: this.data.codigoCotizacion,
        version: this.data.version,
        numeroCotizacion: this.data.numero,
        lastModifyUser: this.lastModifyUser,
      },
    });
    return dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.cotizacionWriterService
          .continueCotizacion(this.data.codigoCotizacion, result.observaciones)
          .subscribe((res) => {
            if (!res) { return; }
            // update last author on view
            const lastAuthor = this.authenticationService.currentUserValue
              .userName;
            this.navigateCotizacion(this.data.codigoCotizacion);
          });
      }
    });
  }

  navigateCotizacion(codigoCotizacion: number) {
    this.notificationService.showToast('Abriendo cotización');
    // this.router.navigate(['/cotizaciones', codigoCotizacion]);
    this.reload.emit({ event: 'reload' });
  }
}
