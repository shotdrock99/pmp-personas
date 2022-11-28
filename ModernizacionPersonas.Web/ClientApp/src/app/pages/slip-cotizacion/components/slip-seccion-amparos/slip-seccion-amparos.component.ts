import { SlipAmparosSection } from '../../../../models/slip';
import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-slip-seccion-amparos',
  templateUrl: './slip-seccion-amparos.component.html',
  styleUrls: ['./slip-seccion-amparos.component.scss']
})
export class SlipSeccionAmparosComponent implements OnInit {

  constructor() { }

  @Input('data')
  model: SlipAmparosSection

  ngOnInit() {

  }

}
