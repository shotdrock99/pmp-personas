import { CausalesReaderService } from './../../services/causales-reader.service';
import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { CotizacionFilter } from 'src/app/models/cotizacion-filter';
import { CotizacionItemList } from 'src/app/models/cotizacion-item-list';
import { PageToolbarConfig } from 'src/app/models/page-toolbar-item';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { CotizacionReaderService } from 'src/app/services/cotizacion-reader.service';
import { ConfirmacionCotizacion, CotizacionWriterService } from 'src/app/services/cotizacion-writer.service';
import { AlertDialogComponent, AlertDialogModel } from 'src/app/shared/components/alert-dialog';
import { CerrarCotizacionDialogComponent } from '../components/cerrar-cotizacion/cerrar-cotizacion.component';
import { CotizacionState, Cotizacion } from './../../models/cotizacion';
import { PageToolbarItem } from './../../models/page-toolbar-item';
import { NotificationService } from './../../shared/services/notification.service';
import { PageToolbarBuilder } from './../../shared/services/page-toolbar-builder';
import { AceptacionCotizacionDialogComponent } from './../components/aceptacion-cotizacion/aceptacion-cotizacion.component';
import { ContinuarCotizacionComponent } from './../components/continuar-cotizacion/continuar-cotizacion.component';
import * as moment from 'moment';
import { ApplicationUser } from 'src/app/models/application-user';
import { GenerateExpedicionWebComponent } from '../components/generate-expedicion-web/generate-expedicion-web.component';
import { AlertDialogPreventCloseComponent } from 'src/app/shared/components/alert-dialog/alert-dialog.preventClose.component';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { RolesUsuarioReaderService } from "src/app/services/roles-usuario.reader.service";
import { Permiso, Rol } from "src/app/models/rol";

@Component({
  selector: 'app-lista-cotizaciones',
  templateUrl: './lista-cotizaciones.component.html',
  styleUrls: ['./lista-cotizaciones.component.scss'],
})
export class ListaCotizacionesComponent implements OnInit {
  @ViewChild(MatPaginator)
  paginator: MatPaginator;

  @ViewChild(MatSort)
  sort: MatSort;

  @Output() selectionChange: EventEmitter<any> = new EventEmitter();

  data: CotizacionItemList[];
  itemsCount = 0;
  displayedColumns: string[] = [
    'selected',
    'numeroCotizacion',
    // 'version',
    // 'zona',
    'sucursal',
    'ramo',
    // 'subramo',
    'tomador',
    'estado',
    'ultimoUsuario',
    'usuarioNotificado',
    'fechaModificacion',
  ];
  dataSource: MatTableDataSource<CotizacionItemList>;
  selection = new SelectionModel<CotizacionItemList>(false, []);
  isLoading = true;
  showFilter: boolean;
  causales: any;
  permisosRol: any;
  toolbarConfig: PageToolbarConfig;
  filterArgs: CotizacionFilter;
  currentUser: ApplicationUser = this.authenticationService.currentUserValue;

  constructor(
    private permisosReaderService: RolesUsuarioReaderService,
    private router: Router,
    private dialog: MatDialog,
    private notificationService: NotificationService,
    private toolbarBuilder: PageToolbarBuilder,
    private authenticationService: AuthenticationService,
    private cotizacionDataPersistence: CotizacionPersistenceService,
    private cotizacionReaderService: CotizacionReaderService,
    private cotizacionWriterService: CotizacionWriterService,
    private causalesReaderService: CausalesReaderService
  ) { }

  ngOnInit() {
    this.loadCotizaciones();
    this.loadCausales();
    this.initializeToolbar();
    this.loadRolpermissions();
  }

  loadCausales() {
    this.causalesReaderService
      .getCausales()
      .subscribe((res) => (this.causales = res));
  }

