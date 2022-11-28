import { Component, OnInit, Input, Pipe, PipeTransform } from '@angular/core';
import { PerfilEdades, Amparo, RangoEdad } from 'src/app/models/fichatecnica';

@Component({
  selector: 'app-ficha-tecnica-perfil-edades',
  templateUrl: './ficha-tecnica-perfil-edades.component.html',
  styleUrls: ['./ficha-tecnica-perfil-edades.component.scss']
})
export class FichaTecnicaPerfilEdadesComponent implements OnInit {

  constructor() { }

  @Input()
  model: PerfilEdades;

  ngOnInit() {
  }
}
