import { Component, Input, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { Amparo, TipoSumaAsegurada, ValorAseguradoAmparo } from 'src/app/models';
import { GrupoAseguradoWizardService } from 'src/app/services/grupo-asegurado-wizard.service';

@Component({
  selector: 'app-valores-asegurados',
  templateUrl: './valores-asegurados.component.html',
  styleUrls: ['./valores-asegurados.component.scss']
})
export class ValoresAseguradosComponent implements OnInit {
  dataSource: MatTableDataSource<any>;
  hasAmparosModalidad = false;

  constructor(private wizardService: GrupoAseguradoWizardService) { }

  @Input() amparos: Amparo[];
  @Input() readonly: boolean;
  @Input() tipoSumaAsegurada: TipoSumaAsegurada;

  get form() {
    return this.wizardService.formGroup;
  }

  get isValoresAseguradosANValid() {
    return this.wizardService.isValoresAseguradosANValid;
  }

  ngOnInit() {
    
    this.dataSource = new MatTableDataSource<any>([]);
    this.registerAmparosChange();
  }

  private registerAmparosChange() {
    this.wizardService.formGroup.get('amparos')
      .valueChanges
      .subscribe(change => {
        let row: any;
        const previousValue = this.dataSource.data.map(x => x.amparo);
        const currentValue = change.filter((x: any) => Object.keys(x).length !== 0);

        if (currentValue.length === 0) {
          this.clearDataSource();
          this.clearFormGroup();
          return;
        }

        const action = currentValue.length > previousValue.length ? 'push' : 'pop';
        action === 'push' ?
          row = currentValue.find((x: any) => !previousValue.includes(x)) :
          row = previousValue.find((x: any) => !currentValue.includes(x));
        action === 'push' ? this.pushAmparo(row) : this.popAmparo(row);

        this.hasAmparosModalidad = this.dataSource.data.some(x => x.amparo.modalidad.codigo === 4);
      });
  }

  private pushAmparo(amparo: Amparo) {
    const opciones = this.wizardService.generateValorAseguradoOpciones(amparo);
    const valorAseguradoItem: ValorAseguradoAmparo = {
      amparo,
      opciones,
      calculoBasePorValorAsegurado: true
    };
    const row = this.dataSource.data.find((x: any) => x.amparo.codigoAmparo === amparo.codigoAmparo);
    if (!row) {
      const tmp = this.dataSource.data;
      tmp.push(valorAseguradoItem);
      // let ascList = tmp.sort((a, b) => a.amparo.codigoAmparo - b.amparo.codigoAmparo)
      // sort by name
      const na = tmp.sort((a, b) => a.amparo.nombreAmparo.localeCompare(b.amparo.nombreAmparo));
      // sort by codigoGrupoAsegurado y siNoAdicional
      const ds = na.sort((a, b) => {
        return (a.amparo.codigoGrupoAmparo < b.amparo.codigoGrupoAmparo) ? -1 : a.amparo.siNoAdicional < b.amparo.siNoAdicional ? -1 : 0;
      });
      this.dataSource.data = ds;
      // add to form group
      this.wizardService.addValorAsegurado(valorAseguradoItem);
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
      this.wizardService.removeValorAsegurado(amparo, row.opciones);
    }
  }

  private clearDataSource() {
    if (this.dataSource) {
      const ds = [];
      this.dataSource.data = ds;
    }
  }

  private clearFormGroup() {
    this.wizardService.clearValoresAsegurados();
  }
}
