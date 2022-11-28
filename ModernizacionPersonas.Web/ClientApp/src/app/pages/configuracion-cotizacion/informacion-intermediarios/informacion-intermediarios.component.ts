import { InformacionIntermediariosViewData } from './../../../models/cotizacion';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { BreakpointObserver } from '@angular/cdk/layout';
import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { Intermediario, TipoDocumento } from 'src/app/models';
import { IntermediarioState } from 'src/app/models/intermediario-state';
import { InformacionintermediariosWriterService } from 'src/app/services/informacionintermediarios-writer.service';
import { NavegacionService } from 'src/app/services/navegacion.service';
import { ConfirmDialogComponent, ConfirmDialogModel } from 'src/app/shared/components/confirm-dialog';
import { ConfigurarIntermediarioComponent } from './components/configurar-intermediario/configurar-intermediario.component';
import { InformacionIntermediario } from 'src/app/models/informacion-intermediario';
import { ApplicationUser } from 'src/app/models/application-user';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { PersonasReaderService } from 'src/app/services/personas-reader.service';
import { NotificationService } from 'src/app/shared/services/notification.service';
import { timeInterval } from 'rxjs/operators';

@Component({
  selector: "app-informacion-intermediarios",
  templateUrl: "./informacion-intermediarios.component.html",
  styleUrls: ["./informacion-intermediarios.component.scss"],
})
export class InformacionIntermediariosComponent implements OnInit {
  private indexView = 3;
  private formErrors: any = {};
  private sectionInfo = this.navigationService.sections.getItemByIndex(
    this.indexView
  );

  @Input()
  public esNegocioDirecto: boolean;

  @Input("model")
  model: InformacionIntermediariosViewData;

  @Input("readonly")
  readonly: boolean;
  @Input("version")
  version: number;

  canContinue: boolean = false;
  submitted = false;
  allowAgregarIntermediario: boolean;
  sumPorcentajeParticipacion: any = 0;
  formEnabled = false;
  itemsCount = 0;
  intermediariosForm = {
    isValid: false,
    errors: this.formErrors,
    setErrors: (error) => {
      const key = Object.keys(error)[0];
      const value = Object.values(error)[0];
      this.formErrors[key] = value;
    },
    disable: () => {
      this.formEnabled = false;
      this.allowAgregarIntermediario =
        this.formEnabled || (this.sectionInfo.initialized && !this.readonly);
    },
    enable: () => {
      this.formEnabled = true;
    },
  };

  intermediarios: Intermediario[] = [];
  showUnder100: boolean;
  showOver100: boolean;
  currentUser: ApplicationUser = this.authenticationService.currentUserValue;

  constructor(
    public dialog: MatDialog,
    private cotizacionDataService: CotizacionPersistenceService,
    private notificationService: NotificationService,
    private navigationService: NavegacionService,
    private intermediariosWriterService: InformacionintermediariosWriterService,
    private breakpointObserver: BreakpointObserver,
    private authenticationService: AuthenticationService,
    private personasReaderService: PersonasReaderService
  ) {
    this.breakpointObserver
      .observe(["(max-width: 600px)"])
      .subscribe((result) => {
        this.displayedColumns = result.matches
          ? [
            "clave",
            "nombre",
            "tipodocumento",
            "numerodocumento",
            "porcentajeparticipacion",
            "opciones",
          ]
          : [
            "clave",
            "nombre",
            "tipodocumento",
            "numerodocumento",
            "porcentajeparticipacion",
            "opciones",
          ];
      });
  }

  dataSource = new MatTableDataSource<any>([]);
  displayedColumns: string[] = [
    "clave",
    "nombre",
    "tipodocumento",
    "numerodocumento",
    "porcentajeparticipacion",
    "opciones",
  ];

