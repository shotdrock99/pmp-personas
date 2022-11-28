import { Component, DoCheck, Input, OnInit, Renderer2, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { Amparo, EdadIngresoPermanenciaAmparo } from 'src/app/models';
import { EdadesAmparosValidator } from 'src/app/services/grupos-asegurados/edades-amparos.validator';
import { GrupoAseguradoWizardService } from 'src/app/services/grupo-asegurado-wizard.service';


@Component({
  selector: 'app-edades-ingresosamparos',
  templateUrl: './edadesingresosamparos.component.html',
  styleUrls: ['./edadesingresosamparos.component.scss']
})
export class EdadesingresosamparosComponent implements OnInit, DoCheck {

  constructor(
    private renderer: Renderer2,
    private edadesAmparosValidator: EdadesAmparosValidator,
    private wizardService: GrupoAseguradoWizardService) { }

  displayedColumns: string[] = [
    'amparo',
    'edadminima',
    'edadmaxima',
    'edadmaxpermanencia',
    //'numdiascarencia',
    'opciones'
  ];

  dataSource = new MatTableDataSource<EdadIngresoPermanenciaAmparo>([]);
  @Input() amparos: Amparo[];
  @Input() readonly: boolean;

  @ViewChild('tableEdades', { static: true })
  tableHtml: any;

  ngOnInit() {
    this.dataSource = new MatTableDataSource<any>([]);
    this.registerAmparosChange();
    // this.__removeWeirdHeaders();
    this.wizardService.isEdadesAmparosValid = true;
  }

  ngDoCheck(): void {
    this.__removeWeirdHeaders();
  }

  private registerAmparosChange() {
    this.wizardService.formGroup.get('amparos')
      .valueChanges
      .subscribe(change => {
        let row;
        const previousValue = this.dataSource.data.map(x => x.amparo);
        const currentValue = change.filter(x => Object.keys(x).length !== 0);
        if (currentValue.length === 0) {
          this.clearDataSource();
          return;
        }
        const action = currentValue.length > previousValue.length ? 'push' : 'pop';
        action === 'push' ?
          row = currentValue.find(x => !previousValue.includes(x)) :
          row = previousValue.find(x => !currentValue.includes(x));
        action === 'push' ? this.pushAmparo(row) : this.popAmparo(row);
        // this.__removeWeirdHeaders();
      });
  }

  private clearDataSource() {
    if (this.dataSource) {
      const ds = [];
      this.dataSource.data = ds;
    }
  }

  private pushAmparo(amparo: Amparo) {
    const edadMinimaIngreso = this.wizardService.createEdadIngresoPermanenciaField(amparo, 'edadMinimaIngreso');
    const edadMaximaIngreso = this.wizardService.createEdadIngresoPermanenciaField(amparo, 'edadMaximaIngreso');
    const edadMaximaPermanencia = this.wizardService.createEdadIngresoPermanenciaField(amparo, 'edadMaximaPermanencia');
    const numeroDiasCarencia = this.wizardService.createEdadIngresoPermanenciaField(amparo, 'numeroDiasCarencia');
    const edadesAmparoItem: EdadIngresoPermanenciaAmparo = {
      amparo,
      edadMaximaIngreso,
      edadMinimaIngreso,
      edadMaximaPermanencia,
      numeroDiasCarencia
    };
    const row = this.dataSource.data.find(x => x.amparo.codigoAmparo === amparo.codigoAmparo);
    if (!row) {
      const tmp = this.dataSource.data;
      tmp.push(edadesAmparoItem);
      const ascList = tmp.sort((a, b) => a.amparo.codigoAmparo - b.amparo.codigoAmparo);
      // sort by codigoGrupoAsegurado y siNoAdicional
      const ds = ascList.sort((a, b) => {
        return (a.amparo.codigoGrupoAmparo < b.amparo.codigoGrupoAmparo) ? -1 : a.amparo.siNoAdicional < b.amparo.siNoAdicional ? -1 : 0;
      });
      this.dataSource.data = ds;
      // add to form group
      this.wizardService.addEdadesAmparo(edadesAmparoItem);
    }
  }

  private popAmparo(amparo: Amparo) {
    const row = this.dataSource.data.find(x => x.amparo.codigoAmparo === amparo.codigoAmparo);
    if (row) {
      const idx = this.dataSource.data.indexOf(row);
      const ds = this.dataSource.data;
      ds.splice(idx, 1);
      this.dataSource.data = ds;
      // remove from form group
      this.wizardService.removeEdadesAmparo(amparo);
    }
  }

  changeValue(element: EdadIngresoPermanenciaAmparo, event: any) {
    const htmlEl = event.currentTarget;
    const parentEl = htmlEl.parentElement.parentElement;
    const column = htmlEl.getAttribute('data-column');
    const val = htmlEl.value;
    const rawValue = Number(val.replace(/\D+/g, ''));
    let key: string;
    element[column].rawValue = rawValue;
    element[column].valor = rawValue;

    const isValid = this.edadesAmparosValidator.validateOption(element);
    if (!isValid) {
      // this.renderer.addClass(htmlEl, 'input-error');
      this.renderer.addClass(parentEl, 'amparo-error');
      element[key = 'errors'] = this.edadesAmparosValidator.errors;
    } else {
      // this.renderer.removeClass(htmlEl, 'input-error');
      this.renderer.removeClass(parentEl, 'amparo-error');
    }

    this.wizardService.isEdadesAmparosValid = this.edadesAmparosValidator.errors.length === 0;
  }

  private __removeWeirdHeaders() {
    const nativeEl = this.tableHtml._elementRef.nativeElement;
    const rows: any[] = nativeEl.querySelectorAll('tr.mat-row');
    rows.forEach(r => {
      const ths: any[] = r.querySelectorAll('th');
      ths.forEach(el => {
        this.renderer.removeChild(nativeEl, el);
      });
    });
  }
}
