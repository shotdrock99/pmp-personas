import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GuardarResumenArgs, TasaOpcion } from 'src/app/models/guardar-resumen-args';
import { PageToolbarConfig } from 'src/app/models/page-toolbar-item';
import { ProcesarResumenResponse, Resumen } from 'src/app/models/resumen';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { CotizacionWriterService } from 'src/app/services/cotizacion-writer.service';
import { ResumenCotizacionReaderService } from 'src/app/services/resumen-cotizacion-reader.service';
import { ResumenCotizacionWriterService } from 'src/app/services/resumen-cotizacion-writer.service';
import { ResumenInlinepreprocessorService } from 'src/app/services/resumen-cotizacion-inlineprocessor.service';
import { PageToolbarBuilder } from 'src/app/shared/services/page-toolbar-builder';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
  selector: 'app-resumen-cotizacion',
  templateUrl: './resumen-cotizacion.component.html',
  styleUrls: ['./resumen-cotizacion.component.scss'],
})
export class ResumenCotizacionComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private toolbarBuilder: PageToolbarBuilder,
    private cotizacionDataService: CotizacionPersistenceService,
    private cotizacionWriterService: CotizacionWriterService,
    private resumenService: ResumenCotizacionReaderService,
    private preprocessorService: ResumenInlinepreprocessorService,
    private resumenWriter: ResumenCotizacionWriterService,
    private authenticationService: AuthenticationService
  ) { }

  codigoCotizacion: number;
  version: number;
  numeroCotizacion: string;
  model: Resumen;
  pageLoaded = false;
  toolbarConfig: PageToolbarConfig;
  readonly: boolean;
  userRole: boolean;
  blockComision: boolean;
  controlColumn: number;
  ShowData: boolean = true;
  get cotizacion() {
    return this.cotizacionDataService.cotizacion;
  }

  ngOnInit() {
    this.route.data.subscribe((res) => {
      this.readonly = res.data.readonly;
      this.blockComision = res.data.informacionNegocio.esNegocioDirecto;
      this.codigoCotizacion = res.data.codigoCotizacion;
      this.version = res.data.version;
      this.numeroCotizacion = res.data.numero;
      this.cotizacionDataService.setCotizacionState(res.data.estado);
      this.consultarResumen();
    });

    this.getIsGSPUser();
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
        icon_path: 'restore',
        label: '',
        tooltip: 'Restaurar',
        isEnabled: true,
        fixed: true,
        onClick: () => {
          this.refresh(this.model.tieneSiniestralidad);
        },
      },
      {
        name: 'save',
        icon_path: 'save',
        label: '',
        tooltip: 'Guardar',
        isEnabled: true,
        fixed: true,
        onClick: () => {
          this.saveIt(this.model.tieneSiniestralidad);
        },
      },
    ];

    this.toolbarConfig = this.toolbarBuilder.build(items);
  }

  navigateToCotizaciones() {
    localStorage.removeItem('resumen');
    this.router.navigate(['cotizaciones']);
  }

  private getIsGSPUser() {
    const user = this.authenticationService.currentUserValue.rol.roleName.toLowerCase();
    this.userRole = user.includes('gsp') ? true : false;
  }

  triggerUnlockCotizacion() {
    this.cotizacionWriterService
      .unlockCotizacion(this.codigoCotizacion, this.version)
      .subscribe((res) => res);
  }

  private consultarResumen(callback?: any) {
    this.resumenService
      .consultarResumen(this.codigoCotizacion, this.version)
      .subscribe((response: ProcesarResumenResponse) => {
        if (!response) { return; }
        this.model = response.data;
        this.configurarTabla();
        localStorage.setItem('resumen', JSON.stringify(this.model));
        this.cotizacionDataService.setCotizacionState(response.cotizacionState);
        this.pageLoaded = true;
        if (callback) {
          callback();
        }
      });
  }
  private configurarTabla() {
    this.controlColumn = 0;
    this.ShowData = !(this.model.gruposAsegurados[0].opciones[0].tasaComercialAnual == 0) 
    var tmp : any = [];
    this.model.gruposAsegurados.forEach(element => {
      tmp = [];
      element.opciones.forEach(element => {
        if (element.valorAsegurado != 0) {
          this.controlColumn++;
          tmp.push(element);
        }

      });
      element.opciones = tmp;
    });

  }
  private consultarResumenAfterUpdate() {
    this.resumenService
      .consultarResumen(this.codigoCotizacion, this.version)
      .subscribe((res) => {
        this.model = res.data;
        this.configurarTabla();
      });
  }

  private navigateToFichaTecnica() {
    localStorage.removeItem('resumen');
    const commands = ['cotizaciones', this.codigoCotizacion, 'fichatecnica'];
    const extras = {
      queryParams: {
        version: this.version,
      },
    };

    this.router.navigate(commands, extras);
  }

  private mapTasaOpciones(esSiniestralidad: boolean): TasaOpcion[] {
    const result: TasaOpcion[] = [];
    this.model.gruposAsegurados.forEach((g) => {
      g.opciones.forEach((o) => {
        const option: TasaOpcion = {
          codigoGrupoAsegurado: g.codigo,
          descuento: o.porcentajeDescuento,
          descuentoSiniestralidad: esSiniestralidad ? o.siniestralidad.porcentajeDescuento : 0,
          indiceOpcion: o.indiceOpcion,
          primaIndividual: o.primaAnualIndividual,
          primaTotal: o.primaAnualTotal,
          recargo: o.porcentajeRecargo,
          recargoSiniestralidad: esSiniestralidad ? o.siniestralidad.porcentajeRecargo : 0,
          tasaComercial: o.tasaComercialAnual,
          tasaComercialTotal: o.tasaComercialAplicar,
        };
        result.push(option);
      });
    });

    return result;
  }

  // onChangeDescuentoRecargo(e, esSiniestralidad) {
  //   let nModel = this.preprocessorService.process(this.model, esSiniestralidad);
  //   this.model = nModel;
  // }

  onChangeDescuentoRecargo(e, opcion, esSiniestralidad) {
    const nModel = this.preprocessorService.processOpcion(
      opcion.indiceOpcion,
      this.model,
      esSiniestralidad
    );
    this.model = nModel;
  }

  // visibleGroups = [];
  // onChange(e, esSiniestralidad) {
  //   let nModel = this.preprocessorService.process(this.model, esSiniestralidad);
  //   this.model = nModel;

  //   this.visibleGroups = [];
  //   this.model.gruposAsegurados.map((x) =>
  //     this.visibleGroups.push({ key: x.codigo, value: x.visible })
  //   );

  //   // this.save();
  //   // this.consultarResumen(() => {
  //   //   this.visibleGroups.forEach(x => {
  //   //     this.model.gruposAsegurados.find(g => g.codigo === x.key).visible = x.value;
  //   //   });
  //   // });
  // }

  onChange(e, esSiniestralidad) {
    this.saveIt(esSiniestralidad);
  }

  reload(args, esSiniestralidad) {
    this.refresh(esSiniestralidad);
  }

  refresh(esSiniestralidad) {
    // this.pageLoaded = false;
    // this.consultarResumen();
    this.model = JSON.parse(localStorage.getItem('resumen'));
    this.saveIt(esSiniestralidad);
  }

  save(esSinestralidad?: boolean) {
    // const args: GuardarResumenArgs = {
    //   codigoCotizacion: this.codigoCotizacion,
    //   porcentajeRetorno: this.model.gRetorno,
    //   porcentajeOtrosGastos: this.model.otrosGastos,
    //   porcentajeComision: this.model.comision,
    //   utilidadCompania: this.model.utilidad,
    //   gastosCompania: this.model.gastosCompania,
    //   factorG: this.model.factorG,
    //   tasaOpciones: this.mapTasaOpciones(),
    // };

    const args1: any = {
      codigoCotizacion: this.codigoCotizacion,
      porcentajeComision: this.model.comision,
      porcentajeIvaComision: this.model.ivaComision,
      porcentajeRetorno: this.model.gRetorno,
      porcentajeIvaRetorno: this.model.ivagRetorno,
      porcentajeOtrosGastos: this.model.otrosGastos,
      gastosCompania: this.model.gastosCompania,
      utilidadCompania: this.model.utilidad,
      factorG: this.model.factorG,
      tasaOpciones: this.mapTasaOpciones(esSinestralidad),
    };

    return this.resumenWriter.SaveResumen1(args1);
  }

  saveIt(esSiniestralidad?: boolean) {
    this.save(esSiniestralidad).subscribe((res) => this.consultarResumenAfterUpdate());
  }

  continue() {
    if (this.model.factorG > 100) {
      return;
    } else {
      this.save(this.model.tieneSiniestralidad).subscribe((res) => {
        if (res) {
          this.navigateToFichaTecnica();
        }
      });
    }
  }
}
