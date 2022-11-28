export interface UsuarioPersonas {
  name: string;
  email: string;
  userId?: number;
  userName: string;
  rol: RolUsuarioPersonas;
  active: boolean;
}

export interface RolUsuarioPersonas {
  roleId: number;
  roleName: string;
  rolDescription: string;
}