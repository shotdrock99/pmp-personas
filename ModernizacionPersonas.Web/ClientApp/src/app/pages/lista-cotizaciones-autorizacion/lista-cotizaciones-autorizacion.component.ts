import { NotificationService } from './../../shared/services/notification.service';
import { PageToolbarBuilder } from 'src/app/shared/services/page-toolbar-builder';
import { Component, OnInit } from '@angular/core';
import { AutorizacionesReaderService } from 'src/app/services/autorizaciones-reader.service';
import { BreakpointObserver } from '@angular/cdk/layout';
import { MatTableDataSource } from '@angular/material/table';
import { environment } from 'src/environments/environment';
import { CotizacionAutorizacionItemList } from 'src/app/models/cotizacion-autorizacion-item-list';
import { PageToolbarConfig, PageToolbarItem } from 'src/app/models/page-toolbar-item';
import { CotizacionFilter } from 'src/app/models/cotizacion-filter';
import * as moment from 'moment';

@Component({
  selector: 'app-lista-cotizaciones-autorizacion',
  templateUrl: './lista-cotizaciones-autorizacion.component.html',
  styleUrls: ['./lista-cotizaciones-autorizacion.component.scss']
})
export class ListaCotizacionesAutorizacionComponent implements OnInit {
  filterArgs: CotizacionFilter;
  data: any;

  constructor(
    private autorizacionesReaderService: AutorizacionesReaderService,
    private breakpointObserver: BreakpointObserver,
    private toolbarBuilder: PageToolbarBuilder,
    private notificationService: NotificationService
  ) {
    this.breakpointObserver.observe(['(max-width: 600px)']).subscribe(result => {
      this.displayedColumns = result.matches ?
        ['numeroCotizacion', 'sucursal', 'ramo', 'tomador', 'ultimoUsuario', 'usrNotificado', 'fechaModificacion', 'opciones'] :
        ['numeroCotizacion', 'sucursal', 'ramo', 'tomador', 'ultimoUsuario', 'usrNotificado', 'fechaModificacion', 'opciones'];
    });
  }

  showFilter: boolean;
  toolbarConfig: PageToolbarConfig;
  dataSource: MatTableDataSource<any>;
  displayedColumns: string[] = ['numeroCotizacion', 'sucursal', 'ramo', 'tomador', 'ultimoUsuario', 'usrNotificado', 'fechaModificacion',
  'opciones'];
  itemsCount = 0;
  isLoading = true;

  ngOnInit() {
    this.dataSource = new MatTableDataSource<any>([]);
    this.data = {
      calledBy: 'Autorizaciones'
    };
    this.loadData();
  }

  private loadData(filterArgs: CotizacionFilter = new CotizacionFilter()) {
    if (filterArgs.fechaDesde) {
      const fechaDesde = new Date(filterArgs.fechaDesde);
      const fds = moment(fechaDesde).locale('es').format('YYYY/MM/DD');
      filterArgs.fechaDesde = fds;
    }
    if (filterArgs.fechaHasta) {
      const fechaHasta = new Date(filterArgs.fechaHasta);
      const fhs = moment(fechaHasta).locale('es').format('YYYY/MM/DD');
      filterArgs.fechaHasta = fhs;
    }
    this.filterArgs = filterArgs;
    this.autorizacionesReaderService.consultarCotizaciones(filterArgs)
      .subscribe(response => {
        this.dataSource.data = response;
        this.isLoading = false;
        this.itemsCount = this.dataSource.data.length;
      });

    this.initializeToolbar();
  }

  initializeToolbar() {
    const items: PageToolbarItem[] = [{
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

  refresh() {
    this.loadData(this.filterArgs);
  }

  onFilterChange(filterArgs: CotizacionFilter) {
    this.loadData(filterArgs);
  }

  openCotizacion(e, args: CotizacionAutorizacionItemList) {
    this.notificationService.showToast('Abriendo cotización');
    const codigoCotizacion = args.codigoCotizacion;
    const url = `${environment.appSettings.BASEHREF}/cotizaciones/${codigoCotizacion}`;
    window.open(url, '_blank');
  }
}
