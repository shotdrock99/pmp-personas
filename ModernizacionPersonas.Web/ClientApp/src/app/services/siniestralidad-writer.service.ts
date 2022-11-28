import { catchError, map } from 'rxjs/operators';
import { HttpClient, HttpEventType, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CotizacionPersistenceService } from 'src/app/services/cotizacion-persistence.service';
import { environment } from 'src/environments/environment';

import {
  RegistroSiniestralidad,
  SiniestralidadModel,
} from '../pages/configuracion-cotizacion/informacion-siniestralidad/informacion-siniestralidad.component';
import { Router } from '@angular/router';
import { NotificationService } from '../shared/services/notification.service';
import { ObservableInput, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SiniestralidadWriterService {
  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  constructor(private httpClient: HttpClient,
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

  Write(model: SiniestralidadModel) {
    const cotizacion = this.cotizacionPersistenceService.cotizacion;

    let sModel: any[] = [];
    let keys = Object.keys(model);
    keys.forEach(x => {
      let opcion: RegistroSiniestralidad = model[x];
      if (typeof opcion === 'object') {
        sModel.push({
          CodigoSiniestralidad: 0,
          CodigoCotizacion: 1,
          Anno: opcion.anno,
          FechaInicial: opcion.fechaInicial,
          FechaFinal: opcion.fechaFinal,
          ValorIncurrido: opcion.valorIncurrido,
          NumeroCasos: opcion.numeroCasos
        });
      }
    })

    var url = `${this.BASE_URL}/${cotizacion.codigoCotizacion}/siniestralidad`;
    const params = new HttpParams().append('version', cotizacion.version.toString());
    let args = { Data: sModel };
    return this.httpClient.post(url, args, {
      params: params,
      reportProgress: true,
      observe: 'events'
    }).pipe(
      map(res => {
        cotizacion.updateInformacionSiniestralidad(model);
        return true;
      }),
      catchError(err => {
        return this.handleError(err);
      })
    );
  }
}
