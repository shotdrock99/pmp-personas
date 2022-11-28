import { DataSource } from '@angular/cdk/table';
import { CurrencyPipe } from '@angular/common';
import { Component, DoCheck, Input, IterableDiffer, IterableDiffers, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { Amparo, TipoSumaAsegurada, ValorAseguradoAmparo } from 'src/app/models';
import { GrupoAseguradoWizardService } from 'src/app/services/grupo-asegurado-wizard.service';

@Component({
  selector: 'valores-asegurados-modalidades',
  templateUrl: './valores-asegurados-modalidades.component.html',
  styleUrls: ['./valores-asegurados-modalidades.component.scss']
})
export class ValoresAseguradosModalidadesComponent implements OnInit, DoCheck {

  displayedColumns: string[];
  displayedRows: string[];
  dataSource: MatTableDataSource<any>;
  dataSourceComplete: any[];
  currencyPipe = new CurrencyPipe('en-US');
  esTipoSumaFija: boolean;
  iterableDiffer: IterableDiffer<any>;

  constructor(
    private wizardService: GrupoAseguradoWizardService,
    private iterableDiffers: IterableDiffers) { 
      this.iterableDiffer = iterableDiffers.find([]).create(null);
    }
 
  @Input()
  amparos: any

  @Input('readonly')
  readonly: boolean;

  @Input()
  tipoSumaAsegurada: TipoSumaAsegurada;

  get form() {
    return this.wizardService.formGroup;
  }

  get valoresAsegurados() {
    return this.wizardService.valoresAseguradosArray.controls;
  }

  ngOnInit() {
    this.dataSource = new MatTableDataSource<any>([]);
    this.esTipoSumaFija = this.tipoSumaAsegurada.codigoTipoSumaAsegurada === 1 ? true : false;
    this.displayedColumns = this.esTipoSumaFija
      ? [
          "amparo",
          "nDias1",
          "valorDiario1",
          "valor1",
          "nDias2",
          "valorDiario2",
          "valor2",
          "nDias3",
          "valorDiario3",
          "valor3",
        ]
      : ["amparo", "nDias1", "valorDiario1", "valor1"];
  }

  ngDoCheck() {
    let changes = this.iterableDiffer.diff(this.amparos);
    if (changes) {
      this.dataSource.data =  this.amparos.filter(x => x.amparo.modalidad.codigo === 4);
      this.dataSourceComplete = this.amparos;
    }
  }

}