  ngOnInit() { 
    debugger;   
    if (this.version > 1) {
      this.readonly = true;
    }
    if (this.currentUser.rol.roleId === 9 && this.cotizacionDataService.isEdit === false) {
      let codigoSucursal = this.cotizacionDataService.cotizacion.informacionBasica.codigoSucursal;
      let claveIntermediario = Number(this.currentUser.userName);
      let intermediario: Intermediario;
      this.personasReaderService
        .consultarIntermediario(codigoSucursal, claveIntermediario)
        .subscribe((response) => {
          
          intermediario = {
            Clave: response.codigoAgente,
            TipoDocumento: response.codigoTipoDocumento,
            NumeroDocumento: response.numeroDocumento,
            PrimerNombre: response.primerNombre,
            SegundoNombre: response.segundoNombre,
            PrimerApellido: response.primerApellido,
            SegundoApellido: response.segundoApellido,
            PorcentajeParticipacion: 100,
            Estado: IntermediarioState.Active,
            TipoPersona: response.codigoTipoAgente,
            Email: response.correoElectronico
          };

          this.agregarIntermediario(intermediario);
        });
    }

    this.navigationService.subscribe({
      indexView: this.indexView,
      continuePromise: () => {
        return this.continue();
      },
      initializePromise: () => {
        this.initializeSection();
      },
    });

    this.intermediariosForm.disable();
    if (this.cotizacionDataService.isEdit) {
      this.updateForm();
    }
  }

  private updateForm() {
    this.esNegocioDirecto = this.cotizacionDataService.cotizacion.informacionNegocio.esNegocioDirecto;
    if (this.esNegocioDirecto) {
      this.allowAgregarIntermediario = false;
      this.dataSource = new MatTableDataSource<any>(this.model.intermediarios);
      this.sumPorcentajeParticipacion = 0;
    } else {
      if (this.model.intermediarios.length > 0) {
        this.sumPorcentajeParticipacion = this.model.intermediarios
          .map((x) => x.participacion)
          .reduce((a, b) => {
            return a + b;
          });
        // update intermediarios list
        this.intermediarios = this.convertToIntermediarios(
          this.model.intermediarios
        );
        let tableData = this.convertToTableData(this.intermediarios);
        this.dataSource = new MatTableDataSource<any>(tableData);
        this.itemsCount = this.dataSource.data.length;

        this.validateIntermediarios();
        this.validateForm();
      }
    }

    this.intermediariosForm.enable();
    if (this.readonly) {
      this.intermediariosForm.disable();
    }

    this.canContinue = this.model.intermediarios.length > 0;
  }

  private convertToTableData(intermediarios: Intermediario[]): any[] {
    let result = [];
    intermediarios.forEach((x) => {
      let item = this.convertToTableItem(x);
      result.push(item);
    });

    return result;
  }

  private convertToIntermediarios(
    intermediarios: InformacionIntermediario[]
  ): Intermediario[] {
    var result: Intermediario[] = [];
    intermediarios.forEach((x) => {
      const tipoDocumento = this.cotizacionDataService.tiposDocumento.find(
        (l) => l.codigoTipoDocumento === x.codigoTipoDocumento
      );
      result.push({
        Codigo: x.codigo,
        Clave: x.clave.toString(),
        TipoDocumento: tipoDocumento,
        NumeroDocumento: x.numeroDocumento,
        PrimerNombre: x.primerNombre,
        SegundoNombre: x.segundoNombre,
        PrimerApellido: x.primerApellido,
        SegundoApellido: x.segundoApellido,
        TipoPersona: "",
        Email: x.email,
        PorcentajeParticipacion: x.participacion,
        Estado: IntermediarioState.Active,
      });
    });

    return result;
  }

  private initializeSection() {
    this.allowAgregarIntermediario = !(
      this.esNegocioDirecto || this.esNegocioDirecto === undefined
    );
    if (this.esNegocioDirecto) {
      this.sumPorcentajeParticipacion = 0;
      this.dataSource.data.splice(0, this.intermediarios.length);
      const ds = this.dataSource.data;
      this.dataSource.data = ds;
    }
  }

  private openIntermediariosModal(data: any) {
    return this.dialog.open(ConfigurarIntermediarioComponent, {
      width: "500px",
      data,
    });
  }

