import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ObservableInput, of } from 'rxjs';
import { Router } from '@angular/router';
import { CotizacionApiRouterService } from 'src/app/services/cotizacion-api-router.service';
import { environment } from 'src/environments/environment';
import { catchError, map } from 'rxjs/operators';
import { NotificationService } from '../shared/services/notification.service';
import { GenerarSlipConfiguracionResponse } from '../models/slip-configuracion';

@Injectable({
  providedIn: 'root'
})
export class ConfiguracionSlipReaderService {

  constructor(private httpClient: HttpClient,private router: Router,private notificationService: NotificationService) { }

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas/cotizaciones`;

  private handleError(err: any): ObservableInput<any> {
    const dialogRef = this.notificationService.showAlert("Se ha presentado un error, por favor contacte al administrador");
    dialogRef.afterClosed().subscribe((result) => {
      this.router.navigate(["/cotizaciones"]);
    });

    return of(false);
  }
  obtenerConfiguracion(codigoCotizacion: number) {
    const url = `${this.BASE_URL}/${codigoCotizacion}/slip/configuracion`;
    let result = this.httpClient.get<GenerarSlipConfiguracionResponse>(url).pipe(
      map(res => res),
      catchError(err => {
        return this.handleError(err);
      })
    );;
    return result;
  }
}
