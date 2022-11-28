import { SlipCondicionesSection } from '../../../../models/slip';
import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-slip-seccion-condiciones',
  template: '' +
    '<div class="main-container" *ngIf="model.secciones.length>0">' +
    ' <h4>{{model.tituloSeccion}}</h4>' +
    ' <hr />' +
    ' <textarea [innerHTML]="model.secciones" style="resize:none;border:none;width:54em;height:25em;text-align:justify" [disabled]="true"></textarea>' +
    '</div>'
})
export class SlipSeccionCondicionesComponent implements OnInit {

  constructor() { }

  @Input('data')
  model: SlipCondicionesSection

  ngOnInit() {
  }

}
