namespace ModernizacionPersonas.BLL.Services
{
    public class TasaPorEdadesValoresPrimasDataProcessor : ValoresPrimasDataProcessor, IResumenDataProcessor
    {
        private readonly int codigoTipoTasa;
        private readonly int codigoTipoSumaAsegurada;

        public TasaPorEdadesValoresPrimasDataProcessor(int codigoTipoTasa, int codigoTipoSumaAsegurada)
            : base(codigoTipoTasa, codigoTipoSumaAsegurada)
        {
            this.codigoTipoTasa = codigoTipoTasa;
            this.codigoTipoSumaAsegurada = codigoTipoSumaAsegurada;
        }

        public override decimal CalcularPrimaTotalAnual(decimal valorPrima, decimal valorAsistencia, int numeroAsegurados)
        {
            return valorPrima;
        }
    }
}