  private convertToTableItem(intermediario: any) {
    const result = {
      Clave: intermediario.Clave,
      Codigo: intermediario.Codigo,
      TipoDocumento: intermediario.TipoDocumento,
      NumeroDocumento: intermediario.NumeroDocumento,
      Nombre: null,
      Email: intermediario.Email,
      PorcentajeParticipacion: intermediario.PorcentajeParticipacion,
    };

    const esPersonaNatural =
      intermediario.TipoDocumento.codigoTipoDocumento !== 3 &&
      intermediario.TipoDocumento.codigoTipoDocumento !== 19;

    result.Nombre = esPersonaNatural
      ? `${intermediario.PrimerNombre} ${intermediario.SegundoNombre} ${intermediario.PrimerApellido} ${intermediario.SegundoApellido}`
      : intermediario.PrimerApellido;

    return result;
  }

  private validateIntermediarios() {
    const intermediarios = this.dataSource.data;
    if (intermediarios.length === 0) {
      this.sumPorcentajeParticipacion = 0;
    } else {
      let sum = 0;
      intermediarios.forEach((i) => {
        const v = Number(i.PorcentajeParticipacion);
        sum += v;
      });

      this.sumPorcentajeParticipacion = sum;

      if (typeof this.sumPorcentajeParticipacion === "object") {
        this.sumPorcentajeParticipacion = this.sumPorcentajeParticipacion.PorcentajeParticipacion;
      }
    }
    //controlar si se  permite agregar o no intermediarios
    this.allowAgregarIntermediario = this.sumPorcentajeParticipacion < 100;
    if(this.version > 1)
    {
      this.allowAgregarIntermediario = false;
    }
  }

  private existeIntermediario(
    vm: any,
    codigoTipoDocumento: number,
    numeroDocumento: string
  ) {
    const intermediario = this.dataSource.data.find(
      (x) =>
        x.TipoDocumento.codigoTipoDocumento === codigoTipoDocumento &&
        x.NumeroDocumento === numeroDocumento
    );
    return !(intermediario === undefined);
  }

  abrirAgregarIntermediario(): void {
    const dialogRef = this.openIntermediariosModal({
      isEdit: false,
      model: new Intermediario(),
      sumPorcentajeParticipacion: this.sumPorcentajeParticipacion,
      validarIntermediario: (
        codigoTipoDocumento: number,
        numeroDocumento: string
      ) => {
        return this.existeIntermediario(
          this,
          codigoTipoDocumento,
          numeroDocumento
        );
      },
    });

    dialogRef.afterClosed().subscribe(async (result) => {
      if (result === undefined) {
        return;
      }

      const intermediario: Intermediario = {
        Clave: result.Clave,
        TipoDocumento: result.TipoDocumento,
        NumeroDocumento: result.NumeroDocumento,
        PrimerNombre: result.PrimerNombre,
        SegundoNombre: result.SegundoNombre,
        PrimerApellido: result.PrimerApellido,
        SegundoApellido: result.SegundoApellido,
        PorcentajeParticipacion: result.PorcentajeParticipacion,
        Estado: IntermediarioState.Active,
        TipoPersona: result.TipoPersona,
        Email: result.Email,
      };

      this.agregarIntermediario(intermediario);
    });
  }

  private agregarIntermediario(intermediario: Intermediario) {
    let toast = this.notificationService.showToast('Guardando Intermediario', 0);
    this.intermediariosWriterService
      .crearIntermediario(intermediario)
      .subscribe((res) => {
        if (!res) {
          toast.dismiss();
          return;
        }

        intermediario.Codigo = res.codigo;
        const tableItem = this.convertToTableItem(intermediario);
        this.intermediarios.push(intermediario);

        const ds = this.dataSource.data;
        ds.push(tableItem);
        this.dataSource.data = ds;

        this.itemsCount = this.dataSource.data.length;

        this.validateIntermediarios();
        this.validateForm();

        toast.dismiss();
        this.canContinue = true;
      });
  }

