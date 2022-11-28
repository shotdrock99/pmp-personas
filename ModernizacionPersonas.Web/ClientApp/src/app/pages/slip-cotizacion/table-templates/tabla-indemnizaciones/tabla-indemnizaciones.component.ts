import { Component, OnInit, Input } from '@angular/core';
import { TableItem } from '../../../../models/tableitem';

@Component({
  selector: 'app-tabla-indemnizaciones',
  templateUrl: './tabla-indemnizaciones.component.html',
  styleUrls: ['./tabla-indemnizaciones.component.scss']
})
export class TablaIndemnizacionesComponent implements OnInit {

  constructor() { }

  data: TableItem[];
  @Input('data') codAmparo: string;
  ngOnInit() {
    this.data = [{
      index: 1,
      descripcion: 'Pérdida total e irreparable de ambos ojos',
      valor: 100
    }, {
      index: 2,
      descripcion: 'Pérdida total e irreparable de ambas manos o ambos pies',
      valor: 100
    }, {
      index: 3,
      descripcion: 'Sordera Total bilateral',
      valor: 100
    }, {
      index: 4,
      descripcion: 'Perdida del habla',
      valor: 100
    }, {
      index: 5,
      descripcion: 'Pérdida o inutilización de una mano y de un pie',
      valor: 100
    }, {
      index: 6,
      descripcion: 'Pérdida o inutilización de una mano o un pie y la visión de un ojo',
      valor: 100
    }, {
      index: 7,
      descripcion: 'Pérdida o inutilización del brazo de la mano derecha',
      valor: 60
    }, {
      index: 8,
      descripcion: 'Perdida completa de la visión de un ojo',
      valor: 60
    }, {
      index: 9,
      descripcion: 'Sordera Total Unilateral',
      valor: 50
    }, {
      index: 10,
      descripcion: 'Pérdida o inutilización de una sola mano o de un pie',
      valor: 60
    }, {
      index: 11,
      descripcion: 'Pérdida o inutilización de una pierna por encima de la rodilla',
      valor: 50
    }, {
      index: 12,
      descripcion: 'Pérdida completa o inutilización del uso de la cadera',
      valor: 30
    }, {
      index: 13,
      descripcion: 'Pérdida o inutilización del dedo pulgar derecho',
      valor: 25
    }, {
      index: 14,
      descripcion: 'Pérdida total o inutilización de tres dedos de la mano derecha o el pulgar y otro dedo que no sea el indice',
      valor: 20
    }, {
      index: 15,
      descripcion: 'Pérdida o inutilización de una mano',
      valor: 20
    }, {
      index: 16,
      descripcion: 'Pérdida o inutilización del dedo pulgar izquierdo',
      valor: 20
    }, {
      index: 17,
      descripcion: 'Pérdida completa del uso de la muñeca o del codo derecho',
      valor: 20
    }, {
      index: 18,
      descripcion: 'Pérdida completa del uso de algúna rodilla',
      valor: 20
    }, {
      index: 19,
      descripcion: 'Fractura no consolidada de una rodilla',
      valor: 20
    }, {
      index: 20,
      descripcion: 'Pérdida o inutilización del dedo indice derecho',
      valor: 15
    }, {
      index: 21,
      descripcion: 'Pérdida completa del uso de la muñeca o del codo izquierdo',
      valor: 15
    }, {
      index: 22,
      descripcion: 'Pérdida completa del uso del tobillo',
      valor: 15
    }, {
      index: 23,
      descripcion: 'Pérdida o inutilización del dedo indice izquierdo',
      valor: 15
    }, {
      index: 24,
      descripcion: 'Pérdida o inutilización de uno cualquiera de los restantes dedos de la mano o de los pies, siempre que comprenda la totalidad de las falanges de cada uno',
      valor: 10
    }];
  }
}
