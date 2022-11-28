import { AuthenticationService } from './../../../services/authentication.service';
import { Component, Input, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { CotizacionComponentBase } from 'src/app/CotizacionComponentBase';
import { CotizacionSectionState, Ramo, SubRamo, Sucursal } from 'src/app/models';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { CotizacionWriterService } from 'src/app/services/cotizacion-writer.service';
import { NavegacionService } from 'src/app/services/navegacion.service';
import { PersonasReaderService } from 'src/app/services/personas-reader.service';
import { RequireMatch } from 'src/app/shared/functions/requireMatch';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { MatSelect } from '@angular/material/select';

@Component({
  selector: 'app-informacion-general',
  templateUrl: './informacion-general.component.html',
  styleUrls: ['./informacion-general.component.scss']
})
export class InformacionGeneralComponent extends CotizacionComponentBase implements OnInit {

  constructor(
    private router: Router,
    private formBuilder: FormBuilder,
    private notificationService: NotificationService,
    private navigationService: NavegacionService,
    private cotizacionDataService: CotizacionPersistenceService,
    private cotizacionWriterService: CotizacionWriterService,
    private authenticationService: AuthenticationService,
    private personasReaderService: PersonasReaderService) {
    super();
  }

  @Input('model')
  model: any;

  @Input('readonly')
  readonly: boolean;

  @ViewChild('ctrlRamo', { static: true })
  ctrlRamo: MatSelect;

  indexView = 0;
  datosBasicosForm: FormGroup;
  submitted = false;

  sucursales: Sucursal[] = [];
  ramos: Ramo[] = [];
  subramos: SubRamo[] = [];

  filteredSucursales: Observable<Sucursal[]>;
  filteredRamos: Observable<Ramo[]>;
  filteredSubramos: Observable<SubRamo[]>;

  private _filterSucursales(value: string): Sucursal[] {
    if (typeof (value) === 'string') {
      return this.sucursales.filter(option => option.nombreSucursal.toLowerCase().includes(value.toLowerCase()));
    }
  }

  private _filterRamos(value: string): Ramo[] {
    if (typeof (value) === 'string') {
      return this.ramos.filter(option => option.nombreRamo.toLowerCase().includes(value.toLowerCase()));
    }
  }

  private _filterSubramos(value: string): SubRamo[] {
    if (typeof (value) === 'string') {
      return this.subramos.filter(option => option.nombreSubRamo.toLowerCase().includes(value.toLowerCase()));
    }
  }

  // convenience getter for easy access to form fields
  get form() { return this.datosBasicosForm.controls; }

  ngOnInit() {
    this.navigationService.subscribe({
      indexView: this.indexView, continuePromise: () => {
        return this.continue();
      }
    });

    this.initializeForm();
    this.loadSucursales();

    this.registerOnSucursalChange();
    this.registerOnRamoChange();
  }

  private initializeForm() {
    // Definicion del formulario
    this.datosBasicosForm = this.formBuilder.group({
      Sucursal: ['', [Validators.required, RequireMatch]],
      Ramo: ['', [Validators.required, RequireMatch]],
      Subramo: ['', [Validators.required, RequireMatch]]
    });

    if (this.readonly) {
      this.datosBasicosForm.disable();
    }
  }

  private registerOnRamoChange() {
    this.datosBasicosForm.get('Ramo')
      .valueChanges
      .subscribe((selection) => {
        if (typeof (selection) === 'object') {
          this.loadSubRamosRamo(selection.codigoRamo);
        }
      });
  }

  private loadSubRamosRamo(codigoRamo: any) {
    this.personasReaderService.getSubRamosPorRamo(codigoRamo)
      .subscribe((response: SubRamo[]) => {
        if (response.length === 0) {
          let ramo = SubRamo.create('No hay subramos disponibles.');
          this.subramos.push(ramo);
        }
        else {
          this.subramos = response;
          this.filteredSubramos = this.datosBasicosForm.get('Subramo')
            .valueChanges
            .pipe(
              startWith(''),
              map(value => this._filterSubramos(value))
            );

          if (this.model.codigoSubramo) {
            let subramo = this.subramos.find(x => x.codigoSubRamo === this.model.codigoSubramo);
            const ctrl = this.datosBasicosForm.get('Subramo');
            ctrl.setValue(subramo, { emitEvent: false });
            //ctrl.disable();

            this.datosBasicosForm.disable({ emitEvent: false });
          }
        }
      });
  }

  private registerOnSucursalChange() {
    this.datosBasicosForm.get('Sucursal')
      .valueChanges
      .subscribe((selection) => {
        if (selection !== '') {
          if (typeof (selection) === 'object') {
            this.loadRamosSucursal(selection.codigoSucursal);
          }
        }
      });
  }

  private loadRamosSucursal(codigoSucursal: any) {
    this.personasReaderService.getRamos()
      .subscribe((response: Ramo[]) => {
        this.ramos = response;
        this.filteredRamos = this.datosBasicosForm.get('Ramo')
          .valueChanges
          .pipe(
            startWith(''),
            map(value => this._filterRamos(value))
          );

        if (this.model.codigoRamo) {
          let ramo = this.ramos.find(x => x.codigoRamo === this.model.codigoRamo);
          const ctrl = this.datosBasicosForm.get('Ramo');
          ctrl.setValue(ramo, { emitEvent: false });
          //ctrl.disable();

          this.loadSubRamosRamo(ramo.codigoRamo);
        }
      });
  }

  private loadSucursales() {
    const ctrl = this.datosBasicosForm.get('Sucursal');
    this.personasReaderService.getSucursales()
      .subscribe((response: Sucursal[]) => {
        this.sucursales = response;
        this.filteredSucursales = this.datosBasicosForm.get('Sucursal')
          .valueChanges
          .pipe(
            startWith(''),
            map(value => this._filterSucursales(value))
          );

        let codigoSucursal;
        const userInfo = this.authenticationService.currentUserValue.externalInfo;
        if (userInfo) {
          //codigoSucursal = userInfo.sucursal;
          codigoSucursal = userInfo.sucursal === 800 ? this.model.codigoSucursal : userInfo.sucursal
          let sucursal = response.find(x => x.codigoSucursal === codigoSucursal);
          if (this.sucursales.length === 1) {
            ctrl.setValue(sucursal, { emitEvent: false });
            ctrl.disable();
            this.ctrlRamo.open();
          }
        }

        let sucursal = response.find(x => x.codigoSucursal === codigoSucursal);
        if (!this.model.codigoSucursal && codigoSucursal) {
          ctrl.setValue(sucursal, { emitEvent: false });
          this.loadRamosSucursal(sucursal.codigoSucursal);
        }

        if (this.model.codigoSucursal) {
          let sucursal1 = response.find(x => x.codigoSucursal === this.model.codigoSucursal)
          ctrl.setValue(sucursal1, { emitEvent: false });
          this.loadRamosSucursal(sucursal1.codigoSucursal);
          // ctrl.setValue(this.model.codigoSucursal, { emitEvent: false });
          // this.loadRamosSucursal(this.model.codigoSucursal);
        }
      });
  }

  private async continue() {
    if (!this.cotizacionDataService.isEdit) {
      let toast = this.notificationService.showToast('Inicializando Cotización', 0);
    }

    let promise = new Promise((resolve, reject) => {
      const codigoCotizacion = this.cotizacionDataService.cotizacion.codigoCotizacion;
      this.submitted = true;
      const isValidForm: boolean = !this.datosBasicosForm.invalid || codigoCotizacion > 0;
      const formCompleted = this.cotizacionDataService.cotizacion.informacionBasica.state !== CotizacionSectionState.Completed;
      if (isValidForm && formCompleted && !this.cotizacionDataService.isEdit) {
        let model = this.datosBasicosForm.getRawValue();
        const codigoZona = model.Sucursal.codigoZona;
        const codigoSucursal = model.Sucursal.codigoSucursal;
        const codigoRamo = model.Ramo.codigoRamo;
        const codigoSubramo = model.Subramo.codigoSubRamo;

        this.notificationService.showToast('Creando nueva cotización...', 3000);

        this.cotizacionWriterService.initializeCotizacion(codigoSucursal, codigoRamo, codigoSubramo, codigoZona)
          .subscribe((res: any) => {
            if (res) {
              // redirect to consultar cotizacion
              this.router.navigate(['cotizaciones', res.codigoCotizacion]);
              resolve(true);
            }
          });
      }
      else {
        resolve(true);
      }
    });

    let result = await promise;
    return result;
  }

}
