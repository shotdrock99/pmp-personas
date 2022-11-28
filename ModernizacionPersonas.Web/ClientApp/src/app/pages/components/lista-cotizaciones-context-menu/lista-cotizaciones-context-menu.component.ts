import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-lista-cotizaciones-context-menu',
  templateUrl: './lista-cotizaciones-context-menu.component.html',
  styleUrls: ['./lista-cotizaciones-context-menu.component.scss']
})
export class ListaCotizacionesContextMenuComponent implements OnInit {

  constructor() { }

  @Input('data')
  menuItems: any

  ngOnInit() {
  }

  itemClick(event: Event, item: any) {
    item.onClick(event);
  }
}
