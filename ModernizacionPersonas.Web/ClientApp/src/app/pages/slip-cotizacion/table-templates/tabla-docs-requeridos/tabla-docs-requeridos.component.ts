import { Component, OnInit } from '@angular/core';
import { DocumentosRequeridosData } from '../../../../models/documentos-requeridos-data';

@Component({
  selector: 'app-tabla-docs-requeridos',
  templateUrl: './tabla-docs-requeridos.component.html',
  styleUrls: ['./tabla-docs-requeridos.component.scss']
})
export class TablaDocsRequeridosComponent implements OnInit {

  constructor() { }

  model: DocumentosRequeridosData;
  model1: DocumentosRequeridosData = {
    columns: ['MUERTE ACCIDENTAL', 'INCAPACIDAD TOTAL Y PERMANENTE', 'ENFERMEDADES GRAVES', 'RENTA DIARIA POR HOSPITALIZACION'],
    data: [{
      index: 1,
      descripcion: 'Solicitud Individual para Seguro de Vida Grupo',
      valores: [true, true, true, true]
    }, {
      index: 2,
      descripcion: 'Formulario único de conocimiento del cliente. (FUCC)',
      valores: [true, true, true, true]
    }, {
      index: 3,
      descripcion: 'Carta de reclamación',
      valores: [true, true, true, true]
    }, {
      index: 4,
      descripcion: 'Fotocopia de la cédula del asegurado ampliada al 150%',
      valores: [true, true, true, true]
    }, {
      index: 5,
      descripcion: 'Registro civil de defunción original o fotocopia autenticada',
      valores: [true, false, false, false]
    }, {
      index: 6,
      descripcion: 'Muerte accidental. Informe de Fiscalía donde se detallen las circunstancias de tiempo, modo y lugar del fallecimiento',
      valores: [true, false, false, false]
    }, {
      index: 7,
      descripcion: 'En caso de accidente de transito, croquis o informe del accidente',
      valores: [true, true, false, false]
    }, {
      index: 8,
      descripcion: 'Muerte Natural. Historia clínica completa',
      valores: [true, false, false, false]
    }, {
      index: 9,
      descripcion: 'Calificación emitida por entidad competente (Junta de calificación) de acuerdo con la ley 100 y sus decretos reglamentarios, donde se acredite el porcentaje de perdida de de capacidad laboral (PCL)',
      valores: [false, true, false, false]
    }, {
      index: 10,
      descripcion: 'Historia clínica y/o informe medico que permita establecer la existencia de la enfermedad, tiempo estimado del padecimiento de la misma, fecha de diagnostico tratamiento requerido',
      valores: [true, true, true, true]
    }, {
      index: 11,
      descripcion: 'Certificado del numero de días de hospitalizaron de la E.P.S., acompañado del resumen de la Historia clínica de la atención',
      valores: [false, false, false, true]
    }, {
      index: 12,
      descripcion: 'Documentos de identificación de cada uno de los beneficiarios designados',
      valores: [true, false, false, false]
    }]
  };

  model2: DocumentosRequeridosData = {
    columns: ['MUERTE ACCIDENTAL', 'INCAPACIDAD TOTAL Y PERMANENTE'],
    data: [{
      index: 1,
      descripcion: 'Solicitud Individual para Seguro de Vida Grupo',
      valores: [true, true]
    }, {
      index: 2,
      descripcion: 'Formulario único de conocimiento del cliente. (FUCC)',
      valores: [true, true]
    }, {
      index: 3,
      descripcion: 'Carta de reclamación',
      valores: [true, true]
    }, {
      index: 4,
      descripcion: 'Fotocopia de la cédula del asegurado ampliada al 150%',
      valores: [true, true]
    }, {
      index: 5,
      descripcion: 'Registro civil de defunción original o fotocopia autenticada',
      valores: [true, false]
    }, {
      index: 6,
      descripcion: 'Muerte accidental. Informe de Fiscalía donde se detallen las circunstancias de tiempo, modo y lugar del fallecimiento',
      valores: [true, false]
    }, {
      index: 7,
      descripcion: 'En caso de accidente de transito, croquis o informe del accidente',
      valores: [true, true]
    }, {
      index: 8,
      descripcion: 'Muerte Natural. Historia clínica completa',
      valores: [true, false]
    }, {
      index: 9,
      descripcion: 'Calificación emitida por entidad competente (Junta de calificación) de acuerdo con la ley 100 y sus decretos reglamentarios, donde se acredite el porcentaje de perdida de de capacidad laboral (PCL)',
      valores: [false, true]
    }, {
      index: 10,
      descripcion: 'Historia clínica y/o informe medico que permita establecer la existencia de la enfermedad, tiempo estimado del padecimiento de la misma, fecha de diagnostico tratamiento requerido',
      valores: [true, true]
    }, {
      index: 11,
      descripcion: 'Certificado del numero de días de hospitalizaron de la E.P.S., acompañado del resumen de la Historia clínica de la atención',
      valores: [false, false]
    }, {
      index: 12,
      descripcion: 'Documentos de identificación de cada uno de los beneficiarios designados',
      valores: [true, false]
    }]
  };

  ngOnInit() {
    this.model = this.model1;
  }

}
