import { AuthenticationService } from 'src/app/services/authentication.service';
import { Component, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {

  @Output()
  toggleSidenavEmitter = new EventEmitter();

  constructor(private authenticationService: AuthenticationService) { }

  items: NavItem[];

  ngOnInit(): void {
    this.authenticationService.getMenu()
      .subscribe((res: any) => {
        //var data = JSON.parse(res);
        this.items = res.items;
      });
  }

  toggleSideNav(e) {
    this.toggleSidenavEmitter.emit(e);
  }
}

const MENU = [{
  name: 'cotizaciones',
  icon: 'view_list',
  text: 'Cotizaciones',
  itemsCount: 2,
  items: [{
    name: 'newcotizacion',
    icon: 'add',
    routerLink: '/cotizaciones/nueva',
    text: 'Nueva Cotización'
  }, {
    name: 'listacotizaciones',
    icon: 'list',
    routerLink: '/cotizaciones',
    text: 'Lista de Cotizaciones'
  }]
}, {
  name: 'autorizaciones',
  icon: 'security',
  routerLink: '/autorizaciones',
  text: 'Autorizaciones',
  itemsCount: 0
}, {
  name: 'admin',
  icon: 'settings',
  routerLink: '/admin',
  text: 'Administración',
  itemsCount: 0
}];

export class NavItem {
  name: string;
  icon: string;
  routerLink?: string;
  text: string;
  itemsCount?: number;
  items?: NavItem[];
}
