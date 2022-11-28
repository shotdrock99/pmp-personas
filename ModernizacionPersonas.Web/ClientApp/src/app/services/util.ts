import { HttpClient, HttpEventType, HttpParams, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { SlipConfiguracion } from '../models/slip-configuracion';
import { EdadAsegurabilidad } from './../models/edad-asegurabilidad';
import { CotizacionPersistenceService } from './cotizacion-persistence.service';
import { map, catchError } from 'rxjs/operators';
import { NotificationService } from '../shared/services/notification.service';
import { ObservableInput, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class Util {
  constructor(private httpClient: HttpClient, private notificationService: NotificationService) {
  }
  public consumeService2(body: string): Promise<any> {
    const url = document.baseURI + 'api/main/rest';
    const headers = new Headers();
    headers.append('Allow', '*');
    headers.append('Accept', 'application/json');
    headers.append('Content-Type', 'application/json');
    return fetch(url, {
      body: body,
      headers: headers,
      method: 'POST'
    });
  }
  public consumeService(body: string) {
    const url = document.baseURI + 'api/main/rest';
    const headers = new HttpHeaders({ 'Allow': '*', 'Accept': 'application/json', 'Content-Type': 'application/json' });
    const headers2 = new Headers();
    headers.append('Allow', '*');
    headers.append('Accept', 'application/json');
    headers.append('Content-Type', 'application/json');
    return this.httpClient.post<any>(url, body, {headers: headers})
    .pipe(
      map(res => res),
      catchError(err => {
        return this.handleError(err);
      })
    )
  }
  public replaceSauareBracket(id: string, value: string, replace: any) {
    const parent = `\\{${id}\\}`;
    const reg = RegExp(parent);
    return value.replace(reg, replace);
  }

  private handleError(err: any): ObservableInput<any> {
    const dialogRef = this.notificationService.showAlert(err.error);
    dialogRef.afterClosed().subscribe(result => {

    });

    return of(false);
  }
}
