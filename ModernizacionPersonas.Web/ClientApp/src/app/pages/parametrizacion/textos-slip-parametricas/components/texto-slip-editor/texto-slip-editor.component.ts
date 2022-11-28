import { TextoSlip } from './../../../../../models/texto-slip';
import { VariableSlip } from 'src/app/models/variable-slip';
import { Component, OnInit, Inject, ElementRef, ViewChild } from '@angular/core';
import { VariablesSlipReaderService } from 'src/app/services/variables-slip-reader.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-texto-slip-editor',
  templateUrl: './texto-slip-editor.component.html',
  styleUrls: ['./texto-slip-editor.component.scss']
})
export class TextoSlipEditorComponent implements OnInit {

  model: TextoSlip;
  searchValue: string;
  texto: string;
  variables: VariableSlip[];

  @ViewChild('textoSlipArea', { static: true })
  textSlipArea: ElementRef;

  constructor(private variablesSlipReaderService: VariablesSlipReaderService,
    private dialogRef: MatDialogRef<TextoSlipEditorComponent>,
    @Inject(MAT_DIALOG_DATA) data) {
    this.model = data.model;
    this.texto = this.model.texto;
  }

  ngOnInit() {
    this.loadVariables();
  }

  private loadVariables() {
    this.variablesSlipReaderService.getVariablesSlipByCodigoTexto(this.model.codigo)
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
    this.model.texto = this.textSlipArea.nativeElement.innerText;
    this.dialogRef.close(this.model);
  }

  close() {
    this.dialogRef.close();
  }
}
