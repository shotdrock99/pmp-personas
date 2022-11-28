import { UsuarioPersonas } from './../models/usuario-personas';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})

export class UsersWriterService {

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/personas`;

  constructor(private httpClient: HttpClient) { }

  disableUser(userId: number) {
    const url = `${this.BASE_URL}/users/${userId}`
    return this.httpClient.delete(url);
  }

  updateUser(usuario: UsuarioPersonas){
    const url = `${this.BASE_URL}/users/${usuario.userId}`
    return this.httpClient.put(url, usuario);
  }

  createUser(usuario: UsuarioPersonas){
    const url = `${this.BASE_URL}/users`
    return this.httpClient.post(url, usuario);
  }

  createIntermediario(usuario: UsuarioPersonas){
    const url = `${this.BASE_URL}/users/intermediario`
    return this.httpClient.post(url, usuario);
  }

}
