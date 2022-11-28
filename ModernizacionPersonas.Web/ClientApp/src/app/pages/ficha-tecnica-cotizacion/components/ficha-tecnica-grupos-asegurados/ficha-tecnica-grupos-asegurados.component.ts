import { Component, OnInit, Input } from '@angular/core';
import { TipoTasa } from 'src/app/models';
import { GrupoAseguradoFichaTecnica } from 'src/app/models/fichatecnica';

@Component({
  selector: 'app-ficha-tecnica-grupos-asegurados',
  templateUrl: './ficha-tecnica-grupos-asegurados.component.html',
  styleUrls: ['./ficha-tecnica-grupos-asegurados.component.scss']
})
export class FichaTecnicaGruposAseguradosComponent implements OnInit {

  constructor() { }

  @Input()
  tipoTasa1: TipoTasa;

  @Input()
  model: GrupoAseguradoFichaTecnica[];

  @Input()
  tieneSiniestralidad: boolean;

  get mostrarNumeroAseguradosPotencial(): boolean {
    return this.tieneSiniestralidad || this.tipoTasa1.codigoTasa === 4;
  }

  get mostrarPorcentajeEsperado(): boolean {
    return this.tieneSiniestralidad || this.tipoTasa1.codigoTasa === 4;
  }

  ngOnInit() {

  }

  showAsistenciaField(grupo: GrupoAseguradoFichaTecnica) {
    const result = (grupo.tipoSumaAsegurada.codigoTipoSumaAsegurada !== 3
      && grupo.tipoSumaAsegurada.codigoTipoSumaAsegurada !== 1) && grupo.asistencias.length > 0;
  }
}
