import { AuthenticationService } from 'src/app/services/authentication.service';
import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApplicationTab } from 'src/app/models/application-tab';
import { CotizacionState } from 'src/app/models';
import { NotificationService } from '../../services/notification.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-cotizacion-tabs',
  templateUrl: './cotizacion-tabs.component.html',
  styleUrls: ['./cotizacion-tabs.component.scss']
})
export class CotizacionTabsComponent implements OnInit {
  codigoCotizacion: number;
  version: number;
  navigationTabIndex: number;
  readonly: boolean;
  cotizacionState: CotizacionState;
  usuarioNotificado: string;
  @Input() disabledTab$: Observable<any>;

  constructor(
    private route: ActivatedRoute,
    private authenticationService: AuthenticationService,
    private notificationService: NotificationService,
    private router: Router) { }

  tabs: ApplicationTab[] = [{
    index: 0,
    label: 'Configuración Cotización',
    codigoEstado: CotizacionState.OnSiniestralidad, // codigoestado maximo de configuracion de cotizacion OnSiniestralidad
    active: false,
    disabled: false
  }, {
    index: 1,
    label: 'Resumen',
    codigoEstado: CotizacionState.OnResumen,
    active: false,
    disabled: false
  }, {
    index: 2,
    label: 'Ficha Técnica',
    codigoEstado: CotizacionState.OnFichaTecnica,
    active: false,
    disabled: false
  }, {
    index: 3,
    label: 'Configuración Slip',
    codigoEstado: CotizacionState.OnSlipConfiguration,
    active: false,
    disabled: false
  }, {
    index: 4,
    label: 'Slip',
    codigoEstado: CotizacionState.OnSlip,
    active: false,
    disabled: false
  }];

  public clearSelection() {
    this.tabs.forEach(x => x.active = false);
  }

  public buildTabs(navigationTabIndex: number, state: number) {
    state == undefined ? state = 0 : state = state;
    this.navigationTabIndex = navigationTabIndex;
    this.clearSelection();
    for (let i = 0; i < this.tabs.length; i++) {
      const tab = this.tabs[i];

      tab.active = navigationTabIndex === i + 1;
      tab.disabled = state < tab.codigoEstado;
      if (tab.active || (i === 0 && state === 0)) {
        tab.disabled = false;
      }

      const isAuthenticationUser = this.usuarioNotificado === this.authenticationService.currentUserValue.userName;
      const isPendingAuthorization = this.cotizacionState === CotizacionState.PendingAuthorization;
      const tabRule = isAuthenticationUser && isPendingAuthorization;
      if (tabRule && (tab.index === 4)) {
        tab.disabled = true;
      }
    }

    return this.tabs;
  }

  ngOnInit() {
    this.route.data.subscribe(res => {
      
      this.codigoCotizacion = res.data.codigoCotizacion;
      this.version = res.data.version;
      this.readonly = res.data.readonly;
      this.cotizacionState = res.data.estado;
      this.usuarioNotificado = res.data.usuarioNotificado;
      if (this.version == 777) {
        this.tabs = this.buildTabs(res.navigationTabIndex, 1);
      } else {
        this.tabs = this.buildTabs(res.navigationTabIndex, res.data.estado);
      }

      if (this.disabledTab$) {
        this.disabledTab$.subscribe(r => {
          if (r && r != null) {
            const indexTab = this.tabs.findIndex((t => t.index === r.tabId));
            this.tabs[indexTab].disabled = r.disabled;
            this.tabs[indexTab].active = false;
            this.cotizacionState = r.state;
          }
        });
      }
    });
  }

  navigate(event: Event, index: number) {
    
    const extras = {};
    // const extras = {
    //   queryParams: {
    //     version: this.version
    //   }
    // };

    if (this.navigationTabIndex === index + 1) { return; }
    if (this.codigoCotizacion) {
      const tab = this.tabs[index];
      if (tab.disabled == false) {
        this.clearSelection();

        switch (index) {
          // consultar cotización
          case 0:
            this.router.navigate(['/cotizaciones', this.codigoCotizacion]);
            break;
          // ver resumen de cotización
          case 1:
            this.router.navigate(['/cotizaciones', this.codigoCotizacion, 'resumen'], extras);
            break;
          // ver ficha técnica de cotización
          case 2:
            this.router.navigate(['/cotizaciones', this.codigoCotizacion, 'fichatecnica'], extras);
            break;
          // ver configuración de slip
          case 3:
            this.router.navigate(['/cotizaciones', this.codigoCotizacion, 'slip', 'config'], extras);
            break;
          // ver preview de slip
          case 4:
            if (this.cotizacionState >= CotizacionState.OnSlip) {
              this.router.navigate(['/cotizaciones', this.codigoCotizacion, 'slip', 'preview'], extras);
              break;
            } else {
              const message = 'Recuerda primero llegar a la Configuración del Slip para validar los cambios realizados y' +
                ' poder visualizar el Slip.';
              this.notificationService.showAlert(message);
              break;
            }
          // this.router.navigate(['/cotizaciones', this.codigoCotizacion, 'slip', 'preview'], extras);
          default:
            this.router.navigate(['/cotizaciones/nueva']);
            break;
        }
      }
    }
  }

  get tabWidth() {
    const w = 100 / this.tabs.length;
    return `${w}%`;
  }
}
