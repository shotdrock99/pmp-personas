import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ObservableInput, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

import { CotizacionSectionState } from '../models';
import { NotificationService } from '../shared/services/notification.service';
import { CotizacionApiRouterService } from './cotizacion-api-router.service';
import { CotizacionPersistenceService } from './cotizacion-persistence.service';

@Injectable({
  providedIn: 'root'
})
export class DatostomadorWriterService {

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  constructor(
    private httpClient: HttpClient,
    private router: Router,
    private notificationService: NotificationService,
    private cotizacionPersistenceService: CotizacionPersistenceService) { }

  private handleError(err: any): ObservableInput<any> {
    const dialogRef = this.notificationService.showAlert(err.error || "Se presentó un error en la ejecución del proceso, intente nuevamente.");
    dialogRef.afterClosed().subscribe(result => {
      this.router.navigate(['/cotizaciones']);
    });

    return of(false);
  }

  // return GuardarDatosTomadorResponse
  SaveTomador(model: any) {
    let cotizacion = this.cotizacionPersistenceService.cotizacion;

    cotizacion.informacionBasicaTomador.tipoDocumento = model.TipoDocumento;
    cotizacion.informacionBasicaTomador.numeroDocumento = model.NumeroDocumento;
    cotizacion.informacionBasicaTomador.primerNombre = model.PrimerNombre;
    cotizacion.informacionBasicaTomador.segundoNombre = model.SegundoNombre;
    cotizacion.informacionBasicaTomador.primerApellido = model.TipoDocumento.tipoPersona == "J" ? model.RazonSocial: model.PrimerApellido;
    cotizacion.informacionBasicaTomador.segundoApellido = model.SegundoApellido;
    cotizacion.informacionBasicaTomador.actividadEconomica = model.ActividadEconomica;
    cotizacion.informacionBasicaTomador.pais = model.Pais;
    cotizacion.informacionBasicaTomador.departamento = model.Departamento;
    cotizacion.informacionBasicaTomador.municipio = model.Municipio;
    cotizacion.informacionBasicaTomador.direccion = model.Direccion;
    cotizacion.informacionBasicaTomador.email = model.Email;
    cotizacion.informacionBasicaTomador.telefono = model.Telefono;
    cotizacion.informacionBasicaTomador.nombreContacto = model.NombreContacto;
    cotizacion.informacionBasicaTomador.telefonoContacto1 = model.TelefonoContacto1;
    cotizacion.informacionBasicaTomador.telefonoContacto2 = model.TelefonoContacto2;

    var sModel = {
      //CodigoUsuario
      CodigoTipoDocumento: model.TipoDocumento.codigoTipoDocumento,
      NumeroDocumento: model.NumeroDocumento,
      Nombres: `${model.PrimerNombre} ${model.SegundoNombre}`,
      PrimerApellido: model.TipoDocumento.tipoPersona == "J" ? model.RazonSocial: model.PrimerApellido,
      SegundoApellido: model.SegundoApellido,
      CodigoActividad: model.ActividadEconomica.codigoActividadEconomica,
      CodigoPais: model.Pais.codigoPais,
      CodigoDepartamento: model.Departamento.codigoDepartamento,
      CodigoMunicipio: model.Municipio.codigoMunicipio,
      Direccion: model.Direccion,
      Email: model.Email,

      Telefono: model.Telefono,
      NombreContacto: model.NombreContacto || '',
      Telefono1Contacto: model.TelefonoContacto1 || '',
      Telefono2Contacto: model.TelefonoContacto2 || '',
      //Licitacion
      //AseguradoraActual
      //PorcentajeRetorno
      //PorcentajeOtros
      //OtrosGastos
      //DescripcionTipoRiesgo
    }

    var url = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/tomador`;
    const params = new HttpParams().append('version', cotizacion.version.toString());

    return this.httpClient.post(url, sModel, {
      params: params
    })
      .pipe(
        map(res => {
          cotizacion.informacionBasicaTomador.codigoTomador = res['codigoTomador'];
          // complete section
          cotizacion.informacionBasicaTomador.state = CotizacionSectionState.Completed;
          return true;
        }),
        catchError(err => {
          return this.handleError(err);
        })
      );
  }
}