  initializeToolbar() {
    const items: PageToolbarItem[] = [
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
        name: 'add',
        icon_path: 'add',
        label: 'Nueva cotización',
        tooltip: 'Nueva cotización',
        isEnabled: true,
        fixed: true,
        onClick: () => {
          this.router.navigate(['/cotizaciones', 'nueva']);
        },
      },
      {
        name: 'open',
        icon_path: 'open_in_new',
        label: 'Abrir cotización',
        tooltip: 'Abrir cotización',
        onClick: (e: Event) => this.openCotizacion(e),
      },
      {
        name: 'accept',
        icon_path: 'playlist_add_check',
        label: 'Aceptar cotización',
        tooltip: 'Aceptar cotización',
        onClick: (e: Event) => this.acceptCotizacion(e),
      },
      {
        name: 'reject',
        icon_path: 'clear',
        label: 'Rechazar cotización',
        tooltip: 'Rechazar cotización',
        onClick: (e: Event) => this.rejectCotizacion(e),
      },
      {
        name: 'more',
        icon_path: 'more_horiz',
        label: '',
        tooltip: 'Más opciones',
        items: [
          {
            name: 'timeline',
            icon_path: 'timeline',
            label: 'Ver histórico',
            tooltip: 'Ver histórico',
            isEnabled: true,
            fixed: true,
            onClick: (e: Event) => this.showTimeline(),
          },
          {
            name: 'copy',
            icon_path: 'file_copy',
            label: 'Copiar',
            tooltip: 'Copiar cotización',
            onClick: (e: Event) => this.copyCotizacion(e),
          },
          {
            name: 'new_version',
            icon_path: 'note_add',
            label: 'Nueva versión',
            tooltip: 'Nueva versión',
            onClick: (e: Event) => this.createVersion(e),
          },
          {
            name: 'nav_data_sheet',
            icon_path: 'featured_play_list',
            label: 'Ver Ficha Técnica',
            tooltip: 'Ver Ficha Técnica',
            onClick: (e: Event) => this.navigateFichaTecnica(e),
          },
          {
            name: 'nav_data_sheet_alternative',
            icon_path: 'featured_play_list',
            label: 'Ver Ficha Técnica Alterna',
            tooltip: 'Ver Ficha Técnica Alterna',
            onClick: (e: Event) => this.navigateFichaTecnicaAlternativa(e),
          },
          {
            name: 'nav_slip',
            icon_path: 'description',
            label: 'Ver Slip',
            tooltip: 'Ver Slip',
            onClick: (e: Event) => this.navigateSlip(e),
          },
          {
            name: 'expedition',
            icon_path: 'receipt_long',
            label: 'Expedición Web',
            tooltip: 'Expedir',
            onClick: (e: Event) => this.generateExpedicion(e),
          },
        ],
      },
    ];

