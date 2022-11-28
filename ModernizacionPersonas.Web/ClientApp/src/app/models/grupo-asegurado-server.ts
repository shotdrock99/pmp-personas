import { Amparo } from '.';

export class GrupoAseguradoServerModel {
    codigoGrupoAsegurado: number;
    nombreGrupoAsegurado: string;
    codigoTipoSuma: number;
    valorMinAsegurado: number;
    valorMaxAsegurado: number;
    numeroSalariosAsegurado: number;
    codigoCotizacion: number;
    amparosGrupo: AmparoGrupo[]
}

export class AmparoGrupo extends Amparo {
    codigoAmparoGrupoAsegurado: number;
    codigoGrupoAsegurado: number;
    codigoAmparo: number;
    opcionesValores: OpcionValor[];
    edadesGrupo: EdadesValorGrupo;
}

export class OpcionValor {
    codigoOpcionValorAsegurado: number;
    codigoAmparoGrupoAsegurado: number;
    porcentajeCobertura: number;
    numeroSalarios: number;
    tasa: number;
    valorAsegurado: number;
    prima: number;
    numeroDias: number;
    valorDiario: number;
    valorAseguradoDias: number;
}

export class EdadesValorGrupo {
    codigoEdadPermanencia: number;
    codigoGrupoAsegurado: number;
    codigoAmparo: number;
    edadMinAsegurado: number;
    edadMaxAsegurado: number;
    diasCarencia: number;
    edadMaxPermanencia: number;
}
