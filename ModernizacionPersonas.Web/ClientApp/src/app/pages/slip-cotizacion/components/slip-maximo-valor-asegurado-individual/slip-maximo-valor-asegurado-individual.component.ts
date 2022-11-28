import { Component, OnInit, Input } from '@angular/core';
import { ValorMaximoAseguradoIndividualSlip } from 'src/app/models/slip';

@Component({
  selector: 'app-slip-maximo-valor-asegurado-individual',
  templateUrl: './slip-maximo-valor-asegurado-individual.component.html',
  styleUrls: ['./slip-maximo-valor-asegurado-individual.component.scss']
})
export class SlipMaximoValorAseguradoIndividualComponent implements OnInit {

  @Input() data: ValorMaximoAseguradoIndividualSlip;

  constructor() { }

  ngOnInit() {
  }

}