    this.toolbarConfig = this.toolbarBuilder.build(items);
  }

  refresh() {
    this.loadCotizaciones(this.filterArgs);
    this.toolbarConfig.reset();
    this.closeFilter();
  }

  closeFilter() {
    this.showFilter = false;
  }

  private loadCotizaciones(
    filterArgs: CotizacionFilter = new CotizacionFilter()
  ) {
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
    this.cotizacionReaderService.consultarCotizaciones(filterArgs)
      .subscribe((response) => {
        this.isLoading = false;
        this.data = response;
        // Assign the data to the data source for the table to render
        this.dataSource = new MatTableDataSource(this.data);
        // this.dataSource.paginator = this.paginator;
        // this.dataSource.sort = this.sort;
        this.itemsCount = this.dataSource.data.length;
      });
  }

  openCotizacion(event: Event) {
    const row: CotizacionItemList = this.selection.selected[0];
    const codigoCotizacion = row.codigoCotizacion;
    const version = row.version;

    this.navigateCotizacion(codigoCotizacion, version);
    event.stopPropagation();
  }

  canOpenCotizacion(row: CotizacionItemList) {
    let result = false;
    const lastModifyUser = row.lastAuthor;
    const loggedUser = this.authenticationService.currentUserValue;
    if (row.codigoEstado === CotizacionState.PendingAuthorization) {
      this.alertPendingAuthorizacion(row);
    } else if (row.closed) {
      this.alertCotizacionClosed(row);
    } else if (row.locked && lastModifyUser !== loggedUser.userName) {
      this.alertCotizacionLocked(row);
    } else if (row.codigoEstado === CotizacionState.Accepted) {
      this.alertCotizacionAccepted(row);
    } else if (lastModifyUser && lastModifyUser !== loggedUser.userName) {
      // si el ultimo usuario que modifico la cotizacion es diferente al usuario logeado
      // this.requireContinueCotizacion(lastModifyUser);
    } else {
      result = true;
    }

    return result;
  }

  private showTimeline() {
    const row: CotizacionItemList = this.selection.selected[0];
    this.cotizacionDataPersistence.cotizacion = new Cotizacion();
    this.cotizacionDataPersistence.cotizacion.codigoCotizacion = row.codigoCotizacion;
    this.cotizacionDataPersistence.cotizacion.version = row.version;
    this.cotizacionDataPersistence.cotizacion.estado = row.codigoEstado;
    this.cotizacionDataPersistence.cotizacion.usuarioNotificado = row.usuarioNotificado;
    this.router.navigate(['/cotizaciones', row.codigoCotizacion, 'timeline']);
  }

  alertPendingAuthorizacion(row: CotizacionItemList) {
    const message =
      'La cotización esta pendiente de autorización y no puede ser consultada hasta que sea confirmada.';
    const dialogData = new AlertDialogModel('Información', message, 'Aceptar');
    const dialogRef = this.dialog.open(AlertDialogComponent, {
      maxWidth: '500px',
      data: dialogData,
    });

    dialogRef.afterClosed().subscribe((result) => {
      // TODO handle result
    });
  }

  alertCotizacionLocked(row: CotizacionItemList) {
    const message = `No es posible acceder a la cotización <b>${row.numeroCotizacion}</b>, se encuentra bloqueada por <b>` +
      `${row.lastAuthor}</b>.`;
    const dialogData = new AlertDialogModel('Información', message, 'Aceptar');
    const dialogRef = this.dialog.open(AlertDialogComponent, {
      maxWidth: '500px',
      data: dialogData,
    });

    dialogRef.afterClosed().subscribe((result) => {
      // TODO handle result
    });
  }

  alertCotizacionAccepted(row: CotizacionItemList) {
    const message = `La cotización ${row.numeroCotizacion}, versión ${row.version} fue aceptada por ${row.lastAuthor}.`;
    const dialogData = new AlertDialogModel('Información', message, 'Aceptar');
    const dialogRef = this.dialog.open(AlertDialogComponent, {
      maxWidth: '400px',
      data: dialogData,
    });

    dialogRef.afterClosed().subscribe((result) => {
      // this.navigateCotizacion(row.codigoCotizacion, row.version);
    });
  }

  alertCotizacionClosed(row: CotizacionItemList) {
    const message = `La cotización ${row.numeroCotizacion} se encuentra cerrada.`;
    const dialogData = new AlertDialogModel('Información', message, 'Aceptar');
    const dialogRef = this.dialog.open(AlertDialogComponent, {
      maxWidth: '400px',
      data: dialogData,
    });

    dialogRef.afterClosed().subscribe((result) => {
      // TODO handle result
    });
  }

  copyCotizacion(event: Event) {
    this.notificationService.showToast('Generando copia de cotización...');
    const row: any = this.selection.selected[0];
    const codigoCotizacion = row.codigoCotizacion;
    const version = row.version;
    this.cotizacionWriterService
      .copyCotizacion(codigoCotizacion, version)
      .subscribe((res: any) => {
        this.navigateCotizacion(res.codigoCotizacion, res.version);
      });
  }


  navigateFichaTecnicaAlternativa(event: Event) {
    this.notificationService.showToast('Generando ficha técnica Alterna...');
    const row: any = this.selection.selected[0];
    const codigoCotizacion = row.codigoCotizacion;
    const version = row.version;
    this.cotizacionWriterService
      .copyAltCotizacion(codigoCotizacion, version)
      .subscribe((res: any) => {
        this.router.navigate([
          '/cotizaciones',
          res.codigoCotizacion,
          'fichatecnica',
        ], { queryParams: { versionor: version } });
        //this.navigateCotizacion(res.codigoCotizacion, res.version);
      });
  }

  createVersion(event: Event) {
    const row: any = this.selection.selected[0];
    const codigoCotizacion = row.codigoCotizacion;
    const version = row.version;
    this.cotizacionWriterService
      .createVersionCotizacion(codigoCotizacion, version)
      .subscribe((res: any) => {
        this.navigateCotizacion(res.codigoCotizacion, res.version);
      });
  }

  acceptCotizacion(event: Event) {
    const row: any = this.selection.selected[0];
    const dialogRef = this.dialog.open(AceptacionCotizacionDialogComponent, {
      maxWidth: '50em',
      data: {
        codigoCotizacion: row.codigoCotizacion,
        version: row.version,
        numeroCotizacion: row.numeroCotizacion,
        causales: this.causales,
      },
    });

    return dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.confirmCotizacion(row, result, ConfirmacionCotizacion.accepted);
      }
    });
  }

  rejectCotizacion(event: Event) {
    const row: any = this.selection.selected[0];
    const dialogRef = this.dialog.open(CerrarCotizacionDialogComponent, {
      maxWidth: '50em',
      data: {
        codigoCotizacion: row.codigoCotizacion,
        version: row.version,
        numeroCotizacion: row.numeroCotizacion,
        causales: this.causales,
      },
    });

    return dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        const action =
          result.tipoRechazo === 1
            ? ConfirmacionCotizacion.rejectedByClient
            : ConfirmacionCotizacion.rejectedByCompany;
        this.confirmCotizacion(row, result, action);
      }
    });
  }

  confirmCotizacion(row: any, result: any, action: ConfirmacionCotizacion) {
    const args = {
      codigoCotizacion: row.codigoCotizacion,
      version: row.version,
      transactionId: result.transactionId,
      causalId: result.causalId,
      tipoRechazo: result.tipoRechazo,
      action,
      observaciones: result.observaciones,
      to: result.to,
      withCopy: result.withCopy,
    };

    this.cotizacionWriterService.confirmCotizacion(args).subscribe((res) => {
      const filterArgs = new CotizacionFilter();
      this.loadCotizaciones(filterArgs);
    });
  }

  generateExpedicion(event: Event) {
    const row: any = this.selection.selected[0];
    const dialogRef = this.dialog.open(GenerateExpedicionWebComponent, {
      maxWidth: '30em',
      disableClose: true,
      data: {
        codigoCotizacion: row.codigoCotizacion,
        version: row.version,
        numeroCotizacion: row.numeroCotizacion,
      },
    });

    return dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        const message = 'Espere un momento mientras enviamos la propuesta.';
        const dialogData = new AlertDialogModel('Información', message);
        const dialogRef2 = this.dialog.open(AlertDialogPreventCloseComponent, {
          disableClose: true,
          maxWidth: '600px',
          data: dialogData,
        });

        const args = {
          codigoCotizacion: row.codigoCotizacion,
          to: [result.to],
          observaciones: result.observaciones,
        };

        this.cotizacionWriterService
          .generateExpedicion(args)
          .subscribe((res) => {
            dialogRef2.close();
            if (res) {
              const messageExpedition = 'El correo fue enviado exitosamente.';
              const dialogDatamessageExpedition = new AlertDialogModel('Información', messageExpedition);
              const dialogRef3 = this.dialog.open(AlertDialogComponent, {
                maxWidth: '400px',
                data: dialogDatamessageExpedition,
              });

              dialogRef3.afterClosed().subscribe((resultExpedition) => {
                this.refresh();
              });
            }
          });
      }
    });
  }

  onRowClick(event: Event) {
    this.toolbarConfig.reset();
    const row: CotizacionItemList = this.selection.selected[0];
    if (row) {
      const opts = this.enableToolbarOptionsByState(row.codigoEstado);
      this.toolbarConfig.enableItems(opts, true);
    }
  }

  private enableToolbarOptionsByState(codigoEstado: CotizacionState): string[] {
    const opts: string[] = ['open', 'more'];
   if (codigoEstado <= CotizacionState.Accepted) {
      opts.push('reject');
    }

    if (codigoEstado >= CotizacionState.OnFichaTecnica) {
      opts.push('nav_data_sheet');
      if (this.filterPermisson() && this.selection.selected[0].btnFichaAlterna == 1) {
        opts.push('nav_data_sheet_alternative');
      }
    }

    if (
      codigoEstado >= CotizacionState.OnSlip &&
      codigoEstado <= CotizacionState.Accepted
    ) {
      opts.push('nav_slip');
    }

    if (codigoEstado >= CotizacionState.Sent) {
      opts.push('copy', 'nav_slip', 'nav_data_sheet');
      if (this.filterPermisson() && this.selection.selected[0].btnFichaAlterna == 1) {
        opts.push('nav_data_sheet_alternative');
      }
      if (codigoEstado !== CotizacionState.Accepted) {
        if (codigoEstado !== CotizacionState.RejectedByCompany) {
          if (codigoEstado !== CotizacionState.RejectedByClient) {
            opts.push('accept');
          }
        }
      }
    }

    if (codigoEstado === CotizacionState.Sent) {
      opts.push('new_version');
    }

    if (codigoEstado === CotizacionState.Accepted || codigoEstado === CotizacionState.Issued ||
      codigoEstado === CotizacionState.ExpeditionRequest) {
      opts.push('expedition');
    }

    return opts;
  }
  filterPermisson(): any {
    var flag = false;
    var data = this.selection.selected[0].btnFichaAlterna;

    var Roluser = this.currentUser.rol.roleId
    this.permisosRol.filter(function (obj) {
      if (obj.codigo === Roluser) {
        obj.permisos.filter(function (obj2) {
          if (obj2.codigo === 9) {
            flag = true;
          }
        })
      }

    });

    return flag;
  }
  loadRolpermissions() {
    this.permisosReaderService.getRolesUsuario().subscribe((response) => {
      this.permisosRol = response
    });
  }
  navigateCotizacion(codigoCotizacion: number, version: number) {
    this.notificationService.showToast('Abriendo cotización');
    this.router.navigate(['/cotizaciones', codigoCotizacion]);
  }

  navigateFichaTecnica(e: any) {
    this.notificationService.showToast('Abriendo Ficha Técnica...');
    const row: any = this.selection.selected[0];
    this.router.navigate([
      '/cotizaciones',
      row.codigoCotizacion,
      'fichatecnica',
    ]);
  }

  navigateSlip(e: any) {
    this.notificationService.showToast('Abriendo Slip...');
    const row: any = this.selection.selected[0];
    this.router.navigate([
      '/cotizaciones',
      row.codigoCotizacion,
      'slip',
      'preview',
    ]);
  }

  onFilterChange(filterArgs: CotizacionFilter) {
    if (filterArgs) {
      this.loadCotizaciones(filterArgs);
    }
  }

  applyFilter(filterValue: string) {
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }

    this.itemsCount = this.dataSource.data.length;
  }
}

export class ConfirmCotizacionArgs {
  codigoCotizacion: number;
  version: number;
  transactionId: number;
  causalId: number;
  action: ConfirmacionCotizacion;
}
