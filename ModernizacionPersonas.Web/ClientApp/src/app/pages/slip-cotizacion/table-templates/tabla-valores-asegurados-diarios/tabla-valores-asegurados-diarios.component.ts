import { ValoresAseguradosAmparoSlip, OpcionValoresAseguradosAmparoSlip } from '../../../../models/slip';
import { Component, OnInit, Input } from '@angular/core';
import { PreviewSlipReaderService } from 'src/app/services/preview-slip-reader.service';

@Component({
  selector: 'app-tabla-valores-asegurados-diarios',
  templateUrl: './tabla-valores-asegurados-diarios.component.html',
  styleUrls: ['./tabla-valores-asegurados-diarios.component.scss']
})
export class TablaValoresAseguradosDiariosComponent implements OnInit {

  constructor(private slipReaderService: PreviewSlipReaderService) { }

  countOpciones: number;
  opciones = [];
  isSumaFija: boolean;
  nameTipoSuma: string;
  tablValoresAsegDiarios: ValoresAseguradosAmparoSlip[] = [];
  @Input() data: ValoresAseguradosAmparoSlip[];
  @Input() esTasaMensual: boolean;

  ngOnInit() {
    ;
    var tablValoresAseglocal: ValoresAseguradosAmparoSlip[] = [];
    this.countOpciones = this.data[0].countOpciones;
    this.opciones = Array(this.countOpciones);
    this.isSumaFija = this.data[0].codigoTipoSumaAsegurada === 1 ? true : false;
    this.nameTipoSuma =
      this.data[0].codigoTipoSumaAsegurada === 1
        ? 'SUMA FIJA'
        : this.data[0].codigoTipoSumaAsegurada === 2
          ? 'MULTIPLO DE SUELDOS'
          : this.data[0].codigoTipoSumaAsegurada === 3
            ? 'SUMA VARIABLE POR ASEGURADO'
            : this.data[0].codigoTipoSumaAsegurada === 5
              ? 'SUMA UNIFORME MAS MULTIPLO DE SUELDOS'
              : this.data[0].codigoTipoSumaAsegurada === 6
                ? 'SALDO DEUDORES'
                : 'S.M.M.L.V.';
    this.data.forEach(element => {
      element.opciones.forEach(ele => {
        if (ele.tablaValoresDiarios) {
          tablValoresAseglocal.push(element);
          return;
        }
      });
    });
    var hash = {};
    this.tablValoresAsegDiarios = tablValoresAseglocal.filter(function (current) {
      var exists = !hash[current.nombreAmparo];
      hash[current.nombreAmparo] = true;
      return exists;
    });
  }
}
