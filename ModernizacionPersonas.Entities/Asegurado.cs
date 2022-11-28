using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class Asegurado
    {
        public int CodigoAsegurado { get; set; }
        public int CodigoGrupoAsegurado { get; set; }
        public int CodigoTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Nombre { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string FechaOriginal { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int Genero { get; set; }
        public int NumeroSueldosAsegurado { get; set; }
        public decimal SaldoDeudaAsegurado { get; set; }
        public decimal ValorAhorroAsegurado { get; set; }
        public decimal ValorAportesAsegurado { get; set; }
        public decimal ValorSueldoAsegurado { get; set; }
        public decimal ValorAsegurado { get; set; }
        public decimal ValorPrimaAsegurado { get; set; }
        public int CentroCosto { get; set; }
        public int Edad { get; set; }
        public decimal ValorAseguradoFinal { get; }
        public bool VetadoSarlaft { get; set;}
        public Asegurado()
        {

        }

        public Asegurado(string numeroDocumento, string nombre, DateTime fechaNacimiento, string fechaOriginal, decimal valorAsegurado, int grupoAsegurado, int numeroSalarios = 0)
        {
            this.NumeroDocumento = numeroDocumento;
            this.Nombre = nombre;
            this.FechaNacimiento = fechaNacimiento;
            this.FechaOriginal = fechaOriginal;
            this.ValorAsegurado = valorAsegurado;
            this.ValorAseguradoFinal = numeroSalarios > 0 ? this.ValorAsegurado * numeroSalarios : this.ValorAsegurado;
            this.Edad = this.CalculateAge(fechaNacimiento);
            this.CodigoGrupoAsegurado = grupoAsegurado;
        }

        public Asegurado(int codigoAsegurado, string numeroDocumento, DateTime fechaNacimiento, decimal valorAsegurado)
        {
            //this.CodigoAsegurado = codigoAsegurado;
            this.NumeroDocumento = numeroDocumento;
            this.FechaNacimiento = fechaNacimiento;
            this.ValorAsegurado = valorAsegurado;
            this.Edad = this.CalculateAge(fechaNacimiento);
        }

        public Asegurado(int codigoAsegurado, string numeroDocumento, DateTime fechaNacimiento, decimal valorAsegurado, bool vetadoSarlaft)
        {
            //this.CodigoAsegurado = codigoAsegurado;
            this.NumeroDocumento = numeroDocumento;
            this.FechaNacimiento = fechaNacimiento;
            this.ValorAsegurado = valorAsegurado;
            this.Edad = this.CalculateAge(fechaNacimiento);
            this.VetadoSarlaft = vetadoSarlaft;
        }

        public int CalculateAge(DateTime dateOfBirth)
        {
            // TODO debe moverse a una carpeta de utilidades
            int age = 0;
            age = DateTime.Now.Year - dateOfBirth.Year;
            int currentYearDayOf = DateTime.Now.DayOfYear;
            int dirthDateYearDayOf = dateOfBirth.DayOfYear;
            if (DateTime.IsLeapYear(DateTime.Now.Year))
                currentYearDayOf -= 1;
            if (DateTime.IsLeapYear(dateOfBirth.Year))
                dirthDateYearDayOf -= 1;
            if (currentYearDayOf < dirthDateYearDayOf)
                age -= 1;

            return age;
        }
    }
}
