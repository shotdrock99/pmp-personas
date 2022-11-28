import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Cotizacion, CotizacionState } from 'src/app/models';
import { PageToolbarConfig } from 'src/app/models/page-toolbar-item';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AutorizacionesReaderService } from 'src/app/services/autorizaciones-reader.service';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { CotizacionReaderService } from 'src/app/services/cotizacion-reader.service';
import { PageToolbarBuilder } from 'src/app/shared/services/page-toolbar-builder';
import { environment } from 'src/environments/environment';

import { ActionResponseBase } from './../../../models/action-response.base';
import { CotizacionTransaction } from './../../../models/cotizacion-transaction';

@Component({
  selector: 'app-cotizacion-history-timeline',
  templateUrl: './cotizacion-history-timeline.component.html',
  styleUrls: ['./cotizacion-history-timeline.component.scss']
})
export class CotizacionHistoryTimelineComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private toolbarBuilder: PageToolbarBuilder,
    private cotizacionReader: CotizacionReaderService,
    private cotizacionDataPersistence: CotizacionPersistenceService,
    private authenticationService: AuthenticationService,
    private authorizationReaderService: AutorizacionesReaderService) { }

  URL_BASE = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;
  codigoCotizacion: number;
  cotizacionState: number;
  numeroCotizacion: string;
  data: CotizacionTransaction[];
  selectedItem: CotizacionTransaction;
  pageLoaded = false;
  hasUNotificado = false;
  uNotificado: string;
  toolbarConfig: PageToolbarConfig;
  cotizacion: Cotizacion;
  autorizacionUsers: any[];
  isUserAllowedToViewAuthorization: boolean;

  ngOnInit() {
    const param0 = this.route.snapshot.paramMap.get('cod_cotiza');
    this.codigoCotizacion = Number(param0);
    this.cotizacion = this.cotizacionDataPersistence.cotizacion;
    this.loadData();
  }

  private loadData() {
    this.pageLoaded = false;
    this.cotizacionReader.getTransactions(this.codigoCotizacion, 0)
      .subscribe((res: GetTransactionsResponse) => {
        this.numeroCotizacion = res.numeroCotizacion;
        this.data = res.transactions;
        this.cotizacionState = res.cotizacionState;
        this.pageLoaded = true;
        if(this.data[0].version > 1){
          this.data.forEach(element => {
            if(element.description == "VERSION"){
              element.description = "VERSION " + element.version;
            }

          });
        }
      });

    this.getAutorizacionUser();
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
      {
        name: 'refresh',
        icon_path: 'refresh',
        label: '',
        tooltip: 'Refrescar',
        isEnabled: true,
        fixed: true,
        onClick: () => {
          this.refresh();
        },
      },
      {
        name: 'backToAuthorization',
        icon_path: 'first_page',
        label: 'Abrir Autorización',
        tooltip: 'Devolver a autorización',
        // isEnabled:
        //   this.cotizacionDataPersistence.cotizacion.estado ===
        //     CotizacionState.PendingAuthorization &&
        //   this.cotizacionDataPersistence.cotizacion.usuarioNotificado ===
        //     this.authenticationService.currentUserValue.userName
        //     ? true
        //     : false,
        isEnabled:
          this.cotizacionDataPersistence.cotizacion.estado ===
            CotizacionState.PendingAuthorization &&
          this.isUserAllowedToViewAuthorization,
        onClick: () => {
          this.router.navigate([
            '/cotizaciones',
            this.codigoCotizacion,
            'authorize',
          ]);
        },
      },
    ];

    this.toolbarConfig = this.toolbarBuilder.build(items);
  }

  navigateToCotizaciones() {
    this.router.navigate(['cotizaciones']);
  }

  navigateToSlip() {

  }

  downloadAttachments(item: CotizacionTransaction) {
    const downloadUrl = `${this.URL_BASE}/${this.codigoCotizacion}/authorizations/soportes/${item.codigoTransaccion}/download?version=0`;
    const link = document.createElement('a');
    link.href = downloadUrl;
    document.body.appendChild(link);
    link.click();
  }

  private getAutorizacionUser() {
    if(this.cotizacionDataPersistence.cotizacion != undefined){
      this.authorizationReaderService
      .getAuthorizationUsers(this.codigoCotizacion, this.cotizacionDataPersistence.cotizacion.version)
      .subscribe((response) => {
        this.autorizacionUsers = response;
        this.isUserAllowedToViewAuthorization = this.autorizacionUsers.some(
          (x) =>
            x.codigo === this.authenticationService.currentUserValue.userName
        );
        this.initializeToolbar();
      });
    }
  }

  refresh() {
    this.pageLoaded = false;
    this.loadData();
  }
}



export class GetTransactionsResponse extends ActionResponseBase {
  public transactions: CotizacionTransaction[];
}
