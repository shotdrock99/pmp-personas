import { Component, OnInit, Input } from '@angular/core';
import { SlipClausulasSection } from '../../../../models/slip';

@Component({
  selector: 'app-slip-seccion-clausulas',
  templateUrl: './slip-seccion-clausulas.component.html',
  styleUrls: ['./slip-seccion-clausulas.component.scss']
})
export class SlipSeccionClausulasComponent implements OnInit {

  constructor() { }

  @Input('data')
  model: SlipClausulasSection

  ngOnInit() {
  }

}
