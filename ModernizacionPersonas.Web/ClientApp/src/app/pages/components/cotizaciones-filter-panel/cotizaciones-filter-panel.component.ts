import { Component, OnInit, Output, EventEmitter, Host, Input } from '@angular/core';
import { FormBuilder, Form, FormGroup } from '@angular/forms';
import { PersonasReaderService } from 'src/app/services/personas-reader.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { CotizacionFilter } from 'src/app/models/cotizacion-filter';
import { Sucursal, Ramo, SubRamo } from 'src/app/models';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MomentDateAdapter } from '@angular/material-moment-adapter';

export const DATE_FORMATS = {
  parse: {
    dateInput: 'DD/MM/YYYY',
  },
  display: {
    dateInput: 'DD/MM/YYYY',
    monthYearLabel: 'MM/YYYY',
    dateA11yLabel: 'DD/MM/YYYY',
    monthYearA11yLabel: 'MM/YYYY',
  },
};
export const DEFAULT_DATETIME = '0001-01-01T00:00:00';

@Component({
  selector: 'app-cotizaciones-filter-panel',
  templateUrl: './cotizaciones-filter-panel.component.html',
  styleUrls: ['./cotizaciones-filter-panel.component.scss'],
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'es' },
    { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
    { provide: MAT_DATE_FORMATS, useValue: DATE_FORMATS },
  ]
})
export class CotizacionesFilterPanelComponent implements OnInit {

  constructor(
    private formBuilder: FormBuilder,
    private personasReaderService: PersonasReaderService,
    private authenticationService: AuthenticationService,
    private cotizacionDataService: CotizacionPersistenceService,
    private dateAdapter: DateAdapter<any>) { }

  @Output() filter: EventEmitter<CotizacionFilter> = new EventEmitter();
  @Input() data: any;

  zonasList: any[];
  sucursalesList: Sucursal[];
  ramosList: Ramo[];
  subramosList: SubRamo[];
  filterForm: FormGroup;
  tiposDocumentoList: any[];

  // get tiposDocumentoList(): any[] {
  //   if (this.cotizacionDataService.tiposDocumento !== null) {
  //     return this.cotizacionDataService.tiposDocumento;
  //   }

  //   this.personasReaderService.getTiposDocumento();
  // }

  get estadosList(): any[] {
    return this.cotizacionDataService.estados;
  }

  get loggedUser() {
    return this.authenticationService.currentUserValue;
  }

  get form() { return this.filterForm.controls; }

  ngOnInit(): void {
    this.dateAdapter.setLocale('es');
    this.initializeForm();
    this.loadZonas();
    this.loadRamosSucursal('');
    this.registerOnZonaChange();
    this.registerOnSucursalChange();
    this.registerOnRamoChange();
    this.loadTiposDocumento();
  }

  initializeForm() {
    const codigoZona = this.loggedUser.externalInfo.zona === 26 ? null : this.loggedUser.externalInfo.zona;
    const codigoSucursal = this.loggedUser.externalInfo.sucursal === 800 ? null : this.loggedUser.externalInfo.sucursal;
    this.filterForm = this.formBuilder.group({
      codigoZona: [codigoZona],
      codigoSucursal: [codigoSucursal],
      codigoRamo: [null],
      codigoSubramo: [null],
      numeroCotizacion: [null],
      fechaDesde: [null],
      fechaHasta: [null],
      codigoEstado: [null],
      // subestado
      codigoUsuario: [null],
      codigoTipoDocumento: [null],
      numeroDocumento: [null]
    });
    // this.clearFilter();
    if (this.data && this.data.calledBy === 'Autorizaciones') {
      this.filterForm.get('codigoEstado').disable();
      this.filterForm.patchValue({
        codigoEstado: 1111
      });
    }
    // TODO check, handle data from server
    // if (this.loggedUser.externalInfo.sucursal !== 800 && this.loggedUser.rol.roleId !== 9) {
      //   this.filterForm.get('codigoZona').disable();
      //   if (this.loggedUser.rol.roleId !== 4) {
        //     this.filterForm.get('codigoSucursal').disable();
        //   }
        // }
    this.loadSucursales(codigoZona);
  }

  public loadTiposDocumento() {
    if (this.cotizacionDataService.tiposDocumento !== null) {
      this.tiposDocumentoList = this.cotizacionDataService.tiposDocumento;
    }

    this.personasReaderService
      .getTiposDocumento()
      .subscribe((res) => (this.tiposDocumentoList = res));
  }

  private registerOnZonaChange() {
    this.filterForm.get('codigoZona')
      .valueChanges
      .subscribe(sel => {
        this.loadSucursales(sel);
      });
  }

  private registerOnSucursalChange() {
    this.filterForm.get('codigoSucursal')
      .valueChanges
      .subscribe((selection) => {
        if (selection !== '') {
          this.loadRamosSucursal(selection);
        }
      });
  }

  private registerOnRamoChange() {
    this.filterForm.get('codigoRamo')
      .valueChanges
      .subscribe((selection) => {
        this.loadSubRamosRamo(selection);
      });
  }

  private loadZonas() {
    this.personasReaderService.getZonas()
      .subscribe((response: any) => {
        this.zonasList = response;
        if (response.length === 1) {
          this.filterForm.get('codigoZona').disable();
        }
      });
  }

  private loadSucursales(codigoZona: number) {
    if (codigoZona === null) {
      return;
    }
    this.personasReaderService.getSucursales(codigoZona)
      .subscribe((response: Sucursal[]) => {
        this.sucursalesList = response;
        if (response.length === 1) {
          this.filterForm.get('codigoSucursal').disable();
        }
      });
  }

  private loadRamosSucursal(codigoSucursal: any) {
    this.personasReaderService.getRamos()
      .subscribe((response: Ramo[]) => {
        this.ramosList = response;
      });
  }

  private loadSubRamosRamo(codigoRamo: any) {
    if (codigoRamo === null) {
      return;
    }
    this.personasReaderService.getSubRamosPorRamo(codigoRamo)
      .subscribe((response: SubRamo[]) => {
        if (response.length === 0) {
          const ramo = SubRamo.create('No hay subramos disponibles.');
          this.subramosList.push(ramo);
        } else {
          this.subramosList = response;
        }
      });
  }

  applyFilter() {
    const filterArgs: CotizacionFilter = this.filterForm.getRawValue();
    this.filter.emit(filterArgs);
  }

  clearFilter() {
    const ctrlZona = this.filterForm.get('codigoZona');
    const ctrlSucursal = this.filterForm.get('codigoSucursal');
    const ctrlEstado = this.filterForm.get('codigoEstado');
    this.filterForm.setValue({
      codigoZona: ctrlZona.value,
      codigoSucursal: ctrlSucursal.value,
      codigoRamo: null,
      codigoSubramo: null,
      numeroCotizacion: null,
      fechaDesde: null,
      fechaHasta: null,
      codigoEstado: ctrlEstado.value,
      // subestado
      codigoUsuario: null,
      codigoTipoDocumento: null,
      numeroDocumento: null
    });
    if (!ctrlZona.disabled) {
      ctrlZona.setValue(null);
    }
    if (!ctrlSucursal.disabled) {
      ctrlSucursal.setValue(null);
    }
    if (!ctrlEstado.disabled) {
      ctrlEstado.setValue(null);
    }
    this.filter.emit();
  }
}
