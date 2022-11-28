import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import * as shortid from 'shortid';
import { EdadAsegurabilidad } from 'src/app/models/edad-asegurabilidad';
import { ConfiguracionSlipWriterService } from 'src/app/services/configuracion-slip-writer.service';

@Component({
  selector: 'app-configuracion-slip-asegurabilidad',
  templateUrl: './configuracion-slip-asegurabilidad.component.html',
  styleUrls: ['./configuracion-slip-asegurabilidad.component.scss'],
})
export class ConfiguracionSlipAsegurabilidadComponent implements OnInit {
  constructor(
    private configuracionSlipWriterService: ConfiguracionSlipWriterService
  ) { }

  errors = [];
  isEdit = false;
  requisitos: string[] = ['A', 'B', 'C'];
  @Input() data: EdadAsegurabilidad[];
  @Input() readonly: boolean;
  @Output() refresh: EventEmitter<any> = new EventEmitter();
  @Output() valueChange: EventEmitter<any> = new EventEmitter();

  currentItem: EdadAsegurabilidad;
  originalItem: EdadAsegurabilidad;

  ngOnInit() { }

  private validateItem(item: EdadAsegurabilidad) {
    this.errors = [];
    let result = true;
    const edadDesde = Number(item.edadDesde);
    const edadHasta = Number(item.edadHasta);
    const valorIndividualDesde = Number(item.valorIndividualDesde);
    const valorIndividualHasta = Number(item.valorIndividualHasta);
    // valida que el registro no exista en la coleccion, valida campos de edadDesde y edadHasta del registro
    /*const itemExist =
      this.data.filter(
        (x) =>
          x.codigoAsegurabilidad !== 0 &&
          x.edadDesde === item.edadDesde &&
          x.edadHasta === item.edadHasta
      ).length > 0;
    if (itemExist) {
      this.errors.push(
        'Ya existe un registro configurado con esta información'
      );
      result = false;
    }*/

    if (!item.requisitos || item.requisitos === '') {
      this.errors.push('Debe seleccionar una opción en el campo requisitos.');
      result = false;
    }

    if (edadDesde === 0 || edadHasta === 0) {
      this.errors.push('La edad configurada no puede ser cero.');
      result = false;
    }

    if (valorIndividualDesde === 0 || valorIndividualHasta === 0) {
      this.errors.push('El valor configurado no puede ser cero.');
      result = false;
    }

    if (edadDesde > edadHasta) {
      this.errors.push('La edad inicial no puede ser menor a la edad final.');
      result = false;
    }

    if (valorIndividualDesde > valorIndividualHasta) {
      this.errors.push('El valor inicial no puede ser menor al valor final.');
      result = false;
    }

    return result;
  }

  inputValueChange(): void {
    this.valueChange.emit({ dirty: true });
  }

  addItem() {
    this.isEdit = true;
    this.currentItem = {
      codigoAsegurabilidad: 0,
      edadDesde: 0,
      edadHasta: 0,
      valorIndividualDesde: 0,
      valorIndividualHasta: 0,
      requisitos: '',
      _isEdit: true,
    };
    this.data.push(this.currentItem);
    this.valueChange.emit({ dirty: true });
  }

  editItem(index, item) {
    this.originalItem = { ...item };
    this.currentItem = item;
    item._isEdit = true;
  }

  removeItem(index: number, item: EdadAsegurabilidad) {
    this.data.splice(index, 1);
    this.valueChange.emit({ dirty: true });
    this.configuracionSlipWriterService
      .removeRangoAsegurabilidad(item.codigoAsegurabilidad)
      .subscribe((res) => {
        // TODO
        if (res) {
          // console.log(
          //   "El registro de asegurabilidad ha sido eliminado exitosamente"
          // );
        }
        // this.refresh.emit();
      });
  }

  cancelEdit(index, item) {
    if (item.codigo === 0) {
      // es nuevo
      this.data.pop();
    }

    if (this.originalItem) {
      this.currentItem = this.originalItem;
    }

    this.currentItem._isEdit = false;
    this.isEdit = false;
  }

  saveItem(index, item) {
    const isValid = this.validateItem(item);
    if (isValid) {
      const cod = shortid.generate();
      this.data.pop();

      item.codigo = cod;
      item._isEdit = false;
      this.data.push(item);

      this.isEdit = false;

      this.configuracionSlipWriterService
        .saveRangoAsegurabilidad(item)
        .subscribe((res) => {
          // TODO
          if (res) {
            console.log(
              'El registro de asegurabilidad ha sido agregado exitosamente'
            );
          }

          item.codigoAsegurabilidad = res.asegurabilidadId;
          // this.refresh.emit();
        });
    }
  }
}
