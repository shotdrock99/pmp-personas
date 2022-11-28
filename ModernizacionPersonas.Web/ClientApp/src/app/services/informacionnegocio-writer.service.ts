import { HttpClient, HttpEventType, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

import { CotizacionPersistenceService } from './cotizacion-persistence.service';
import { map, catchError } from 'rxjs/operators';
import { NotificationService } from '../shared/services/notification.service';
import { ObservableInput, of } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class InformacionnegocioWriterService {
  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  constructor(
    private httpClient: HttpClient,
    private router: Router,
    private notificationService: NotificationService,
    private cotizacionPersistenceService: CotizacionPersistenceService) { }

  private handleError(err: any): ObservableInput<any> {
    const dialogRef = this.notificationService.showAlert(
      err.error || 'Se presentó un error en la ejecución del proceso, intente nuevamente.');
    dialogRef.afterClosed().subscribe(result => {
      this.router.navigate(['/cotizaciones']);
    });

    return of(false);
  }

  SaveInformacionNegocio(model: any) {
    const cotizacion = this.cotizacionPersistenceService.cotizacion;
    const sModel = {
      nombreAseguradora: model.NombreAseguradora,
      fechaInicio: model.FechaInicio,
      fechaFin: model.FechaFin,
      codigoPeriodoFacturacion: model.PeriodoFacturacion.codigoPeriodo,
      codigoTipoRiesgo: model.TipoRiesgo,
      codigoSector: model.Sector,
      codigoTipoNegocio: model.TipoNegocio,
      codigoTipoContratacion: model.TipoContratacion,
      codigoTipoTasa1: model.TipoTasa1,
      codigoTipoTasa2: model.TipoTasa2 || 0,
      AnyosSiniestralidad: model.AnyosSiniestralidad || 0,
      porcentajeRetorno: model.PorcentajeRetorno || 0,
      porcentajeOtrosGastos: model.PorcentajeOtrosGastos || 0,
      otrosGastos: model.OtrosGastos,
      porcentajeComision: model.PorcentajeComision || 0,
      esNegocioDirecto: model.EsNegocioDirecto || false,
      conListaAsegurados: model.ConListaAsegurados || false,
      perfilEdad: model.PerfilEdad || 0,
      perfilValor: model.PerfilValor || 0,
      usuarioDirectorComercial:  model.DirectorComercial== undefined ? null : model.DirectorComercial.loginUsuario,
      nombreDirectorComercial: model.DirectorComercial== undefined ? null :  model.DirectorComercial.nombreUsuario,
      emailDirectorComercial: model.DirectorComercial== undefined ? null : model.DirectorComercial.emailUsuario,
      actividad: model.Actividad
    };
    const params = new HttpParams().append('version', cotizacion.version.toString());
    const url = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/informacionNegocio`;
    return this.httpClient.post(url, sModel, {
      params,
      reportProgress: true,
      observe: 'events'
    }).pipe(
      map(res => {
        // complete section
        // cotizacion.updateInformacionNegocio(model);
        cotizacion.informacionNegocio.sector = model.Sector;
        return true;
      }),
      catchError(err => {
        return this.handleError(err);
      })
    );
  }
}
