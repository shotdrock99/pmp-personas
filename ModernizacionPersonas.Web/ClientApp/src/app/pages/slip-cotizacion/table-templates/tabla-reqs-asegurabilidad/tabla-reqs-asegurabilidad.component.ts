import { EdadAsegurabilidad } from './../../../../models/edad-asegurabilidad';
import { SlipClausula } from './../../../../models/slip-clausula';
import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-tabla-reqs-asegurabilidad',
  templateUrl: './tabla-reqs-asegurabilidad.component.html',
  styleUrls: ['./tabla-reqs-asegurabilidad.component.scss']
})
export class TablaReqsAsegurabilidadComponent implements OnInit {
  maxAge: any;
  maxValueA: number;
  maxValueB: number;
  maxValueC: number;

  constructor() { }

  @Input('data')
  data: EdadAsegurabilidad[];

  ngOnInit() {
    const arr = this.data.map(x => x.edadHasta);
    this.maxAge = Math.max(...arr);
    if(this.data.length > 0){
      this.maxValueA = this.data.find(x => x.requisitos === 'A') == undefined ? 0 : (this.data.find(x => x.requisitos === 'A').valorIndividualHasta);
      this.maxValueB = this.data.find(x => x.requisitos === 'B') == undefined ? 0 : (this.data.find(x => x.requisitos === 'B').valorIndividualHasta);
      this.maxValueC = this.data.find(x => x.requisitos === 'C') == undefined ? 0 : (this.data.find(x => x.requisitos === 'C').valorIndividualHasta);
    }else{
      this.maxValueA =  0;
      this.maxValueB =  0;
      this.maxValueC =  0;
    }
   
  }
}
