export class Municipio {
    codigoDepartamento: number;
    codigoDivipola: number;
    codigoMunicipio: number;
    codigoPais: number;
    nombreMunicipio: string;

    static create(NombreMunicipio: string) {
        let result = new Municipio()
        result.codigoDepartamento = 0;
        result.codigoDivipola = 0;
        result.codigoMunicipio = 0;
        result.codigoPais = 0;
        result.nombreMunicipio = '';

        return result;
    };
}
