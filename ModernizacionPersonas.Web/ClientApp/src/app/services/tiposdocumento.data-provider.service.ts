import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { TipoDocumento } from '../models';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class TiposDocumentoProviderService {

  private observableCache: Observable<TipoDocumento[]>;
  private tiposDocumentoCache: TipoDocumento[];

  constructor(private httpClient: HttpClient) { }

  getTiposDocumento() {
    if (this.tiposDocumentoCache) return of(this.tiposDocumentoCache);
    else if (this.observableCache) return of(this.observableCache);
    else this.observableCache = this.fetchData();

    return this.observableCache;

  }
  private fetchData():Observable<TipoDocumento[]> {
    let url = String(`api/parametrizacion/tipodocumento`);

    return this.httpClient.get<TipoDocumento>(url)
      .pipe(map(response => {
        let array = JSON.parse(response.toString()) as any[];
        let details = array.map(data => new TipoDocumento());
        return details;
      }));
  }
  private mapCachedTiposDocumento(body: TipoDocumento[]) {
    this.observableCache = null;
    //this.tiposDocumentoCache = new
  }
}
