import { Component, OnInit, Input } from '@angular/core';
import { InformacionSiniestralidad } from 'src/app/models/fichatecnica';

@Component({
  selector: 'app-ficha-tecnica-siniestralidad',
  templateUrl: './ficha-tecnica-siniestralidad.component.html',
  styleUrls: ['./ficha-tecnica-siniestralidad.component.scss']
})
export class FichaTecnicaSiniestralidadComponent implements OnInit {

  constructor() { }

  @Input()
  model: InformacionSiniestralidad;

  ngOnInit() {
  }
}
