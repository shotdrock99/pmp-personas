using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace ModernizacionPersonas.BLL.Services
{
    public class ValidarAseguradoResponse
    {
        public bool Valid { get; set; }

        public IEnumerable<Validation> Messages { get; set; }
    }

    public class AseguradosValidatorAgs
    {
        public int EdadMaximaPerfil { get; set; }
        public int EdadMinimaPerfil { get; set; }
        public decimal ValorMaximoGrupoAsegurado { get; set; }
        public decimal ValorMinimoGrupoAsegurado { get; set; }
        public int EdadMaximaAmparoBasico { get; internal set; }
        public int EdadMinimaAmparoBasico { get; internal set; }
        public decimal ValorMaximoPerfil { get; internal set; }
        public decimal ValorMinimoPerfil { get; internal set; }
    }

    public class AseguradosValidator
    {
        private readonly int edadMinimaAmparoBasico;
        private readonly int edadMaximaAmparoBasico;
        private readonly int edadMinimaPerfil;
        private readonly int edadMaximaPerfil;
        private readonly decimal valorMinimoGrupoAsegurado;
        private readonly decimal valorMaximoGrupoAsegurado;
        private readonly decimal valorMinimoPerfil;
        private readonly decimal valorMaximoPerfil;
        private readonly IDatosAseguradoWriter aseguradosWriter;
        private readonly IDatosAseguradoReader aseguradosReader;

        public AseguradosValidator(AseguradosValidatorAgs args)
        {
            this.edadMinimaAmparoBasico = args.EdadMinimaAmparoBasico;
            this.edadMaximaAmparoBasico = args.EdadMaximaAmparoBasico;
            this.edadMinimaPerfil = args.EdadMinimaPerfil;
            this.edadMaximaPerfil = args.EdadMaximaPerfil;
            this.valorMinimoGrupoAsegurado = args.ValorMinimoGrupoAsegurado;
            this.valorMaximoGrupoAsegurado = args.ValorMaximoGrupoAsegurado;
            this.valorMinimoPerfil = args.ValorMinimoPerfil;
            this.valorMaximoPerfil = args.ValorMaximoPerfil;
            this.aseguradosWriter = new DatosAseguradoTableWriter();
            this.aseguradosReader = new DatosAseguradoTableReader();
        }

        public ValidarAseguradoResponse Validate(Asegurado asegurado)
        {
            var asegurados = this.aseguradosReader.LeerAseguradosAsync(asegurado.CodigoGrupoAsegurado).Result;
            var aseguradoOld = new Asegurado();
            if (asegurados.Count() > 0)
            {
                aseguradoOld = asegurados.Where(x => x.NumeroDocumento == asegurado.NumeroDocumento).FirstOrDefault();
                if(aseguradoOld == null)
                {
                    aseguradoOld = asegurados.FirstOrDefault();
                }
            }

            var builder = Validator<Asegurado>.Builder;
            var validator = builder
                // el documento es diferente de vacio
                .Where(x => !string.IsNullOrEmpty(x.NumeroDocumento))
                // la fecha de nacimiento es mayor a hoy menos un dia
                .Where(x => x.FechaNacimiento < DateTime.Now.AddDays(-1))
                // el valor asegurado es mayor a 0
                .Where(x => x.ValorAsegurado > 0)
                // el valor esta dentro del rango de valor permitido configurado en el grupo de asegurados
                .Where(x => x.ValorAseguradoFinal >= this.valorMinimoGrupoAsegurado && x.ValorAseguradoFinal <= this.valorMaximoGrupoAsegurado)
                // el valor esta dentro del rango de valor permitido configurado en el perfil de valores
                .Where(x => x.ValorAseguradoFinal >= this.valorMinimoPerfil && x.ValorAseguradoFinal <= this.valorMaximoPerfil)
                // la edad esta dentro del rango permitido del amparo basico
                .Where(x => x.Edad >= this.edadMinimaAmparoBasico && x.Edad <= this.edadMaximaAmparoBasico)
                // la edad esta dentro del rango permitido del perfil de edades
                .Where(x => x.Edad >= this.edadMinimaPerfil && x.Edad <= this.edadMaximaPerfil)
                // Existe el asegurado con numero de documento
                .Where(x => x.ValorAsegurado != aseguradoOld.ValorAsegurado || x.FechaNacimiento.ToShortDateString() != aseguradoOld.FechaNacimiento.ToShortDateString())
                //x.ValorAsegurado == aseguradoOld.ValorAsegurado
                //x.FechaNacimiento.ToShortDateString() == aseguradoOld.FechaNacimiento.ToShortDateString()
                .Build();

            

            var validation = validator.Validate(asegurado);
            var isValid = validation.All(x => x.Success);
            return new ValidarAseguradoResponse
            {
                Valid = isValid,
                Messages = validation.Where(x => !x.Success)
            };
        }

        public List<string> GetValidationErrors(Asegurado asegurado)
        {
            var asegurados = this.aseguradosReader.LeerAseguradosAsync(asegurado.CodigoGrupoAsegurado).Result;
            var result = new List<string>();
            if (asegurado.FechaNacimiento >= DateTime.Now)
            {
                result.Add("La fecha de nacimiento no puede ser mayor a hoy.");
            }

            if (string.IsNullOrEmpty(asegurado.NumeroDocumento))
            {
                result.Add("El número de documento no puede estar vacío.");
            }

            if (asegurado.ValorAsegurado == 0)
            {
                result.Add("El valor asegurado no puede ser cero.");
            }

            if (asegurado.ValorAseguradoFinal < this.valorMinimoGrupoAsegurado || asegurado.ValorAseguradoFinal > this.valorMaximoGrupoAsegurado)
            {
                result.Add("El valor asegurado no esta dentro del rango permitido configurado en el grupo de asegurados.");
            }

            if (asegurado.ValorAseguradoFinal < this.valorMinimoPerfil || asegurado.ValorAseguradoFinal > this.valorMaximoPerfil)
            {
                result.Add("El valor asegurado no esta dentro del rango permitido configurado en el perfil de valores.");
            }

            if (asegurado.Edad < this.edadMinimaAmparoBasico || asegurado.Edad > this.edadMaximaAmparoBasico)
            {
                result.Add("La edad del asegurado no esta dentro del rango permitido del amparo basico.");
            }

            if (asegurado.Edad < this.edadMinimaPerfil || asegurado.Edad > this.edadMaximaPerfil)
            {
                result.Add("La edad del asegurado no esta dentro del rango permitido del perfil de edades.");
            }

            var aseguradosValidate = aseguradosWriter.ValidarAseguradosAsync(asegurado.CodigoGrupoAsegurado, asegurado.NumeroDocumento);

            var aseguradoOld = new Asegurado();
            if (asegurados.Count() > 0)
            {
                aseguradoOld = asegurados.Where(x => x.NumeroDocumento == asegurado.NumeroDocumento).FirstOrDefault();
            }

            if (aseguradosValidate.Result.RegistrosProcesados > 0)
            {
                if (aseguradoOld.FechaNacimiento.ToShortDateString() == asegurado.FechaNacimiento.ToShortDateString() && aseguradoOld.ValorAsegurado == asegurado.ValorAsegurado)
                    result.Add("Registro duplicado.");
            }

            return result;
        }
    }

    public class Validator<T>
    {
        private readonly IEnumerable<ValidationRule<T>> _rules;

        public Validator(IEnumerable<ValidationRule<T>> rules)
        {
            _rules = rules;
        }

        public static ValidatorBuilder<T> Builder => new ValidatorBuilder<T>();

        public bool IsValid(T obj)
        {
            return _rules.All(x => x.IsMet(obj));
        }

        public IEnumerable<Validation> Validate(T obj)
        {
            if (obj == null)
            {
                yield return new Validation(false, $"Object of type {typeof(T).Name} does not meet the requirement: ({typeof(T).Name} != null)");
                yield break;
            }

            foreach (var rule in _rules)
            {
                var isValid = rule.IsMet(obj);
                yield return new Validation(
                    isValid,
                    isValid
                        ? $"Object of type {typeof(T).Name} meets the requirement: {rule}"
                        : $"Object of type {typeof(T).Name} does not meet the requirement: {rule}");
            }
        }
    }

    public class ValidatorBuilder<T>
    {
        private readonly List<ValidationRule<T>> _rules = new List<ValidationRule<T>>();

        public ValidatorBuilder<T> Where(Expression<Func<T, bool>> expression)
        {
            var expressionString = expression.ToString();

            var variableName = Regex.Match(expressionString, "^([a-z0-9_]+) => ").Groups[1].Value;
            expressionString = Regex.Replace(expressionString, "^[a-z0-9_]+ => ", string.Empty);
            expressionString = Regex.Replace(expressionString, $"{variableName}\\.", $"{typeof(T).Name}.");

            _rules.Add(new ValidationRule<T>(expressionString, expression.Compile()));
            return this;
        }

        public Validator<T> Build()
        {
            return new Validator<T>(_rules);
        }
    }

    public class ValidationRule<T>
    {
        private readonly string _expression;
        private readonly Func<T, bool> _predicate;

        public ValidationRule(string expression, Func<T, bool> predicate)
        {
            _expression = expression;
            _predicate = predicate;
        }

        public bool IsMet(T obj) => _predicate(obj);

        public override string ToString() => _expression;
    }

    public class Validation
    {
        public Validation(bool success, string message)
        {
            Success = success;
            Message = message;
        }
        public bool Success { get; }
        public string Message { get; }
    }
}
