import { Component, OnInit, Input } from '@angular/core';
import { PerfilValores } from 'src/app/models/fichatecnica';

@Component({
  selector: 'app-ficha-tecnica-perfil-valores',
  templateUrl: './ficha-tecnica-perfil-valores.component.html',
  styleUrls: ['./ficha-tecnica-perfil-valores.component.scss']
})
export class FichaTecnicaPerfilValoresComponent implements OnInit {

  constructor() { }

  @Input()
  model: PerfilValores;

  ngOnInit() {
  }

}
