import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ObservableInput, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { environment } from '../../environments/environment';
import { Intermediario } from '../models';
import { NotificationService } from '../shared/services/notification.service';
import { CotizacionPersistenceService } from './cotizacion-persistence.service';

@Injectable({
  providedIn: 'root'
})
export class InformacionintermediariosWriterService {
  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  constructor(private httpClient: HttpClient,
    private router: Router,
    private notificationService: NotificationService,
    private cotizacionPersistenceService: CotizacionPersistenceService) { }

  private handleError(err: any, redirect: boolean = true): ObservableInput<any> {
    const dialogRef = this.notificationService.showAlert(err.error || "Se presentó un error en la ejecución del proceso, intente nuevamente.");
    dialogRef.afterClosed().subscribe(result => {
      if (redirect) {
        this.router.navigate(['/cotizaciones']);
      }
    });

    return of(false);
  }

  crearIntermediario(intermediario: Intermediario) {
    const cotizacion = this.cotizacionPersistenceService.cotizacion;
    const body = {
      CodigoCotizacion: cotizacion.codigoCotizacion,
      Participacion: intermediario.PorcentajeParticipacion,
      Clave: intermediario.Clave,
      PrimerNombre: intermediario.PrimerNombre,
      SegundoNombre: intermediario.SegundoNombre,
      PrimerApellido: intermediario.PrimerApellido,
      SegundoApellido: intermediario.SegundoApellido,
      CodigoTipoDocumento: intermediario.TipoDocumento.codigoTipoDocumento,
      NumeroDocumento: intermediario.NumeroDocumento,
      CodigoEstado: intermediario.Estado,
      Email: intermediario.Email
    };

    const url = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/intermediarios`;
    const params = new HttpParams().append('version', cotizacion.version.toString());

    return this.httpClient.post(url, body)
      .pipe(
        map(res => res),
        catchError(err => {
          return this.handleError(err, false);
        })
      );
  }

  actualizarIntermediario(intermediario: Intermediario) {
    const cotizacion = this.cotizacionPersistenceService.cotizacion;
    let body = {
      Codigo: intermediario.Codigo,
      CodigoCotizacion: cotizacion.codigoCotizacion,
      Participacion: intermediario.PorcentajeParticipacion,
      Clave: intermediario.Clave,
      PrimerNombre: intermediario.PrimerNombre,
      SegundoNombre: intermediario.SegundoNombre,
      PrimerApellido: intermediario.PrimerApellido,
      SegundoApellido: intermediario.SegundoApellido,
      CodigoTipoDocumento: intermediario.TipoDocumento.codigoTipoDocumento,
      NumeroDocumento: intermediario.NumeroDocumento,
      CodigoEstado: intermediario.Estado,
      Email: intermediario.Email
    };

    const url = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/intermediarios/${intermediario.Codigo}`;
    const params = new HttpParams().append('version', cotizacion.version.toString());

    return this.httpClient.patch(url, body)
      .pipe(
        map(res => res),
        catchError(err => {
          return this.handleError(err);
        })
      );
  }

  eliminarIntermediario(codigo: any) {
    const cotizacion = this.cotizacionPersistenceService.cotizacion;
    const url = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/intermediarios/${codigo}`;

    return this.httpClient.delete(url)
      .pipe(
        map(res => res),
        catchError(err => {
          return this.handleError(err);
        })
      );
  }
}
