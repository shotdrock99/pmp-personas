import { SlipGruposAsegurados } from '../../../../models/slip';
import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-slip-grupos-asegurados',
  templateUrl: './slip-grupos-asegurados.component.html',
  styleUrls: ['./slip-grupos-asegurados.component.scss']
})
export class SlipGruposAseguradosComponent implements OnInit {

  constructor() { }

  @Input('data') model: SlipGruposAsegurados;

  ngOnInit() {

  }

}
