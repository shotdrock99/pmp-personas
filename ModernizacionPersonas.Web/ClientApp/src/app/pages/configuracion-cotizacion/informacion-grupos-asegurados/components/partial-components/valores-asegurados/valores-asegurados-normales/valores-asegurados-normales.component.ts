import { ChangeDetectionStrategy, Component, DoCheck, Input, IterableDiffer,
  IterableDiffers, OnInit, Renderer2, ViewChild } from '@angular/core';
import { FormArray } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { Amparo, TipoSumaAsegurada, ValorAseguradoAmparo } from 'src/app/models';
import { GrupoAseguradoWizardService } from 'src/app/services/grupo-asegurado-wizard.service';
import { ValoresAseguradosService } from 'src/app/services/valores-asegurados.service';

@Component({
  selector: 'app-valores-asegurados-normales',
  templateUrl: './valores-asegurados-normales.component.html',
  styleUrls: ['./valores-asegurados-normales.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ValoresAseguradosNormalesComponent implements OnInit, DoCheck {

  headers: any;
  displayedColumns: string[];
  esTipoSumaFija: boolean;
  capturaSalarios: boolean;
  capturaValorAsegurado = true;
  valoresAseguradosForm: FormArray;
  iterableDiffer: IterableDiffer<any>;

  siNoSolicitaValorAmparoBasicoNoAdicional: boolean;

  dataSource: MatTableDataSource<any>;
  dataSourceComplete: any[];

  constructor(
    private wizardService: GrupoAseguradoWizardService,
    private valoresAseguradosService: ValoresAseguradosService,
    private renderer: Renderer2,
    private iterableDiffers: IterableDiffers) {
      this.iterableDiffer = iterableDiffers.find([]).create(null);
    }

  @Input() amparos: ValorAseguradoAmparo[];
  @Input() readonly: boolean;
  @Input() tipoSumaAsegurada: TipoSumaAsegurada;

  @ViewChild('tableValoresAmparos', { static: true })
  tableHtml: any;

  get form() {
    return this.wizardService.formGroup;
  }

  get valoresAsegurados() {
    return this.wizardService.valoresAseguradosArray.controls;
  }

  ngOnInit() {
    this.valoresAseguradosService.init(this.tipoSumaAsegurada);
    this.dataSource = new MatTableDataSource<any>([]);
    this.headers = this.valoresAseguradosService.columnHeaders;
    this.displayedColumns = this.valoresAseguradosService.tableColumns;
    this.valoresAseguradosForm = this.wizardService.valoresAseguradosArray;
    this.esTipoSumaFija = this.tipoSumaAsegurada.codigoTipoSumaAsegurada === 1;
    this.capturaSalarios = this.wizardService.calculaValorAseguradoONumeroSalarios || this.wizardService.esMultiploSueldos;
    this.capturaValorAsegurado = this.tipoSumaAsegurada.codigoTipoSumaAsegurada !== 2;
    this.__removeWeirdHeaders();
  }

  ngDoCheck() {
    const changes = this.iterableDiffer.diff(this.amparos);
    if (changes) {
      const filteredAmparos = this.amparos.filter(x => x.amparo.modalidad.codigo !== 4);
      // sort by name
      const na = filteredAmparos.sort((a, b) => a.amparo.nombreAmparo.localeCompare(b.amparo.nombreAmparo));
      // sort by codigoGrupoAsegurado y siNoAdicional
      const ds = na.sort((a, b) => {
        return (a.amparo.codigoGrupoAmparo < b.amparo.codigoGrupoAmparo) ? -1 : a.amparo.siNoAdicional < b.amparo.siNoAdicional ? -1 : 0;
      });
      this.dataSource.data =  ds;
      
      this.dataSourceComplete = this.amparos;
    }
    this.__removeWeirdHeaders();
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
