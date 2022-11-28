import { IntermediarioState } from './intermediario-state';

export class Intermediario {
    Codigo?: number;
    Clave: string;
    TipoDocumento: any;
    NumeroDocumento: string;
    PrimerNombre: string;
    SegundoNombre?: string;
    PrimerApellido: string;
    SegundoApellido?: string;
    TipoPersona: string;
    PorcentajeParticipacion: number;
    Email:string;
    Estado: IntermediarioState;

    static CreateNew(): Intermediario {
        return new Intermediario();
    }
}
