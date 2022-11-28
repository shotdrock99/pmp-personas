
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { AmparosValidator, EdadesAmparosValidator, EdadesAmparoValidator, GrupoAseguradosValidator, PercentValidator, RawValueValidator,
  ValorAseguradoOpcionValidator, ValoresAseguradosValidatorFn } from './grupos-asegurados/grupo-asegurados.validators';


export class GrupoAseguradosFormFactory {

  private Form: FormGroup;

  get form() {
    return this.Form;
  }

  constructor(private formBuilder: FormBuilder) {
    this.createGrupoAseguradoForm();
  }

  private createGrupoAseguradoForm(): void {
    const numeroAseguradosValidators = [];
    const edadPromedioValidators = [];
    this.Form = this.formBuilder.group({
      amparos: this.formBuilder.array([], [AmparosValidator]),
      valoresAsegurados: this.formBuilder.array([], [ValoresAseguradosValidatorFn]),
      edadesAmparos: this.formBuilder.array([], [EdadesAmparosValidator]),
      asegurados: this.formBuilder.group({
        conListaAsegurados: [false],
        valorAsegurado: [''],
        conDistribucionAsegurados: [false],
        aseguradosOpcion1: ['', []],
        aseguradosOpcion2: ['', []],
        aseguradosOpcion3: ['', []],
        numeroAsegurados: ['', numeroAseguradosValidators],
        porcentajeAsegurados: [''],
        numeroPotencialAsegurados: [''],
        edadPromedio: ['', edadPromedioValidators],
        archivoCargado: [''],
        tipoEstructura: ['tipoUno'],
        rangos: this.formBuilder.array([])
      })
    }, GrupoAseguradosValidator);
  }

  private defineValorAseguradoGroup(): FormGroup {
    return this.formBuilder.group({
      amparo: [],
      opciones: this.formBuilder.array([],
        [ValorAseguradoOpcionValidator])
    }, [ValoresAseguradosValidatorFn]);
  }

  private defineValorAseguradoOpcionGroup(): FormGroup {
    return this.formBuilder.group({
      porcentaje: this.formBuilder.control({
        enabled: [false],
        rawValue: [0]
      }, [PercentValidator]),
      valor: this.formBuilder.control({
        enabled: [false],
        rawValue: [0]
      }, [RawValueValidator])
    });
  }

  private defineEdadesAmparoGroup(): FormGroup {
    return this.formBuilder.group({
      amparo: this.formBuilder.control({}),
      edadMinimaIngreso: this.defineEdadesAmparoField(),
      edadMaximaIngreso: this.defineEdadesAmparoField(),
      edadMaximaPermanencia: this.defineEdadesAmparoField(),
      numeroDiasCarencia: this.defineEdadesAmparoField()
    });
  }

  private defineEdadesAmparoField(): FormControl {
    return this.formBuilder.control({
      disabled: [false],
      rawValue: [0]
    }, [EdadesAmparoValidator]);
  }
}
