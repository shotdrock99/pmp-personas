export interface GuardarResumenArgs {
  codigoCotizacion: number;
  porcentajeRetorno: number;
  porcentajeOtrosGastos: number;
  porcentajeComision: number;
  utilidadCompania: number;
  gastosCompania: number;
  factorG: any;
  tasaOpciones: TasaOpcion[];
}

export interface TasaOpcion {
  codigoGrupoAsegurado: number;
  indiceOpcion: number;
  sumatoriaTasa?: number;
  tasaComercial: number;
  tasaComercialTotal: number;
  descuento: number;
  descuentoSiniestralidad: number;
  recargo: number;
  recargoSiniestralidad: number;
  primaIndividual: number;
  primaTotal: number;
}
