using ModernizacionPersonas.Common.Services;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class UploadAseguradosCSVMapper : IAseguradosMapper
    {
        private readonly AseguradosValidator validator;
        private readonly SARLAFTValidatorService SARLAFTValidator;

        public Stream FileStream { get; }
        private List<UploadAseguradoError> Errores { get; }
        public int TipoEstructura { get; set; }
        public int CodigoGrupoAsegurado { get; private set; }
        private readonly int numeroSalarios;

        public int TotalRegistros { get; set; }

        public UploadAseguradosCSVMapper(MapAseguradosViewModelArgs args, Stream fileStream)
        {
            this.SARLAFTValidator = new SARLAFTValidatorService();
            this.validator = new AseguradosValidator(new AseguradosValidatorAgs
            {
                EdadMaximaPerfil = args.EdadMaximaPerfil,
                EdadMinimaPerfil = args.EdadMinimaPerfil,
                EdadMaximaAmparoBasico = args.EdadMaximaAmparoBasico,
                EdadMinimaAmparoBasico = args.EdadMinimaAmparoBasico,
                ValorMaximoGrupoAsegurado = args.ValorMaximoGrupoAsegurado,
                ValorMinimoGrupoAsegurado = args.ValorMinimoGrupoAsegurado,
                ValorMaximoPerfil = args.ValorMaximoPerfil,
                ValorMinimoPerfil = args.ValorMinimoPerfil
            });

            this.CodigoGrupoAsegurado = args.CodigoGrupoAsegurados;
            this.numeroSalarios = args.NumeroSalarios;
            this.FileStream = fileStream;
            this.Errores = new List<UploadAseguradoError>();
            this.TipoEstructura = args.TipoEstructura;

        }

        public async Task<MapAseguradosResponse> MapAsync()
        {
            var count = 0;
            var asegurados = new List<Asegurado>();
            using (var r = new StreamReader(this.FileStream))
            {
                var idx = 0;
                while (!r.EndOfStream)
                {
                    string line = r.ReadLine();
                    var row = Regex.Split(line, @"[;]|[,]");
                    var errors = new List<string>();
                    if(idx == 0)
                    {
                        if (row.Count() < 3 || row.Count() > 3)
                        {
                            throw new Exception($"El archivo no tiene la estructura necesaria");
                        }
                        if (this.TipoEstructura == 1)
                        {
                            if (!row[0].ToLower().Equals("documento") || !row[1].ToLower().Equals("fecha nacimiento") || !row[2].ToLower().Equals("valor asegurado"))
                            {
                                throw new Exception($"El archivo no tiene la estructura correcta, de acuerdo a la opción seleccionada");
                            }
                        }
                        else if (this.TipoEstructura == 2)
                        {
                            if (!row[0].ToLower().Equals("documento") || !row[1].ToLower().Equals("edad") || !row[2].ToLower().Equals("valor asegurado"))
                            {
                                throw new Exception($"El archivo no tiene la estructura correcta, de acuerdo a la opción seleccionada");
                            }
                        }
                        else if (this.TipoEstructura == 3)
                        {
                            if (!row[0].ToLower().Equals("nombre") || !row[1].ToLower().Equals("fecha nacimiento") || !row[2].ToLower().Equals("valor asegurado"))
                            {
                                throw new Exception($"El archivo no tiene la estructura correcta, de acuerdo a la opción seleccionada");
                            }
                        }
                        else if (this.TipoEstructura == 4)
                        {
                            if (!row[0].ToLower().Equals("nombre") || !row[1].ToLower().Equals("edad") || !row[2].ToLower().Equals("valor asegurado"))
                            {
                                throw new Exception($"El archivo no tiene la estructura correcta, de acuerdo a la opción seleccionada");
                            }
                        }
                    }
                    
                    if (idx > 0)
                    {
                        count++;
                        string numeroDocumento = row[0];
                        string nombreCompleto = "";
                        string fachaOriginal = row[1];
                        DateTime fechaNacimiento = new DateTime();
                        try
                        {
                            if(this.TipoEstructura == 1 || this.TipoEstructura == 3)
                            {
                                fechaNacimiento = ParseDateTime(row[1]);
                            }
                            else
                                fechaNacimiento = DateTime.Now.AddYears(int.Parse(row[1]) * -1);
                            if (this.TipoEstructura == 3 || this.TipoEstructura == 4) 
                                nombreCompleto = row[0];
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"MapAsync :: CSV Mapper {row[1]}", ex);
                        }
                        var valorAsegurado = ParseValorAsegurado(row[2]);
                        var asegurado = new Asegurado(numeroDocumento, nombreCompleto, fechaNacimiento, fachaOriginal, valorAsegurado, this.CodigoGrupoAsegurado, this.numeroSalarios);

                        var response = validator.Validate(asegurado);
                        var isValidSARLAFT = this.TipoEstructura == 3 || this.TipoEstructura == 4 ? true : await this.ValidateSARLAFTAsync(asegurado);
                        if (response.Valid)
                        {
                           // asegurado.CodigoGrupoAsegurado = this.CodigoGrupoAsegurado;
                            asegurado.VetadoSarlaft = !isValidSARLAFT;
                            var existe = asegurados.Where(x => x.NumeroDocumento == asegurado.NumeroDocumento);
                            if (existe.Count() == 0)
                            {
                                asegurados.Add(asegurado);
                            }
                            else
                            {
                                
                                errors.Add("Registro duplicados");
                                this.Errores.Add(new UploadAseguradoError
                                {
                                    Asegurado = asegurado,
                                    Errors = errors
                                });
                            }
                        }

                        if (!response.Valid || !isValidSARLAFT)
                        {
                            
                            response.Messages.ToList().ForEach(error =>
                            {
                                errors.Add(error.Message);
                            });

                            var readableErrors = validator.GetValidationErrors(asegurado);
                            if (!isValidSARLAFT)
                            {
                                readableErrors.Add("Esta operación no se puede realizar. Favor comunicarse con la Gerencia Oficial de Cumplimiento");
                            }

                            this.Errores.Add(new UploadAseguradoError { Asegurado = asegurado, Errors = readableErrors });
                        }
                    }

                    idx++;
                }

                r.Close();
                this.TotalRegistros = count;
            }

            return new MapAseguradosResponse
            {
                Asegurados = asegurados,
                TotalRegistros = count,
                TotalRegistrosValidos = asegurados.Count(x => !x.VetadoSarlaft)
            };
        }

        private async Task<bool> ValidateSARLAFTAsync(Asegurado asegurado)
        {
            var edad = this.CalcularEdad(asegurado.FechaNacimiento);
            var codigoTipoDocumento = edad >= 18 ? 1 : 3;
            var isValid = await this.SARLAFTValidator.ValidateAsync(codigoTipoDocumento, asegurado.NumeroDocumento, asegurado.PrimerNombre, asegurado.SegundoNombre, asegurado.PrimerApellido, asegurado.SegundoApellido);

            return isValid;
        }

        private int CalcularEdad(DateTime fechaNacimiento)
        {
            int edad = 0;
            edad = DateTime.Now.Year - fechaNacimiento.Year;
            if (DateTime.Now.DayOfYear < fechaNacimiento.DayOfYear)
                edad = edad - 1;

            return edad;
        }


        private decimal ParseValorAsegurado(string str)
        {
            if (string.IsNullOrEmpty(str)) return 0;
            var parsed = decimal.TryParse(str, out decimal result);
            if (!parsed)
            {
                result = 0;
            }

            return result;
        }

        private DateTime ParseDateTime(string str)
        {
            if (string.IsNullOrEmpty(str)) return DateTime.Now;
            
            var parsed = DateTime.TryParse(str, out DateTime result);
            if (!parsed)
            {
                var dtParts = str.Split('/');
                var day = dtParts[0];
                var month = dtParts[1];
                var year = dtParts[2];
                result = DateTime.Parse($"{year}/{month}/{day}");
            }

            return result;
        }

        public IEnumerable<UploadAseguradoError> GetErrores()
        {
            return this.Errores;
        }
    }

    public class MapAseguradosArgs
    {
        public int CodigoGrupoAsegurado { get; set; }
        public string FileContentType { get; set; }
        public Stream FileStream { get; set; }
        public int EdadMinimaPerfil { get; set; }
        public int EdadMaximaPerfil { get; set; }
        public decimal ValorMinimoGrupoAsegurado { get; set; }
        public decimal ValorMaximoGrupoAsegurado { get; set; }
        public int NumeroSalarios { get; set; }
    }
}
