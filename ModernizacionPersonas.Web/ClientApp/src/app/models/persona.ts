import { IPersona } from './IPersonsa';

export class Persona implements IPersona {
  Codigo: string;
  Correo: any[];
  DigitoVerificacion: string;
  Direccion: any[];
  NumeroDocumento: string;
  FechaNacimiento: string;
  Genero: string;
  PrimerApellido: string;
  PrimerNombre: string;
  SegundoApellido: string;
  SegundoNombre: string;
  Telefono: any[];
  TipoDocumento: string;
  TipoPersona: string;

  static getTelefono(model): any {
    var tel = '';
    if (model.telefonoContacto1 != undefined) {
      if (model.telefonoContacto1.trim() != "")
        return model.telefonoContacto1;
    }

    if (typeof model.telefono === 'string') {
      tel = model.telefono;
    }
    if (model.telefono != null) {
      if (model.telefono.length > 0) {
        model.telefono.forEach(element => {
          if (element.tipoTelefono == 24) {
            tel = element.numeroTelefono
          }
        });

      }
    }
    return tel;
  }
  static getCorreo(model): any {
    if (!model.correo) return '';
    if (typeof model.correo === 'string') {
      return model.correo;
    }

    if (model.correo.length > 0) {
      return model.correo[0].correoElectronico;
    }
    return '';
  }
  static getDireccion(model): any {
    if (!model.direccion) return '';
    if (typeof model.direccion === 'string') {
      return model.direccion;
    }

    if (model.direccion.length > 0) {
      return model.direccion[0].direccionPersona;
    }
    return '';
  }
}
