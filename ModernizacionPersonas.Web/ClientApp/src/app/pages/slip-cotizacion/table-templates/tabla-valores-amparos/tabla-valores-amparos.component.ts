import { ValoresAseguradosAmparoSlip, OpcionValoresAseguradosAmparoSlip } from '../../../../models/slip';
import { Component, OnInit, Input } from '@angular/core';
import { PreviewSlipReaderService } from 'src/app/services/preview-slip-reader.service';

@Component({
  selector: 'app-tabla-valores-amparos',
  templateUrl: './tabla-valores-amparos.component.html',
  styleUrls: ['./tabla-valores-amparos.component.scss']
})
export class TablaValoresAmparosComponent implements OnInit {

  constructor(private slipReaderService: PreviewSlipReaderService) { }

  countOpciones: number;
  opciones = [];
  isSumaFija: boolean;
  nameTipoSuma: string;
  @Input() data: ValoresAseguradosAmparoSlip[];
  @Input() esTasaMensual: boolean;

  ngOnInit() {
    
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
        
  }
}
