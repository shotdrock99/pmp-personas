export class Tomador {
  codigoCotizacion: number;
  codigoTomador: number;
  codigoTipoDocumento: number;
  numeroDocumento: string;
  nombres: string;
  primerApellido: string;
  segundoApellido: string;
  direccion: string;
  codigoPais: number;
  codigoDepartamento: number;
  codigoMunicipio: number;
  email: string;
  nombreContacto: string;
  telefono1Contacto: string;
  telefono2Contacto: string;
  codigoActividad: string;
  licitacion: boolean;
  aseguradoraActual: string;
  descripcionTipoRiesgo: string;

  static copy(model: Tomador): Tomador {
    let t = new Tomador();
    return Object.assign(t, model)
  }

  isEqual(this: Tomador, target: Tomador) {
    return this.codigoTipoDocumento === target.codigoTipoDocumento
      && this.primerApellido === target.primerApellido
      && this.numeroDocumento === target.numeroDocumento
      && false;
  }
}
