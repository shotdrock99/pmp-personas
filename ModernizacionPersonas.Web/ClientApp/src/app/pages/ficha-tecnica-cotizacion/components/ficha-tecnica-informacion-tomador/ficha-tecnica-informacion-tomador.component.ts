import { Component, OnInit, Input } from '@angular/core';
import { InformacionTomador } from 'src/app/models/fichatecnica';

@Component({
  selector: 'app-ficha-tecnica-informacion-tomador',
  templateUrl: './ficha-tecnica-informacion-tomador.component.html',
  styleUrls: ['./ficha-tecnica-informacion-tomador.component.scss']
})
export class FichaTecnicaInformacionTomadorComponent implements OnInit {

  constructor() { }

  @Input()
  model: InformacionTomador;

  ngOnInit() {
  }

}