  abrirEditarIntermediario(e, args: Intermediario): void {
    ;
    const model = this.intermediarios.find((x) => x.Codigo === args.Codigo);
    const dialogRef = this.openIntermediariosModal({
      isEdit: true,
      readonly: this.readonly,
      model,
      isFirstVersion : this.version > 1
    });

    dialogRef.afterClosed().subscribe(async (result) => {
      // remove item from datasource
      if (result === undefined) {
        return;
      }
      const idx = this.dataSource.data.findIndex(
        (x) => x.NumeroDocumento === result.NumeroDocumento
      );
      this.dataSource.data.splice(idx, 1);

      const intermediario: Intermediario = {
        Codigo: result.Codigo,
        Clave: result.Clave,
        Estado: IntermediarioState.Active,
        TipoDocumento: result.TipoDocumento,
        NumeroDocumento: result.NumeroDocumento,
        PrimerNombre: result.PrimerNombre,
        SegundoNombre: result.SegundoNombre,
        PrimerApellido: result.PrimerApellido,
        SegundoApellido: result.SegundoApellido,
        Email: result.Email,
        PorcentajeParticipacion: result.PorcentajeParticipacion,
        TipoPersona: result.TipoPersona,
      };

      let toast = this.notificationService.showToast('Guardando Intermediario', 0);

      this.intermediariosWriterService
        .actualizarIntermediario(intermediario)
        .subscribe((res) => {
          
          const tableItem = this.convertToTableItem(intermediario);

          const ds = this.dataSource.data;
          ds.push(tableItem);
          this.dataSource.data = ds;
          this.intermediarios.forEach(element => {
            if(element.Codigo == intermediario.Codigo){
              element.Email = intermediario.Email;
              element.NumeroDocumento = intermediario.NumeroDocumento;
              element.PrimerNombre = intermediario.PrimerNombre;
              element.SegundoNombre = intermediario.SegundoNombre;
              element.PrimerApellido = intermediario.PrimerApellido;
              element.SegundoApellido = intermediario.SegundoApellido;
              element.PorcentajeParticipacion = intermediario.PorcentajeParticipacion;
              element.TipoDocumento = intermediario.TipoDocumento;
              element.TipoPersona = intermediario.TipoPersona;
            }
          });
          //this.intermediarios = ds;
          this.validateIntermediarios();
          this.validateForm();

          toast.dismiss();
        });
    });
  }

  eliminarIntermediario(e, args): void {
    const message = `¿Está seguro de que desea eliminar este intermediario?`;
    const dialogData = new ConfirmDialogModel(
      "Eliminar Intermediario",
      message
    );
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: "400px",
      data: dialogData,
    });

    dialogRef.afterClosed().subscribe(async (dialogResult: boolean) => {
      if (dialogResult) {
        // remove item from datasource
        const idx = this.dataSource.data.findIndex(
          (x) => x.NumeroDocumento === args.NumeroDocumento
        );
        this.dataSource.data.splice(idx, 1);
        const ds = this.dataSource.data;
        this.dataSource.data = ds;

        this.itemsCount = this.dataSource.data.length;

        this.intermediariosWriterService
          .eliminarIntermediario(args.Codigo)
          .subscribe((res) => {
            this.validateIntermediarios();
            this.validateForm();
          });
      }
    });
  }

  private compareInformacionNegocioForm() {
    return true;
  }

  private validateForm(): boolean {
    this.intermediariosForm.setErrors({
      empty: this.dataSource.data.length === 0,
    });
    if (this.dataSource.data.length > 0) {
      if (this.sumPorcentajeParticipacion < 100) {
        this.intermediariosForm.setErrors({ under100: true });
      } else if (this.sumPorcentajeParticipacion < 100) {
        this.intermediariosForm.setErrors({ over100: true });
      }

      this.showUnder100 = this.sumPorcentajeParticipacion < 100;
      this.showOver100 = this.sumPorcentajeParticipacion > 100;
    }

    return this.sumPorcentajeParticipacion === 100 && this.canContinue;
  }

  private async continue() {
    this.submitted = true;
    let isValid = this.validateForm();
    if (isValid) this.intermediariosForm.setErrors({ under100: null });
    if (!isValid) return;

    let promise = new Promise((resolve, reject) => {
      this.intermediariosForm.isValid = isValid;
      const hasChanges = this.compareInformacionNegocioForm();
      if (this.intermediariosForm.isValid && hasChanges) {
        resolve(true);
      }
    });

    //return this.intermediariosForm.isValid;
    let result = await promise;
    return result;
  }

  OnInformacionNegocioChange(args): void { }
}

export class FormError {
  key: string;
  value: boolean;

  constructor(key: string, value: boolean) {
    this.key = key;
    this.value = value;
  }
}
