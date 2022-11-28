import { CotizacionPersistenceService } from './../../services/cotizacion-persistence.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PageToolbarConfig } from 'src/app/models/page-toolbar-item';
import { CotizacionWriterService } from 'src/app/services/cotizacion-writer.service';
import { PageToolbarBuilder } from 'src/app/shared/services/page-toolbar-builder';
import { Cotizacion, CotizacionState } from '../../models';
import { CotizacionReaderService } from '../../services/cotizacion-reader.service';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
  selector: 'app-consulta-cotizacion',
  templateUrl: './consulta-cotizacion.component.html',
  styleUrls: ['./consulta-cotizacion.component.scss']
})
export class ConsultaCotizacionComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private toolbarBuilder: PageToolbarBuilder,
    private cotizacionDataService: CotizacionPersistenceService,
    private cotizacionWriterService: CotizacionWriterService,
    private cotizacionReaderService: CotizacionReaderService,
    private authenticationService: AuthenticationService) { }

  codigoCotizacion: number;
  version: number;
  numeroCotizacion: string;
  model: Cotizacion;
  pageLoaded = false;
  toolbarConfig: PageToolbarConfig;
  readonly = false;

  ngOnInit() {
    this.route.data.subscribe(res => {
      this.readonly = res.data.readonly;
      this.codigoCotizacion = res.data.codigoCotizacion;
      this.version = res.data.version;
      this.numeroCotizacion = res.data.numero;


      this.model = res.data;
      this.pageLoaded = true;
    });

    this.initializeToolbar();
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
        isEnabled:
          this.cotizacionDataService.cotizacion.estado ===
            CotizacionState.PendingAuthorization &&
          this.cotizacionDataService.cotizacion.usuarioNotificado ===
            this.authenticationService.currentUserValue.userName
            ? true
            : false,
        onClick: () => {
          this.router.navigate([ '/cotizaciones', this.codigoCotizacion, 'authorize', ]);
        },
      },
    ];

    this.toolbarConfig = this.toolbarBuilder.build(items);
  }

  private navigateToCotizaciones() {
    this.router.navigate(['cotizaciones']);
  }

  triggerUnlockCotizacion() {
    this.cotizacionWriterService.unlockCotizacion(this.codigoCotizacion, this.version)
      .subscribe(res => res);
  }

  reload(args) {
    this.refresh();
  }

  refresh() {
    this.pageLoaded = false;
    this.cotizacionReaderService.consultarCotizacion(this.codigoCotizacion, this.version)
      .subscribe(res => {
        this.readonly = res.readonly;
        this.model = res.data;
        this.refreshCotizacionDataServiceCotizacion(res.data);
        this.pageLoaded = true;
      });
  }

  private refreshCotizacionDataServiceCotizacion(data: any) {
    this.cotizacionDataService.cotizacion.blocked = data.blocked;
    this.cotizacionDataService.cotizacion.codigoCotizacion = data.codigoCotizacion;
    this.cotizacionDataService.cotizacion.estado = data.estado;
    this.cotizacionDataService.cotizacion.gastosCompania = data.gastosCompania;
    this.cotizacionDataService.cotizacion.informacionBasica = data.informacionBasica;
    this.cotizacionDataService.cotizacion.informacionBasicaTomador = data.informacionBasicaTomador;
    this.cotizacionDataService.cotizacion.informacionGruposAsegurados = data.informacionGruposAsegurados;
    this.cotizacionDataService.cotizacion.informacionIntermediarios = data.informacionIntermediarios;
    this.cotizacionDataService.cotizacion.informacionNegocio = data.informacionNegocio;
    this.cotizacionDataService.cotizacion.informacionSiniestralidad = data.informacionSiniestralidad;
    this.cotizacionDataService.cotizacion.lastAuthorId = data.lastAuthorId;
    this.cotizacionDataService.cotizacion.lastAuthorName = data.lastAuthorName;
    this.cotizacionDataService.cotizacion.numero = data.numero;
    this.cotizacionDataService.cotizacion.user = data.user;
    this.cotizacionDataService.cotizacion.usuarioNotificado = data.usuarioNotificado;
    this.cotizacionDataService.cotizacion.version =  data.version;
  }

  updateInitialValuesSF(event: any) {
    this.refresh();
  }
}
