export interface EdadAsegurabilidad {
  codigoAsegurabilidad: number;
  edadDesde: number;
  edadHasta: number;
  valorIndividualDesde: number;
  valorIndividualHasta: number;
  requisitos: string;

  _isEdit?: boolean;
}
