import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ApplicationUser } from 'src/app/models/application-user';
import { Location } from '@angular/common';

@Component({
  selector: 'app-user-menu',
  templateUrl: './user-menu.component.html',
  styleUrls: ['./user-menu.component.scss']
})
export class UserMenuComponent implements OnInit {

  constructor(private authenticationService: AuthenticationService, private loc: Location) { }

  public get user(): ApplicationUser {
    return this.authenticationService.currentUserValue;
  }
public permitLogout : boolean = true;
  ngOnInit() {
    const angularRoute = this.loc.path();
  const url = window.location.href;

  var domainAndApp = url.replace(angularRoute, '');
  if(domainAndApp == "solitest.com.co" || domainAndApp == "https://www.solitest.com.co/PMP"|| domainAndApp == "https://www.solidaria.com.co/PMP"){
    this.permitLogout = false;
  }
  }

  public logout() {
    this.authenticationService.logout();
    const url = environment.appSettings.BASEHREF + '/login';
    window.location.href = url;
  }
}
