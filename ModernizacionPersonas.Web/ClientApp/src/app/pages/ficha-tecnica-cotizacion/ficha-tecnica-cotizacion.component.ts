import { CotizacionWriterService } from 'src/app/services/cotizacion-writer.service';
import { Component, OnInit } from '@angular/core';
import { FichaTecnica, GenerarFichaTecnicaResponse } from 'src/app/models/fichatecnica';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { FichatecnicaCotizacionReaderService } from 'src/app/services/fichatecnica-cotizacion-reader.service';
import { PageToolbarBuilder } from 'src/app/shared/services/page-toolbar-builder';
import { PageToolbarConfig } from 'src/app/models/page-toolbar-item';
import { CotizacionTabsComponent } from 'src/app/shared/components/cotizacion-tabs/cotizacion-tabs.component';
import { throttleTime } from 'rxjs/operators';

@Component({
  selector: 'app-ficha-tecnica-cotizacion',
  templateUrl: './ficha-tecnica-cotizacion.component.html',
  styleUrls: ['./ficha-tecnica-cotizacion.component.scss']
})
export class FichaTecnicaCotizacionComponent implements OnInit {
  codigoCotizacion: number;
  version: number;
  versionor: number;
  numeroCotizacion: string;
  versionFTecAlterna: string;

  model: FichaTecnica;
  mostrarPerfilEdades: boolean;
  mostrarPerfilValores: boolean;
  toolbarConfig: PageToolbarConfig;
  readonly: boolean;
    urlTree: any;

  constructor(private route: ActivatedRoute,
    private router: Router,
    private toolbarBuilder: PageToolbarBuilder,
    private cotizacionDataService: CotizacionPersistenceService,
    private cotizacionWriterService: CotizacionWriterService,
    private fichaTecnicaService: FichatecnicaCotizacionReaderService) {
    this.urlTree = this.router.parseUrl(this.router.url);
    this.versionor = this.urlTree.queryParams['versionor'];}

  pageLoaded = false;

  get cotizacion() { return this.cotizacionDataService.cotizacion; }

  ngOnInit() {
	  
	  this.route.data.subscribe(res => {
      this.codigoCotizacion = res.data.codigoCotizacion;
      this.version = res.data.version;
	  
      this.numeroCotizacion = res.data.numero;

      this.readonly = res.data.readonly;

      this.cotizacionDataService.setCotizacionState(res.data.estado);

      this.consultarFichaTecnica();
    });

    this.initializeToolbar();
  }

  initializeToolbar() {
    const items = [{
      name: 'back',
      icon_path: 'home',
      label: '',
      tooltip: 'Volver a cotizaciones',
      isEnabled: true,
      fixed: true,
      onClick: () => {
        this.navigateToCotizaciones();
      }
    }, {
      name: 'refresh',
      icon_path: 'refresh',
      label: '',
      tooltip: 'Refrescar',
      isEnabled: true,
      fixed: true,
      onClick: () => {
        this.refresh();
      }
    }];

    this.toolbarConfig = this.toolbarBuilder.build(items);
  }

  navigateToCotizaciones() {
    this.router.navigate(['cotizaciones']);
  }

  triggerUnlockCotizacion() {
    this.cotizacionWriterService.unlockCotizacion(this.codigoCotizacion, this.version)
      .subscribe(res => res);
  }

  private consultarFichaTecnica() {
    this.pageLoaded = false;
    this.fichaTecnicaService.consultarFichaTecnica(this.codigoCotizacion, this.version)
      .subscribe((response: GenerarFichaTecnicaResponse) => {
        
        this.model = response.data;
        this.model.directorComercialInfo.nombre = this.model.directorComercialInfo.nombre == "SIN SELECCIONAR" ? "" : this.model.directorComercialInfo.nombre;
        this.cotizacionDataService.setCotizacionState(response.cotizacionState);
        this.mostrarPerfilEdades = this.model.perfilEdades.rangos.length > 0;
        this.mostrarPerfilValores = this.model.perfilValores.rangos.length > 0;

        this.pageLoaded = true;
        this.versionFTecAlterna = this.route.snapshot.paramMap.get('version');
      });
  }

  continue() {
    this.router.navigate(['cotizaciones', this.codigoCotizacion, 'slip', 'config']);
  }

  reload(args) {
    this.refresh();
  }

  refresh() {
    this.consultarFichaTecnica();
  }
}
