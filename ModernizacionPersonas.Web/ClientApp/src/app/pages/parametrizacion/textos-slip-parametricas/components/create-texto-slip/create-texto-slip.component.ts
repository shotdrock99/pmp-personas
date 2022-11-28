import { SeccionSlip } from './../../../../../models/seccion-slip';
import { Validators, FormGroup, FormBuilder } from '@angular/forms';
import { Component, OnInit, ViewChild, ElementRef, Inject } from '@angular/core';
import { VariableSlip } from 'src/app/models/variable-slip';
import { VariablesSlipReaderService } from 'src/app/services/variables-slip-reader.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TextoSlipEditorComponent } from '../texto-slip-editor/texto-slip-editor.component';
import { Ramo, SubRamo } from 'src/app/models';
import { PersonasReaderService } from 'src/app/services/personas-reader.service';
import { startWith, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { ParametrizacionSlipReaderService } from 'src/app/services/parametrizacion-slip-reader.service';
import { TextoSlip } from 'src/app/models/texto-slip';
import { Amparo2 } from 'src/app/models/fichatecnica';

@Component({
  selector: 'app-create-texto-slip',
  templateUrl: './create-texto-slip.component.html',
  styleUrls: ['./create-texto-slip.component.scss']
})
export class CreateTextoSlipComponent implements OnInit {

  model: TextoSlip;
  searchValue: string;
  texto: string;
  variables: VariableSlip[];
  submitted: boolean = false;

  @ViewChild('textoSlipArea', { static: true })
  textSlipArea: ElementRef;

  textForm: FormGroup;
  ramos: Ramo[];
  subramos: SubRamo[];
  amparos: Amparo2[];
  sections: SeccionSlip[];
  ramoTempCod: number;
  filteredRamos: Observable<Ramo[]>;
  filteredSubramos: Observable<SubRamo[]>;

  constructor(private formBuilder: FormBuilder,
    private variablesSlipReaderService: VariablesSlipReaderService,
    private seccionesSlipReader: ParametrizacionSlipReaderService,
    private dialogRef: MatDialogRef<TextoSlipEditorComponent>,
    private personasReaderService: PersonasReaderService,

    @Inject(MAT_DIALOG_DATA) data) {
    this.model = data;

  }

  get form() { return this.textForm.controls; }

  ngOnInit() {
    this.textForm = this.formBuilder.group({
      ramo: ['', [Validators.required]],
      subramo: ['', [Validators.required]],
      amparo: ['', [Validators.required]],
      section: ['', [Validators.required]]
    });

    this.loadSections();
    this.loadVariables();
    this.loadRamos();

    this.registerOnRamoChange();
    this.registerOnSubRamoChange();
  }

  loadSections() {
    this.seccionesSlipReader.getSeccionesSlip()
      .subscribe((res) => {
        this.sections = res.filter(x => x.activo === 1);
      });
  }

  private _filterRamos(value: string): Ramo[] {
    if (typeof (value) === 'string') {
      return this.ramos.filter(option => option.nombreRamo.toLowerCase().includes(value.toLowerCase()));
    }
  }

  private _filterSubramos(value: string): SubRamo[] {
    if (typeof (value) === 'string') {
      return this.subramos.filter(option => option.nombreSubRamo.toLowerCase().includes(value.toLowerCase()));
    }
  }

  private registerOnRamoChange() {
    this.textForm.get('ramo')
      .valueChanges
      .subscribe((selection) => {
        if (typeof (selection) === 'object') {
          this.ramoTempCod = selection.codigoRamo;
          this.loadSubramos(selection.codigoRamo);
          //this.loadAmparos(selection.codigoRamo,0);
        }
      });
  }
  private registerOnSubRamoChange() {
    this.textForm.get('subramo')
      .valueChanges
      .subscribe((selection) => {
        if (typeof (selection) === 'object') {
          //this.loadSubramos(selection.codigoRamo);

          this.loadAmparos(this.ramoTempCod, selection.codigoSubRamo, 1);
        }
      });
  }

  private loadSubramos(codigoRamo: any) {
    this.amparos = [];
    this.personasReaderService.getSubRamosPorRamo(codigoRamo)
      .subscribe((response: SubRamo[]) => {
        if (response.length === 0) {
          let ramo = SubRamo.create('No hay subramos disponibles.');
          this.subramos.push(ramo);
        }
        else {
          this.subramos = response;
          this.filteredSubramos = this.textForm.get('subramo')
            .valueChanges
            .pipe(
              startWith(''),
              map(value => this._filterSubramos(value))
            );

          if (this.model.codigoSubramo) {
            let subramo = this.subramos.find(x => x.codigoSubRamo === this.model.codigoSubramo);
            const ctrl = this.textForm.get('subramo');
            ctrl.setValue(subramo, { emitEvent: false });
            //ctrl.disable();

            this.textForm.disable({ emitEvent: false });
          }
        }
      });
  }

  private loadRamos() {
    this.personasReaderService.getRamos()
      .subscribe((response: Ramo[]) => {
        this.ramos = response;
        this.filteredRamos = this.textForm.get('ramo')
          .valueChanges
          .pipe(
            startWith(''),
            map(value => this._filterRamos(value))
          );

        if (this.model.codigoRamo) {
          let ramo = this.ramos.find(x => x.codigoRamo === this.model.codigoRamo);
          const ctrl = this.textForm.get('ramo');
          ctrl.setValue(ramo, { emitEvent: false });
          //ctrl.disable();

          this.loadSubramos(ramo.codigoRamo);
        }
      });
  }

  private loadAmparos(codigoRamo: number, subramo: number, code: number) {
    
    this.personasReaderService.getAmparos(codigoRamo, subramo, code)
      .subscribe(res => {
        this.amparos = res;
        let AmparoNA: Amparo2;
        AmparoNA = {
          codigoAmparo: 0,
          nombreAmparo: "NO APLICA",
          siNoAdicional: false,
          siNoBasico: false,
          siNoPorcentajeBasico: false,
          siNoRequiereEdades: false
        };

        this.amparos.push(AmparoNA);
        this.amparos = this.amparos.sort((a, b) => a.codigoAmparo - b.codigoAmparo);
      });
  }

  private loadVariables() {
    this.variablesSlipReaderService.getUnsedVariablesSlip()
      .subscribe(res => {
        this.variables = res;
      })
  }

  private insertTextAtCursor(text) {
    let selection = window.getSelection();
    let range = selection.getRangeAt(0);
    range.deleteContents();
    let node = document.createTextNode(text);
    range.insertNode(node);

    for (let position = 0; position != text.length; position++) {
      // selection.modify("move", "right", "character");
    };
  }

  addVariable(e, variable: VariableSlip) {
    // this.texto += `<span class="badge badge-info variable-text">${variable.nombreVariable}</span>`
    let newData = '$' + variable.nombreVariable;
    this.insertTextAtCursor(newData);
  }

  onAcceptClick() {
    this.submitted = true;
    if (this.textForm.valid) {
      let m = this.textForm.getRawValue();
      var args = {
        codigoRamo: m.ramo.codigoRamo,
        codigoSubramo: m.subramo.codigoSubRamo,
        codigoAmparo: m.amparo.codigoAmparo,
        codigoSeccion: m.section.codigo,
        nombreSeccion: m.section.seccion,
        texto: this.textSlipArea.nativeElement.innerText
      };

      this.dialogRef.close(args);
    }
  }

  close() {
    this.dialogRef.close();
  }
}
