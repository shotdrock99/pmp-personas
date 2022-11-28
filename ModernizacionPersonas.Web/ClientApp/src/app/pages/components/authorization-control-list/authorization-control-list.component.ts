import { Component, OnInit, Input } from '@angular/core';
import { CotizacionAuthorization } from 'src/app/models/cotizacion-authorization';

@Component({
  selector: 'app-authorization-control-list',
  templateUrl: './authorization-control-list.component.html',
  styleUrls: ['./authorization-control-list.component.scss']
})
export class AuthorizationControlListComponent implements OnInit {

  constructor() { }

  @Input() data: CotizacionAuthorization[];

  ngOnInit() {

    
  }

}
