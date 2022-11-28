import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PageToolbarBuilder } from 'src/app/shared/services/page-toolbar-builder';
import { Cotizacion } from '../../models/cotizacion';
import { AbandonarCotizacionDialogService } from '../../services/abandonar-cotizacion-dialog.service';
import { CotizacionPersistenceService } from '../../services/cotizacion-persistence.service';
import { PageToolbarConfig } from 'src/app/models/page-toolbar-item';
import {CotizacionTabsComponent} from 'src/app/shared/components/cotizacion-tabs/cotizacion-tabs.component'
@Component({
  selector: 'app-creacion-cotizacion',
  templateUrl: './creacion-cotizacion.component.html',
  styleUrls: ['./creacion-cotizacion.component.scss']
})
export class CreacionCotizacionComponent implements OnInit {
  constructor(
    private router: Router,
    private _abandonarCotizacionDialogService: AbandonarCotizacionDialogService,
    private toolbarBuilder: PageToolbarBuilder,
    private cotizacionDataService: CotizacionPersistenceService) { }
  tabsService: CotizacionTabsComponent;
  model: Cotizacion;
  toolbarConfig: PageToolbarConfig;
  tabs: any

  get abandonarCotizacionDialogService() {
    return this._abandonarCotizacionDialogService;
  }

  ngOnInit() {
    this.reset();
    
   //this.tabs = this.tabsService.buildTabs(0,0);
   
    this.model = new Cotizacion();
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
    }];

    this.toolbarConfig = this.toolbarBuilder.build(items);
  }

  navigateToCotizaciones() {
    this.router.navigate(['cotizaciones']);
  }

  reset() {
    this.cotizacionDataService.reset();
  }
}
