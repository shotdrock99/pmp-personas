import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { EmailParametrizacion } from '../models/email-parametrizacion';

@Injectable({
    providedIn: 'root'
})

export class EmailParametrizacionWriterService {

    private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas`;

    constructor(private httpClient: HttpClient) {}

    editTextoEmail(email: EmailParametrizacion){
        const url = `${this.BASE_URL}/emails/${email.codigoTemplate}`
        return this.httpClient.put<any>(url, email);
    }

}