import { PreviewSlipReaderService } from '../../../../services/preview-slip-reader.service';
import { Component, OnInit, Input } from '@angular/core';
import { EdadesAmparoSlip } from '../../../../models/slip';

@Component({
  selector: 'app-tabla-edades-amparos',
  templateUrl: './tabla-edades-amparos.component.html',
  styleUrls: ['./tabla-edades-amparos.component.scss']
})
export class TablaEdadesAmparosComponent implements OnInit {

  constructor(private slipReaderService: PreviewSlipReaderService) { }

  hasEdadMaxPermitida: boolean;

  @Input('data')
  data: EdadesAmparoSlip[];

  ngOnInit() {
    this.hasEdadMaxPermitida = this.data.some(x => x.edadMaximaPermanencia === 111);
  }
}
