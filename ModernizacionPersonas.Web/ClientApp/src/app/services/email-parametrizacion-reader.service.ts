import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})

export class EmailParametrizacionReaderService {

    private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas`;

    constructor(private httpClient: HttpClient) {}

    getTextosEmailByTemplate(codigoTemplate: number, codigoSeccion: number){
        const url = `${this.BASE_URL}/emails/${codigoTemplate}/${codigoSeccion}`
        return this.httpClient.get<any>(url);
    }

}