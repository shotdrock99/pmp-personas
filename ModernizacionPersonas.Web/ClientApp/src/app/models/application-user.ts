import { Permiso, Rol } from "src/app/models/rol";
export class ApplicationUser {
  public userId: number;
  public userName: string;
  public email: string;
  public documentId: string;
  public rol: ApplicationRole;
  public permissions: Permiso[];

  public externalInfo: UserExternalInfo;
}

export class UserExternalInfo {
  public loginUsuario: string;
  nombreUsuario: string;
  emailUsuario: string;
  zona: number;
  descripcion: string;
  sucursal: number;
  nombreSucursal: string;
  codigoAreaDependencia: number;
  nombreDependencia: string;
  codigoArea: number;
  area: string;
  codigoCargo: number;
  cargo: string;
  codigoTipoDependencia: number;
  tipoDependencia: string;
}

export class ApplicationRole {
  public roleId: number;
  public roleName: string;
  public roleDescription: string;
}
