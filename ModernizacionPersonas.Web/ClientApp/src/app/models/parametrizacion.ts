export interface Amparo {
  codigoAmparo: number;
  codigoGrupoAmparo: number;
  edadMaximaIngreso: number;
  edadMaximaPermanencia: number;
  edadMinimaIngreso: number;
  nombreAmparo: string;
  nombreCortoGrupoAmparo: string;
  nombreGrupoAmparo: string;
  siNoAdicional: boolean;
  siNoBasico: boolean;
  siNoPorcentajeBasico: boolean;
  siNoRequiereEdades: boolean;
}

export interface TipoSumaAsegurada {
  codigoTipoSumaAsegurada: number;
  nombreTipoSumaAsegurada: string;
  valorSalarioMinimo: number;
}
