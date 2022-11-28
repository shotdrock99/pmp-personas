export class Rol {
    codigo: number;
    nombre: string;
    descripcion: string;
    permisos: Permiso[];
}

export class Permiso {
    codigo: number; 
    nombre: string;
    descripcion: string;
}