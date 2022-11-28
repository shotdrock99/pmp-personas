using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.BLL.Services
{
    public interface IResumenDataProcessor
    {
        decimal CalcularPrimaIndividualAnualxTasa(decimal valorPrima);
        decimal CalcularPrimaIndividualAnual(decimal valorAsegurado, decimal tasaComercialAplicar);
        decimal CalcularPrimaIndividualTotal(decimal valorAsistencia, decimal valorPrima);
        decimal CalcularPrimaTotalAnualxTasa(decimal valorPrima, int numeroAsegurados);
        decimal CalcularPrimaTotalAnual(decimal valorPrima, decimal valorAsistencia, int numeroAsegurados);
    }
}
