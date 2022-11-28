using System;

namespace ModernizacionPersonas.BLL.Services
{
    public class ValoresPrimasDataProcessor : IResumenDataProcessor
    {
        private int codigoTipoTasa;
        private int codigoTipoSumaAsegurada;

        public ValoresPrimasDataProcessor(int codigoTipoTasa, int codigoTipoSumaAsegurada)
        {
            this.codigoTipoTasa = codigoTipoTasa;
            this.codigoTipoSumaAsegurada = codigoTipoSumaAsegurada;
        }

        public virtual decimal CalcularPrimaIndividualAnualxTasa(decimal valorPrima)
        {
            return valorPrima;
        }

        public virtual decimal CalcularPrimaIndividualAnual(decimal valorAsegurado, decimal tasaComercialAplicar)
        {
            return Math.Round(valorAsegurado * tasaComercialAplicar / 1000, 0);
        }

        public virtual decimal CalcularPrimaIndividualTotal(decimal valorAsistencia, decimal valorPrima)
        {
            return valorPrima + valorAsistencia;
        }

        public virtual decimal CalcularPrimaTotalAnualxTasa(decimal valorPrima, int numeroAsegurados)
        {
            switch (this.codigoTipoSumaAsegurada)
            {
                // Suma fija                
                case 1:
                    return valorPrima * numeroAsegurados;
                // Suma fija y múltiplo de sueldos
                case 5:
                    return valorPrima;
                // Multiplo de Sueldos
                case 2:
                // Suma Variable por asegurado 
                case 3:
                // Saldo Deudores-Ahorros-Aportes
                case 6:
                // SMMLV
                case 10:
                    return valorPrima;
                default:
                    return valorPrima * numeroAsegurados;
            }
        }

        public virtual decimal CalcularPrimaTotalAnual(decimal valorPrima, decimal valorAsistencia, int numeroAsegurados)
        {
            var primaIndividualTotal = this.CalcularPrimaIndividualTotal(valorPrima, valorAsistencia);
            var primaTotalAnualxTasa = this.CalcularPrimaTotalAnualxTasa(valorPrima, numeroAsegurados);
            var primaTotalAsistencia = valorAsistencia * numeroAsegurados;
            switch (this.codigoTipoSumaAsegurada)
            {
                case 3:
                    return valorPrima;
                default:
                    return primaTotalAnualxTasa + primaTotalAsistencia;
            }
        }
    }
}
