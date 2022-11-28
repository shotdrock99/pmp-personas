import { HttpClient, HttpEventType, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ObservableInput, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

import { GrupoAsegurado } from '../models';
import { NotificationService } from '../shared/services/notification.service';
import { CotizacionPersistenceService } from './cotizacion-persistence.service';

@Injectable({
  providedIn: 'root'
})
export class GruposAseguradosWriterService {
  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  constructor(
    private httpClient: HttpClient,
    private notificationService: NotificationService,
    private cotizacionDataService: CotizacionPersistenceService) { }

  private handleError(err: HttpErrorResponse): ObservableInput<any> {
    let message = 'Se presentó un error guardando la información, intente nuevamente.';
    if (err.error.type === 'DatosGrupoAseguradoTableWriter :: ActualizarGrupoAseguradoAsync') {
      message = 'Uno de los valores configurados en el grupo desborda los valores permitidos por el sistema';
    }
    const dialogRef = this.notificationService.showAlert(message);
    dialogRef.afterClosed().subscribe(result => {
      // this.router.navigate(['/cotizaciones'])
    });

    return of(false);
  }

  async crearGrupoAsegurados(model: GrupoAsegurado) {
    const cotizacion = this.cotizacionDataService.cotizacion;
    const body = {
      CodigoCotizacion: cotizacion.codigoCotizacion,
      CodigoTipoSuma: model.tipoSumaAsegurada.codigoTipoSumaAsegurada,
      NombreGrupoAsegurado: model.nombreGrupoAsegurado,
      ValorMinAsegurado: Number(model.valorMinAsegurado) || 0,
      ValorMaxAsegurado: Number(model.valorMaxAsegurado) || 0
    };

    const url = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/gruposasegurados`;
    const params = new HttpParams().append('version', cotizacion.version.toString());

    return new Promise(resolve => {
      this.httpClient.post(url, body, {
        params,
        reportProgress: true,
        observe: 'events'
      }).subscribe(events => {
        if (events.type === HttpEventType.UploadProgress) {
          // show progress
        } else if (events.type === HttpEventType.Response) {
          const response: any = events.body;
          resolve(response);
        }
      });
    });
  }

  updateGrupoAsegurado(body: any) {
    const cotizacion = this.cotizacionDataService.cotizacion;
    const url = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/gruposAsegurados/${body.codigoGrupoAsegurado}`;
    const params = new HttpParams().append('version', cotizacion.version.toString());

    return this.httpClient.patch(url, body, { params })
      .pipe(
        map(res => res),
        catchError(err => {
          return this.handleError(err);
        })
      );
  }

  async removeGrupoAseguradoAsync(codigoGrupoAsegurado: number) {
    const cotizacion = this.cotizacionDataService.cotizacion;
    const url = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/gruposAsegurados/${codigoGrupoAsegurado}`;
    const params = new HttpParams().append('version', cotizacion.version.toString());

    return new Promise(resolve => {
      this.httpClient.delete(url, {
        // params: params,
        reportProgress: true,
        observe: 'events'
      }).subscribe(events => {
        if (events.type === HttpEventType.UploadProgress) {
          // show progress
        } else if (events.type === HttpEventType.Response) {
          const response: any = events.body;
          resolve(response);
        }
      });
    });
  }

  deleteAseguradosAsync(codigoGrupoAsegurado: number): any {
    const cotizacion = this.cotizacionDataService.cotizacion;
    const url = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/gruposAsegurados/${codigoGrupoAsegurado}/asegurados`;
    const params = new HttpParams().append('version', cotizacion.version.toString());

    return new Promise(resolve => {
      this.httpClient.delete(url, {
        // params: params,
        reportProgress: true,
        observe: 'events'
      }).subscribe(events => {
        if (events.type === HttpEventType.UploadProgress) {
          // show progress
        } else if (events.type === HttpEventType.Response) {
          const response: any = events.body;
          resolve(response);
        }
      });
    });
  }
}
