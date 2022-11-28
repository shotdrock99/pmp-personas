import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BehaviorSubject, Observable } from 'rxjs';
import { ApplicationUser } from '../models/application-user';
import { environment } from 'src/environments/environment';
import { Util } from 'src/app/services/util';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private currentUserSubject: BehaviorSubject<ApplicationUser>;
  public currentUser: Observable<ApplicationUser>;

  private BASE_URL = `${environment.appSettings.API_ENDPOINT}/auth`;

  constructor(private httpClient: HttpClient, private consumeService : Util) {
    let str = localStorage.getItem('currentUser');
    let json = JSON.parse(str);
    let c1 = this.toCamel(json);

    this.currentUserSubject = new BehaviorSubject<ApplicationUser>(c1);
    this.currentUser = this.currentUserSubject.asObservable();
  }

  private toCamel(o) {
    var newO, origKey, newKey, value
    if (o instanceof Array) {
      var _this = this;
      return o.map(function (value) {
        if (typeof value === "object") {
          value = _this.toCamel(value)
        }
        return value
      })
    } else {
      newO = {}
      for (origKey in o) {
        if (o.hasOwnProperty(origKey)) {
          newKey = (origKey.charAt(0).toLowerCase() + origKey.slice(1) || origKey).toString()
          value = o[origKey]
          if (value instanceof Array || (value !== null && value.constructor === Object)) {
            value = this.toCamel(value)
          }

          newO[newKey] = value
        }
      }
    }
    return newO
  }

  public get currentUserValue(): ApplicationUser {
    return this.currentUserSubject.value;
  }

  getMenu() {
    const url = `${this.BASE_URL}/menu`;
    return this.httpClient.get(url);
  }
  getMenu2() {
    const bodyReq = {
      token: localStorage.getItem('token'),
      url: `${this.BASE_URL}/menu`,
      method: 'GET'
    };
    return this.consumeService.consumeService(JSON.stringify(bodyReq));
  }


  login(username: string, password: string) {
    
    return this.httpClient.post<any>('/login', { username, password })
      .pipe(map(res => {
        
        // login successful if there's a jwt token in the response
        if (res && res.token) {
          // store user details and jwt token in local storage to keep user logged in between page refreshes
          localStorage.setItem('token', JSON.stringify(res.token));
          this.currentUserSubject.next(res.userName);
        }

        return res;
      }));
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('currentUser');
  }
}
