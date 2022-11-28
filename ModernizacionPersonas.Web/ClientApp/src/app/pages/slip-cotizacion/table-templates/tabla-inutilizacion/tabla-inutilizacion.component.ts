import { Component, OnInit, Input } from '@angular/core';
import { TableItem } from '../../../../models/tableitem';

@Component({
  selector: 'app-tabla-inutilizacion',
  templateUrl: './tabla-inutilizacion.component.html',
  styleUrls: ['./tabla-inutilizacion.component.scss']
})
export class TablaInutilizacionComponent implements OnInit {

  constructor() { }

  data: TableItem[];
  @Input('data') codAmparo: string;

  ngOnInit() {
    this.data = [
      {
        index: 1,
        descripcion: 'Enajenación mental incurable con impotencia funcional absoluta',
        valor: 100
      },
      {
        index: 2,
        descripcion: 'Parálisis o Invalidez Total y Permanente',
        valor: 100
      },
      {
        index: 3,
        descripcion: 'Ceguera completa en ambos ojos',
        valor: 100
      },
      {
        index: 4,
        descripcion: 'Pérdida total e irreparable de ambas manos o ambos pies',
        valor: 100
      },
      {
        index: 5,
        descripcion: 'Sordera Total bilateral',
        valor: 100
      },
      {
        index: 6,
        descripcion: 'Perdida del habla',
        valor: 100
      },
      {
        index: 7,
        descripcion: 'Perdida del brazo o de la mano derecha',
        valor: 60
      },
      {
        index: 8,
        descripcion: 'Perdida completa de la visión de un ojo',
        valor: 50
      },
      {
        index: 9,
        descripcion: 'Sordera Total Unilateral',
        valor: 50
      },
      {
        index: 10,
        descripcion: 'Perdida del Brazo o de la Mano Izquierda',
        valor: 50
      },
      {
        index: 11,
        descripcion: 'Perdida de una pierna por encima de la rodilla',
        valor: 50
      },
      {
        index: 12,
        descripcion: 'Perdida de un pie',
        valor: 40
      },
      {
        index: 13,
        descripcion: 'Perdida completa del uso de la cadera',
        valor: 30
      },
      {
        index: 14,
        descripcion: 'Fractura no consolidada de una pierna',
        valor: 30
      },
      {
        index: 15,
        descripcion: 'Perdida del dedo pulgar izquierdo',
        valor: 25
      },
      {
        index: 16,
        descripcion: 'Pérdida Total de tres dedos de la mano derecha o él pulgar y otro dedo que sea el índice',
        valor: 25
      },
      {
        index: 17,
        descripcion: 'Perdida completa del uso del hombro derecho',
        valor: 25
      },
      {
        index: 18,
        descripcion: 'Trastornos en la masticación y el habla',
        valor: 25
      },
      {
        index: 19,
        descripcion: 'Perdida del dedo pulgar izquierdo',
        valor: 20
      },
      {
        index: 20,
        descripcion: 'Perdida Total de tres dedos de la mano izquierda o él pulgar y otro dedo que no sea el índice',
        valor: 20
      },
      {
        index: 21,
        descripcion: 'Perdida completa del uso de la muñeca o del codo derecho',
        valor: 20
      },
      {
        index: 22,
        descripcion: 'Perdida completa del uso de alguna rodilla',
        valor: 20
      },
      {
        index: 23,
        descripcion: 'Fractura no consolidada de una rodilla',
        valor: 20
      },
      {
        index: 24,
        descripcion: 'Perdida del dedo índice derecho',
        valor: 15
      },
      {
        index: 25,
        descripcion: 'Perdida completa del uso de la muñeca o del codo Izquierdo',
        valor: 15
      },
      {
        index: 26,
        descripcion: 'Perdida completa del uso del tobillo',
        valor: 15
      },
      {
        index: 27,
        descripcion: 'Perdida del dedo índice izquierdo',
        valor: 12
      },
      {
        index: 28,
        descripcion: 'Perdida del dedo anular derecho',
        valor: 10
      },
      {
        index: 29,
        descripcion: 'Perdida del dedo medio derecho',
        valor: 10
      },
      {
        index: 30,
        descripcion: 'Perdida del dedo anular izquierdo',
        valor: 8
      },
      {
        index: 31,
        descripcion: 'Perdida del dedo medio izquierdo',
        valor: 8
      },
      {
        index: 32,
        descripcion: 'Perdida del dedo gordo de alguno de los pies',
        valor: 8
      },
      {
        index: 33,
        descripcion: 'Perdida del dedo meñique derecho',
        valor: 7
      },
      {
        index: 34,
        descripcion: 'Perdida del dedo meñique izquierdo',
        valor: 5
      },
    ]
  }

}
